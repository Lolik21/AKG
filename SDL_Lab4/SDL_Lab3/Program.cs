using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        private static readonly (byte red, byte green, byte blue, byte alpha) RedColor = (255, 0, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) GreenColor = (0, 255, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) BlueColor = (0, 0, 255, 255);

        private static double _xAngle = Math.PI / 4;
        private static double _yAngle = Math.PI / 4;
        private static double _zAngle = -Math.PI / 2;
        //private static double _xAngle = 0d;
        //private static double _yAngle = 0d;
        //private static double _zAngle = 0d;

        private static int d = 600;

        private const int dSpeed = 10;
        private const double RotatingSpeed = Math.PI/36;

        private const int DashLength = 5;

        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;

        private static IntPtr _window;
        private static IntPtr _renderer;

        private static Figure _figure;
        private static Figure _axis;

        #region SDLInit

        [STAThread]
        public static void Main()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) >= 0)
            {
                var window = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED,
                    SDL.SDL_WINDOWPOS_UNDEFINED, WindowWidth, WindowHeight,
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED | SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
                _window = window;
                _renderer = SDL.SDL_CreateRenderer(window, -1, 0);
                _figure = InitFigure();
                _axis = InitAxis();

                var exitApp = false;
                while (!exitApp)
                {
                    while (SDL.SDL_PollEvent(out var e) != 0)
                    {
                        exitApp = SdlWindProc(e);
                    }
                    DrawGreatLine();
                }
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
            }
        }

        private static Figure InitFigure()
        {
            var delta = 600;
            var center = FindCenter();
            var p0 = new Vertex {X = -100, Y = 100, Z = 100 + delta}.Sum(center);
            var p1 = new Vertex {X = -100, Y = 100, Z = -100 + delta}.Sum(center);
            var p2 = new Vertex {X = 0, Y = 100, Z = -100 + delta }.Sum(center);
            var p3 = new Vertex {X = 0, Y = 100, Z = 0 + delta }.Sum(center);
            var p4 = new Vertex {X = 100, Y = 100, Z = 0 + delta }.Sum(center);
            var p5 = new Vertex {X = 100, Y = 100, Z = 100 + delta }.Sum(center);
            var p6 = new Vertex {X = -100, Y = 0, Z = -100 + delta }.Sum(center);
            var p7 = new Vertex {X = 0, Y = 0, Z = -100 + delta }.Sum(center);
            var p8 = new Vertex {X = 0, Y = 0, Z = 0 + delta }.Sum(center);
            var p9 = new Vertex {X = 100, Y = 0, Z = 0 + delta }.Sum(center);
            var p10 = new Vertex {X = 100, Y = 0, Z = 100 + delta }.Sum(center);
            var p11 = new Vertex {X = 0, Y = 0, Z = 100 + delta }.Sum(center);
            var p12 = new Vertex {X = -100, Y = 0, Z = 0 + delta }.Sum(center);
            var p13 = new Vertex {X = -100, Y = -100, Z = 0 + delta }.Sum(center);
            var p14 = new Vertex {X = 0, Y = -100, Z = 0 + delta }.Sum(center);
            var p15 = new Vertex {X = 0, Y = -100, Z = 100 + delta }.Sum(center);
            var p16 = new Vertex {X = -100, Y = -100, Z = 100 + delta }.Sum(center);
            var p17 = new Vertex {X = p12.X, Y = p1.Y, Z = p12.Z, IsVisible = false};
            var p18 = new Vertex {X = p12.X, Y = p12.Y, Z = p16.Z, IsVisible = false};
            var p19 = new Vertex {X = p11.X, Y = p5.Y, Z = p11.Z, IsVisible = false};

            var polList = new List<Polygon>
            {
                CreatePolygon(p1, p2, p7, p6),
                CreatePolygon(p2, p3, p8, p7),
                CreatePolygon(p3, p4, p9, p8),
                CreatePolygon(p4, p5, p10, p9),
                CreatePolygon(p6, p7, p8, p12),
                CreatePolygon(p12, p8, p14, p13),
                CreatePolygon(p13, p14, p15, p16),
                CreatePolygon(p8, p11, p15, p14),
                CreatePolygon(p8, p9, p10, p11)
            };
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p1, p6, GreenColor),
                    new Edge(p6, p12, GreenColor),
                    new Edge(p12, p17, GreenColor, false),
                    new Edge(p17, p1, GreenColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p16, p18, GreenColor),
                    new Edge(p18, p12, GreenColor, false),
                    new Edge(p12, p13, GreenColor),
                    new Edge(p13, p16, GreenColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p17, RedColor),
                    new Edge(p17, p12, GreenColor, false),
                    new Edge(p12, p18, GreenColor, false),
                    new Edge(p18, p0, RedColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p11, p18, GreenColor, false),
                    new Edge(p18, p16, GreenColor),
                    new Edge(p16, p15, GreenColor),
                    new Edge(p15, p11, GreenColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p0, RedColor),
                    new Edge(p0, p18, RedColor),
                    new Edge(p18, p11, GreenColor, false),
                    new Edge(p11, p19, GreenColor, false),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p5, p19, GreenColor),
                    new Edge(p19, p11, GreenColor, false),
                    new Edge(p11, p10, GreenColor),
                    new Edge(p10, p5, GreenColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p17, p3, GreenColor, false),
                    new Edge(p3, p2, GreenColor),
                    new Edge(p2, p1, GreenColor),
                    new Edge(p1, p17, GreenColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p19, RedColor),
                    new Edge(p19, p3, GreenColor, false),
                    new Edge(p3, p17, GreenColor, false),
                    new Edge(p17, p0, RedColor),
                }
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p5, GreenColor),
                    new Edge(p5, p4, GreenColor),
                    new Edge(p4, p3, GreenColor),
                    new Edge(p3, p19, GreenColor, false),
                }
            });


            return new Figure(new Vertex(0, 0, 0+delta).Sum(center), polList);
        }

        private static Polygon CreatePolygon(Vertex v1, Vertex v2, Vertex v3, Vertex v4) => new Polygon
        {
            Edges = new List<Edge>
            {
                new Edge(v1, v2, GreenColor),
                new Edge(v2, v3, GreenColor),
                new Edge(v3, v4, GreenColor),
                new Edge(v4, v1, GreenColor)
            }
        };

        private static Figure InitAxis()
        {
            var center = FindCenterForAxis();

            var pol1 = new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge
                    {
                        Start = new Vertex(0, 0, 0).Sum(center),
                        End = new Vertex(50, 0, 0).Sum(center),
                        Color = (255, 0, 0, 255)
                    }
                }
            };
            var pol2 = new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge
                    {
                        Start = new Vertex(0, 0, 0).Sum(center),
                        End = new Vertex(0, 50, 0).Sum(center),
                        Color = (0, 255, 0, 255)
                    }
                }
            };

            var pol3 = new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge
                    {
                        Start = new Vertex(0, 0, 0).Sum(center),
                        End = new Vertex(0, 0, 50).Sum(center),
                        Color = (0, 0, 255, 255)
                    }
                }
            };

            return new Figure(new Vertex(0, 0, 0).Sum(center), new List<Polygon> {pol1, pol2, pol3});
        }

        private static bool SdlWindProc(SDL.SDL_Event sdlEvent)
        {
            switch (sdlEvent.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    return true;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    switch (sdlEvent.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_F4:
                        case SDL.SDL_Keycode.SDLK_ESCAPE:
                            return true;
                        case SDL.SDL_Keycode.SDLK_d:
                            _xAngle += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_a:
                            _xAngle -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_s:
                            _yAngle += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_w:
                            _yAngle -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_e:
                            _zAngle += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_q:
                            _zAngle -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_r:
                            d-=dSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_f:
                            d+=dSpeed;
                            break;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region Drawing

        private static void DrawGreatLine()
        {
            var renderer = _renderer;
            SDL.SDL_SetRenderDrawColor(renderer, 50, 50, 50, 255);
            SDL.SDL_RenderClear(renderer);
            DrawFigures(renderer);
            DrawAxis(renderer);
            SDL.SDL_RenderPresent(renderer);
        }

        private static void DrawAxis(IntPtr renderer)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            var rect = new SDL.SDL_Rect
            {
                x = (int) Math.Round(_axis.Pivot.X - _axis.Edges.First().Length()),
                y = (int) Math.Round(_axis.Pivot.Y - _axis.Edges.First().Length()),
                h = (int) Math.Round(_axis.Edges.First().Length()*2),
                w = (int) Math.Round(_axis.Edges.First().Length()*2)
            };
            SDL.SDL_RenderFillRect(renderer, ref rect);

            var fig = _axis
                .RotateByAngleAndAxis(_xAngle, Axis.X)
                .RotateByAngleAndAxis(_yAngle, Axis.Y)
                .RotateByAngleAndAxis(_zAngle, Axis.Z);
            var edges = fig.Edges;
            
            edges.Sort((edge, edge1) =>
            {
                if (edge.End.Z > edge1.End.Z)
                    return -1;
                return edge.End.Z == edge1.End.Z ? 0 : 1;
            });

            foreach (var edge in edges)
            {
                SDL.SDL_SetRenderDrawColor(renderer, edge.Color.red, edge.Color.green, edge.Color.blue, edge.Color.alpha);
                SDL.SDL_RenderDrawLine(renderer, (int)edge.Start.X, (int)edge.Start.Y, (int)edge.End.X, (int)edge.End.Y);
            }
        }

        private static void DrawFigures(IntPtr renderer)
        {
            var fig = _figure
                .RotateByAngleAndAxis(_xAngle, Axis.X)
                .RotateByAngleAndAxis(_yAngle, Axis.Y)
                .RotateByAngleAndAxis(_zAngle, Axis.Z)
                .PerspectiveProjection(d, FindCenter())
                ;

            foreach (var edge in fig.Edges.Where(edge => edge.IsVisible))
            {
                SDL.SDL_SetRenderDrawColor(renderer, edge.Color.red, edge.Color.green, edge.Color.blue, edge.Color.alpha);
                SDL.SDL_RenderDrawLine(renderer, (int)edge.Start.X, (int)edge.Start.Y, (int)edge.End.X, (int)edge.End.Y);
            }
        }

        private static void DrawDashLines(IntPtr renderer, List<Point> points)
        {
            for (int i = 0; i < points.Count / 2; i++)
            {
                var startPoint = points[2 * i];
                var endPoint = points[2 * i + 1];
                var length = Math.Sqrt((endPoint.X - startPoint.X) * (endPoint.X - startPoint.X) +
                                       (endPoint.Y - startPoint.Y) * (endPoint.Y - startPoint.Y));
                var dashCount = Math.Ceiling(length / DashLength);
                var dt = 1 / dashCount;
                var line = new List<Point>
                {
                    new Point
                    {
                        X = startPoint.X,
                        Y = startPoint.Y
                    },
                    new Point
                    {
                        X = startPoint.X + (int) Math.Round(dt*(endPoint.X - startPoint.X)),
                        Y = startPoint.Y + (int) Math.Round(dt*(endPoint.Y - startPoint.Y))
                    }
                };
                for (var j = 0; j < dashCount; j += 2)
                {
                    //SDL.SDL_RenderDrawLines(renderer, MapListOfPoints(line).ToArray(), line.Count);
                    line[0] = new Point
                    {
                        X = line[0].X + (int)Math.Round(2 * dt * (endPoint.X - startPoint.X)),
                        Y = line[0].Y + (int)Math.Round(2 * dt * (endPoint.Y - startPoint.Y))
                    };
                    line[1] = new Point
                    {
                        X = line[1].X + (int)Math.Round(2 * dt * (endPoint.X - startPoint.X)),
                        Y = line[1].Y + (int)Math.Round(2 * dt * (endPoint.Y - startPoint.Y))
                    };
                }
            }
        }

        #endregion

        #region PointMappings

        private static Vertex FindCenter()
        {
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            return new Vertex(width/2, height/2, 0);
        }

        private static Vertex FindCenterForAxis()
        {
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            return new Vertex(width - 60, height - 60, 0);
        }

        #endregion
    }
}
