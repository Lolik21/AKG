using System;
using System.Collections.Generic;
using System.Linq;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        #region Constants and Variables

        private static bool _projection = false;
        private static bool _showInvisible = false;
        private static bool _showNormals = false;

        private static readonly (byte red, byte green, byte blue, byte alpha) RedColor = (255, 0, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) GreenColor = (0, 255, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) BlueColor = (0, 0, 255, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) YellowColor = (255, 255, 0, 255);

        private static double _xAngle = Math.PI / 4;
        private static double _yAngle = Math.PI / 4;
        private static double _zAngle = -Math.PI / 2;
        private static int d = 600;

        private static Vertex _observerVector = new Vertex(0, 0, 50);

        private const int dSpeed = 10;
        private const double RotatingSpeed = Math.PI/36;

        private const int DashLength = 5;

        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;

        private static IntPtr _window;
        private static IntPtr _renderer;

        private static Figure _figure;
        private static Figure _axis;

        delegate void DrawFunc(IntPtr renderer, Vertex p1, Vertex p2);

        private static SortedSet<int> _drawedSet = new SortedSet<int>();

        #endregion

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
                //CreatePolygon(p1, p2, p7, p6, 1),
                //CreatePolygon(p2, p3, p8, p7,2),
                //CreatePolygon(p3, p4, p9, p8,3),
                //CreatePolygon(p4, p5, p10, p9,4),
                //CreatePolygon(p6, p7, p8, p12,5),
                //CreatePolygon(p12, p8, p14, p13,6),
                //CreatePolygon(p13, p14, p15, p16,7),
                //CreatePolygon(p8, p11, p15, p14,8),
                //CreatePolygon(p8, p9, p10, p11,9)
            };
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p1, p2, YellowColor,0),
                    new Edge(p2, p7, YellowColor,1),
                    new Edge(p7, p6, YellowColor,2),
                    new Edge(p6, p1, YellowColor,3)
                },
                Number = 1
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p2, p3, YellowColor,4),
                    new Edge(p3, p8, YellowColor,5),
                    new Edge(p8, p7, YellowColor,6),
                    new Edge(p7, p2, YellowColor,1)
                },
                Number = 2
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p3, p4, YellowColor,7),
                    new Edge(p4, p9, YellowColor,8),
                    new Edge(p9, p8, YellowColor,9),
                    new Edge(p8, p3, YellowColor,5)
                },
                Number = 3
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p4, p5, YellowColor,10),
                    new Edge(p5, p10, YellowColor,11),
                    new Edge(p10, p9, YellowColor,12),
                    new Edge(p9, p4, YellowColor,8)
                },
                Number = 4
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p6, p7, YellowColor,2),
                    new Edge(p7, p8, YellowColor,6),
                    new Edge(p8, p12, YellowColor,13),
                    new Edge(p12, p6, YellowColor,14)
                },
                Number = 5
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p12, p8, YellowColor,13),
                    new Edge(p8, p14, YellowColor,15),
                    new Edge(p14, p13, YellowColor,16),
                    new Edge(p13, p12, YellowColor,17)
                },
                Number = 6
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p13, p14, YellowColor,16),
                    new Edge(p14, p15, YellowColor,18),
                    new Edge(p15, p16, YellowColor,19),
                    new Edge(p16, p13, YellowColor,20)
                },
                Number = 7
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p8, p11, YellowColor,21),
                    new Edge(p11, p15, YellowColor,22),
                    new Edge(p15, p14, YellowColor,18),
                    new Edge(p14, p8, YellowColor,15)
                },
                Number = 8
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p8, p9, YellowColor,9),
                    new Edge(p9, p10, YellowColor,12),
                    new Edge(p10, p11, YellowColor,23),
                    new Edge(p11, p8, YellowColor,21)
                },
                Number = 9
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p1, p6, YellowColor,3),
                    new Edge(p6, p12, YellowColor,14),
                    new Edge(p12, p17, YellowColor,24, false),
                    new Edge(p17, p1, YellowColor,25),
                },
                Number = 10
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p16, p18, YellowColor,26),
                    new Edge(p18, p12, YellowColor,27, false),
                    new Edge(p12, p13, YellowColor,17),
                    new Edge(p13, p16, YellowColor,20),
                },
                Number = 11
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p17, BlueColor,28),
                    new Edge(p17, p12, YellowColor,24, false),
                    new Edge(p12, p18, YellowColor,27, false),
                    new Edge(p18, p0, GreenColor,29),
                },
                Number = 12
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p11, p18, YellowColor,30, false),
                    new Edge(p18, p16, YellowColor,26),
                    new Edge(p16, p15, YellowColor,19),
                    new Edge(p15, p11, YellowColor,22),
                },
                Number = 13
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p0, RedColor,31),
                    new Edge(p0, p18, GreenColor,29),
                    new Edge(p18, p11, YellowColor,30, false),
                    new Edge(p11, p19, YellowColor,32, false),
                },
                Number = 14
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p5, p19, YellowColor,33),
                    new Edge(p19, p11, YellowColor,32, false),
                    new Edge(p11, p10, YellowColor,23),
                    new Edge(p10, p5, YellowColor,11),
                },
                Number = 15
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p17, p3, YellowColor,34, false),
                    new Edge(p3, p2, YellowColor,4),
                    new Edge(p2, p1, YellowColor,0),
                    new Edge(p1, p17, YellowColor,25),
                },
                Number = 16
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p19, RedColor,31),
                    new Edge(p19, p3, YellowColor,35, false),
                    new Edge(p3, p17, YellowColor,34, false),
                    new Edge(p17, p0, BlueColor,28),
                },
                Number = 17
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p5, YellowColor,33),
                    new Edge(p5, p4, YellowColor,10),
                    new Edge(p4, p3, YellowColor,7),
                    new Edge(p3, p19, YellowColor,35, false),
                },
                Number = 18
            });


            return new Figure(new Vertex(0, 0, 0+delta).Sum(center), polList);
        }

        //private static Polygon CreatePolygon(Vertex v1, Vertex v2, Vertex v3, Vertex v4, int number) => new Polygon
        //{
        //    Edges = new List<Edge>
        //    {
        //        new Edge(v1, v2, YellowColor),
        //        new Edge(v2, v3, YellowColor),
        //        new Edge(v3, v4, YellowColor),
        //        new Edge(v4, v1, YellowColor)
        //    },
        //    Number = number
        //};

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
                            d += dSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_p:
                            _projection = !_projection;
                            break;
                        case SDL.SDL_Keycode.SDLK_i:
                            _showInvisible = !_showInvisible;
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
            _drawedSet.Clear();
            var fig = _figure
                .RotateByAngleAndAxis(_xAngle, Axis.X)
                .RotateByAngleAndAxis(_yAngle, Axis.Y)
                .RotateByAngleAndAxis(_zAngle, Axis.Z)
                ;

            if (_projection)
            {
                fig = fig.PerspectiveProjection(d, FindCenter());
            }

            var facePolys = fig.Polygons
                .Where(polygon => polygon.IsVisible)
                .Where(polygon => _observerVector.ScalarMultiply(polygon.NormVector()) > 0)
                .ToList();

            Draw(renderer, facePolys, DrawVisible);
            
            var invizPolys = fig.Polygons
                .Where(polygon => polygon.IsVisible)
                .Where(polygon => _observerVector.ScalarMultiply(polygon.NormVector()) < 0)
                .ToList();

            Draw(renderer, invizPolys, DrawDashLines);
        }

        private static void Draw(IntPtr renderer, List<Polygon> polys, DrawFunc func)
        {
            foreach (var polygon in polys)
            {
                foreach (var edge in polygon.Edges.Where(edge => edge.IsVisible))
                {
                    if(_drawedSet.Contains(edge.Number)) continue;
                    SDL.SDL_SetRenderDrawColor(renderer, edge.Color.red, edge.Color.green, edge.Color.blue, edge.Color.alpha);
                    func(renderer, edge.Start, edge.End);
                    _drawedSet.Add(edge.Number);
                }

                if (_showNormals)
                {
                    var norm = polygon.NormVector().NormalizeVector().MultiplyByScalar(150);
                    norm = norm.Sum(FindCenter());
                    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderDrawLine(renderer,
                        (int)norm.X, (int)norm.Y,
                        (int)polygon.Edges[1].End.X, (int)polygon.Edges[1].End.Y);
                }
            }
        }

        private static void DrawVisible(IntPtr renderer, Vertex p1, Vertex p2)
        {
            SDL.SDL_RenderDrawLine(renderer, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
        }

        private static void DrawDashLines(IntPtr renderer, Vertex startPoint, Vertex endPoint)
        {
            var length = Math.Sqrt((endPoint.X - startPoint.X) * (endPoint.X - startPoint.X) +
                                    (endPoint.Y - startPoint.Y) * (endPoint.Y - startPoint.Y));
            var dashCount = Math.Ceiling(length / DashLength);
            var dt = 1 / dashCount;

            var p1 = new SDL.SDL_Point
            {
                x = (int) startPoint.X,
                y = (int) startPoint.Y
            };

            var p2 = new SDL.SDL_Point
            {
                x = (int) startPoint.X + (int) Math.Round(dt*(endPoint.X - startPoint.X)),
                y = (int) startPoint.Y + (int) Math.Round(dt*(endPoint.Y - startPoint.Y))
            };
            
            for (var j = 0; j < dashCount; j += 2)
            {
                SDL.SDL_RenderDrawLine(renderer, p1.x, p1.y, p2.x, p2.y);
                p1 = new SDL.SDL_Point
                {
                    x = p1.x + (int)Math.Round(2 * dt * (endPoint.X - startPoint.X)),
                    y = p1.y + (int)Math.Round(2 * dt * (endPoint.Y - startPoint.Y))
                };
                p2 = new SDL.SDL_Point
                {
                    x = p2.x + (int)Math.Round(2 * dt * (endPoint.X - startPoint.X)),
                    y = p2.y + (int)Math.Round(2 * dt * (endPoint.Y - startPoint.Y))
                };
            }
        }

        #endregion

        #region Centers functions

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
