using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        #region Constants and Variables

        private static bool _projection = false;
        private static bool _showInvisible = false;
        private static bool _showNormals = false;
        private static bool _showNormalsInside = false;
        private static bool _differentColor = false;
        private static bool _showAxis = false;
        private static bool _showVector = false;

        private static readonly (byte red, byte green, byte blue, byte alpha) RedColor = (255, 0, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) GreenColor = (0, 255, 0, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) BlueColor = (0, 0, 255, 255);
        private static readonly (byte red, byte green, byte blue, byte alpha) YellowColor = (255, 255, 0, 255);


        private static double _xAngle = Math.PI / 4;
        private static double _yAngle = Math.PI / 4;
        private static double _zAngle = -Math.PI / 2;
        private static double _vectorAngle = 0;

        private static int _dx = 0;
        private static int _dy = 0;
        private static int _dz = 0;

        private static int _d = -600;
        private static double Teta = 0;
        private static double Fi = 0;

        private const int DSpeed = 10;
        private const int TransitionSpeed = 10;
        private const double RotatingSpeed = Math.PI/36;

        private const int DashLength = 5;
        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;
        private const double E = 0.0001d;

        private static IntPtr _window;
        private static IntPtr _renderer;

        private static Figure _figure;
        private static Figure _axis;

        private static readonly Vertex ObserverVector = new Vertex(0, 0, 50);
        private static readonly SortedSet<int> DrawedSet = new SortedSet<int>();

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
            var delta = -600;
            var center = FindCenter();
            var p0  = new Vertex {X = -100, Y = 100, Z = 100 + delta}.Sum(center);
            var p1  = new Vertex {X = -100, Y = 100, Z = -100 + delta}.Sum(center);
            var p2  = new Vertex {X = 0, Y = 100, Z = -100 + delta }.Sum(center);
            var p3  = new Vertex {X = 0, Y = 100, Z = 0 + delta }.Sum(center);
            var p4  = new Vertex {X = 100, Y = 100, Z = 0 + delta }.Sum(center);
            var p5  = new Vertex {X = 100, Y = 100, Z = 100 + delta }.Sum(center);
            var p6  = new Vertex {X = -100, Y = 0, Z = -100 + delta }.Sum(center);
            var p7  = new Vertex {X = 0, Y = 0, Z = -100 + delta }.Sum(center);
            var p8  = new Vertex {X = 0, Y = 0, Z = 0 + delta }.Sum(center);
            var p9  = new Vertex {X = 100, Y = 0, Z = 0 + delta }.Sum(center);
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

            var polList = new List<Polygon>();
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p1, p2, YellowColor,0),
                    new Edge(p2, p7, BlueColor,1),
                    new Edge(p7, p6, YellowColor,2),
                    new Edge(p6, p1, GreenColor,3)
                },
                Number = 1
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p2, p3, BlueColor,4),
                    new Edge(p3, p8, BlueColor,5),
                    new Edge(p8, p7, BlueColor,6),
                    new Edge(p7, p2, BlueColor,1)
                },
                Number = 2
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p3, p4, YellowColor,7),
                    new Edge(p4, p9, RedColor,8),
                    new Edge(p9, p8, YellowColor,9),
                    new Edge(p8, p3, BlueColor,5)
                },
                Number = 3
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p4, p5, RedColor,10),
                    new Edge(p5, p10, RedColor,11),
                    new Edge(p10, p9, RedColor,12),
                    new Edge(p9, p4, RedColor,8)
                },
                Number = 4
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p6, p7, YellowColor,2),
                    new Edge(p7, p8, BlueColor,6),
                    new Edge(p8, p12, YellowColor,13),
                    new Edge(p12, p6, GreenColor,14)
                },
                Number = 5
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p12, p8, YellowColor,13),
                    new Edge(p8, p14, BlueColor,15),
                    new Edge(p14, p13, YellowColor,16),
                    new Edge(p13, p12, GreenColor,17)
                },
                Number = 6
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p13, p14, YellowColor,16),
                    new Edge(p14, p15, BlueColor,18),
                    new Edge(p15, p16, YellowColor,19),
                    new Edge(p16, p13, GreenColor,20)
                },
                Number = 7
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p8, p11, BlueColor,21),
                    new Edge(p11, p15, BlueColor,22),
                    new Edge(p15, p14, BlueColor,18),
                    new Edge(p14, p8, BlueColor,15)
                },
                Number = 8
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p8, p9, YellowColor,9),
                    new Edge(p9, p10, RedColor,12),
                    new Edge(p10, p11, YellowColor,23),
                    new Edge(p11, p8, BlueColor,21)
                },
                Number = 9
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p1, p6, GreenColor,3),
                    new Edge(p6, p12, GreenColor,14),
                    new Edge(p12, p17, GreenColor,24, false),
                    new Edge(p17, p1, GreenColor,25),
                },
                Number = 10
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p16, p18, GreenColor,26),
                    new Edge(p18, p12, GreenColor,27, false),
                    new Edge(p12, p13, GreenColor,17),
                    new Edge(p13, p16, GreenColor,20),
                },
                Number = 11
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p17, GreenColor,28),
                    new Edge(p17, p12, GreenColor,24, false),
                    new Edge(p12, p18, GreenColor,27, false),
                    new Edge(p18, p0, GreenColor,29),
                },
                Number = 12
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p11, p18, BlueColor,30, false),
                    new Edge(p18, p16, GreenColor,26),
                    new Edge(p16, p15, YellowColor,19),
                    new Edge(p15, p11, BlueColor,22),
                },
                Number = 13
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p0, YellowColor,31),
                    new Edge(p0, p18, GreenColor,29),
                    new Edge(p18, p11, BlueColor,30, false),
                    new Edge(p11, p19, BlueColor,32, false),
                },
                Number = 14
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p5, p19, YellowColor,33),
                    new Edge(p19, p11, BlueColor,32, false),
                    new Edge(p11, p10, YellowColor,23),
                    new Edge(p10, p5, RedColor,11),
                },
                Number = 15
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p17, p3, YellowColor,34, false),
                    new Edge(p3, p2, BlueColor,4),
                    new Edge(p2, p1, YellowColor,0),
                    new Edge(p1, p17, GreenColor,25),
                },
                Number = 16
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p0, p19, YellowColor,31),
                    new Edge(p19, p3, BlueColor,35, false),
                    new Edge(p3, p17, YellowColor,34, false),
                    new Edge(p17, p0, GreenColor,28),
                },
                Number = 17
            });
            polList.Add(new Polygon
            {
                Edges = new List<Edge>
                {
                    new Edge(p19, p5, YellowColor,33),
                    new Edge(p5, p4, RedColor,10),
                    new Edge(p4, p3, YellowColor,7),
                    new Edge(p3, p19, BlueColor,35, false),
                },
                Number = 18
            });


            return new Figure
            {
                Pivot = new Vertex(0, 0, 0+delta).Sum(center), 
                Polygons = polList,
                RotationVector = new Vertex
                {
                    X = 50,
                    Y = -50,
                    Z = -20+delta
                }.Sum(center)
            };
        }

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

            return new Figure
            {
                Pivot = new Vertex(0, 0, 0).Sum(center),
                Polygons = new List<Polygon> {pol1, pol2, pol3},
                RotationVector = new Vertex
                {
                    X = 25,
                    Y = -25,
                    Z = -10
                }.Sum(center)
            };
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
                            _d += DSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_f:
                            _d -= DSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_t:
                            _dz -= TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_g:
                            _dz += TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_p:
                            _projection = !_projection;
                            break;
                        case SDL.SDL_Keycode.SDLK_i:
                            _showInvisible = !_showInvisible;
                            break;
                        case SDL.SDL_Keycode.SDLK_n:
                            if(!_showNormals)
                                _showNormals = true;
                            else if (!_showNormalsInside)
                                _showNormalsInside = true;
                            else
                            {
                                _showNormals = false;
                                _showNormalsInside = false;
                            }
                            break;
                        case SDL.SDL_Keycode.SDLK_KP_2:
                            _dy-= TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_KP_4:
                            _dx+= TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_KP_6:
                            _dx-= TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_KP_8:
                            _dy+= TransitionSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_UP:
                            Teta += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_DOWN:
                            Teta -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_RIGHT:
                            Fi += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_LEFT:
                            Fi -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_c:
                            _differentColor = !_differentColor;
                            break;
                        case SDL.SDL_Keycode.SDLK_m:
                            _showAxis = !_showAxis;
                            break;
                        case SDL.SDL_Keycode.SDLK_y:
                            _vectorAngle += RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_h:
                            _vectorAngle -= RotatingSpeed;
                            break;
                        case SDL.SDL_Keycode.SDLK_v:
                            _showVector = !_showVector;
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
            if (!(_d > 0 && _projection))
                DrawFigures(renderer);
            if(_showAxis)
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
                .RotateByAngleAroundVector(_vectorAngle)
                .RotateByAngleAndAxis(_xAngle, Axis.X)
                .RotateByAngleAndAxis(_yAngle, Axis.Y)
                .RotateByAngleAndAxis(_zAngle, Axis.Z)
            ;
            var edges = fig.Edges;
            
            edges.Sort((edge, edge1) =>
            {
                if (edge.End.Z > edge1.End.Z)
                    return -1;
                return Math.Abs(edge.End.Z - edge1.End.Z) < E ? 0 : 1;
            });

            foreach (var edge in edges)
            {
                SDL.SDL_SetRenderDrawColor(renderer, edge.Color.red, edge.Color.green, edge.Color.blue, edge.Color.alpha);
                SDL.SDL_RenderDrawLine(renderer, (int)edge.Start.X, (int)edge.Start.Y, (int)edge.End.X, (int)edge.End.Y);
            }

            if (_showVector)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL.SDL_RenderDrawLine(renderer, (int)fig.Pivot.X, (int)fig.Pivot.Y, (int)fig.RotationVector.X,
                    (int)fig.RotationVector.Y);
            }
        }

        private static void DrawFigures(IntPtr renderer)
        {
            DrawedSet.Clear();
            var fig = _figure
                .RotateByAngleAroundVector(_vectorAngle)
                .RotateByAngleAndAxis(_xAngle, Axis.X)
                .RotateByAngleAndAxis(_yAngle, Axis.Y)
                .RotateByAngleAndAxis(_zAngle, Axis.Z)
                .MoveByAxis(_dx, Axis.X)
                .MoveByAxis(_dy, Axis.Y)
                .MoveByAxis(_dz, Axis.Z)
                ;

            if (_projection)
            {
                fig = fig
                    .RotateByAngleAndAxisAroundPoint(Teta, Axis.X, new Vertex(0, 0, 0))
                    .RotateByAngleAndAxisAroundPoint(Fi, Axis.Y, new Vertex(0, 0, 0))
                    .PerspectiveProjection(_d, FindCenter());
            }

            var facePolys = fig.Polygons
                .Where(polygon => polygon.IsVisible)
                .Where(polygon => ObserverVector.ScalarMultiply(polygon.NormVector()) > 0)
                .ToList();

            Draw(renderer, facePolys, true);

            if (_showInvisible)
            {
                var invizPolys = fig.Polygons
                    .Where(polygon => polygon.IsVisible)
                    .Where(polygon => ObserverVector.ScalarMultiply(polygon.NormVector()) < 0)
                    .ToList();

                Draw(renderer, invizPolys, false);
            }

            if (_showVector)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                SDL.SDL_RenderDrawLine(renderer, (int)fig.Pivot.X, (int)fig.Pivot.Y, (int)fig.RotationVector.X,
                    (int)fig.RotationVector.Y);
            }
        }

        private static void Draw(IntPtr renderer, List<Polygon> polys, bool isVisible)
        {
            polys.Sort(Comparison);
            foreach (var polygon in polys)
            {
                foreach (var edge in polygon.Edges.Where(edge => edge.IsVisible))
                {
                    if(DrawedSet.Contains(edge.Number)) continue;
                    if(_differentColor)
                        SDL.SDL_SetRenderDrawColor(renderer, edge.Color.red, edge.Color.green, edge.Color.blue, edge.Color.alpha);
                    else
                        SDL.SDL_SetRenderDrawColor(renderer, edge.MainColor.red, edge.MainColor.green, edge.MainColor.blue, edge.MainColor.alpha);
                    if (isVisible)
                    {
                        var edgeCenterZ = edge.Center.Z;
                        var pol = polys
                            .Where(polygon1 => (polygon1.Center.Z > edgeCenterZ))
                            .Where(polygon1 => !polygon1.Edges.Contains(edge))
                            .ToList();
                        var pol1 = new List<Polygon>();
                        foreach (var polygon1 in pol)
                        {
                            foreach (var polygon1Edge in polygon1.Edges)
                            {
                                var inter = IsLinesIntercept(edge.Start, edge.End, polygon1Edge.Start, polygon1Edge.End);
                                if (!inter) continue;
                                pol1.Add(polygon1);
                                break;
                            }
                        }

                        var added = false;

                        List<Edge> visible = new List<Edge>();
                        List<Edge> invisible = new List<Edge>();
                        

                        var po = polys
                            .SelectMany(polygon1 => polygon1.Vertexes
                                .Where(vertex =>
                                       !vertex.Equals(edge.Start)
                                    && !vertex.Equals(edge.End))
                            ).ToList();
                        if (pol1.Count == 0)
                        {
                            var left = 0;
                            foreach (var vertex in po)
                            {
                                var res = PointOnLeftOrRightSide(edge.Start, edge.End, vertex);
                                if (res < 0) left++;
                            }

                            if (left == 0)
                            {
                                visible.Add(edge);
                                added = true;
                            }
                        }

                        if (pol1.Count == 0 && !added)
                        {
                            foreach (var polygon1 in pol)
                            {
                                bool add = true;
                                foreach (var polygon1Edge in polygon1.Edges)
                                {
                                    if (PointOnLeftOrRightSide(polygon1Edge.Start, polygon1Edge.End, edge.Start) <= 0 &&
                                        PointOnLeftOrRightSide(polygon1Edge.Start, polygon1Edge.End, edge.End) <= 0)
                                    {
                                        add = false;
                                    }
                                }

                                if (add)
                                {
                                    invisible.Add(edge);
                                    added = true;
                                }
                            }
                        }

                        if (!added)
                        {
                            foreach (var p in pol1)
                            {
                                if (visible.Count == 0)
                                    FindLineVisiblePoints(invisible, visible, p.DistinctVertexes, edge);
                                else
                                {
                                    var vis = new List<Edge>(visible);
                                    visible.Clear();
                                    foreach (var edge1 in vis)
                                    {
                                        FindLineVisiblePoints(invisible, visible, p.DistinctVertexes, edge1);
                                    }
                                }
                            }
                        }

                        var v = visible
                            .Distinct(new EdgeEqualityComparer())
                            .Where(edge1 => Math.Abs(edge1.Start.X - edge1.End.X) > E
                                            || Math.Abs(edge1.Start.Y - edge1.End.Y) > E)
                            .ToList();
                        var inv = invisible
                            .Distinct(new EdgeEqualityComparer())
                            .Where(edge1 => Math.Abs(edge1.Start.X - edge1.End.X) > E 
                                            || Math.Abs(edge1.Start.Y - edge1.End.Y) > E)
                            .ToList();

                        var comp = new EdgeEqualityComparer();

                        foreach (var vis in v)
                        {
                            var isDraw = true;
                            foreach (var edge1 in inv)
                            {
                                if (comp.Equals(vis, edge1))
                                {
                                    isDraw = false;
                                }
                            }
                            if(isDraw)
                                DrawVisible(renderer, vis.Start, vis.End);
                        }
                        if (_showInvisible)
                            foreach (var invis in inv)
                            {
                                DrawDashLines(renderer, invis.Start, invis.End);
                            }

                        if (!added && (pol1.Count == 0))
                            DrawVisible(renderer, edge.Start, edge.End);
                    }
                    else
                        DrawDashLines(renderer, edge.Start, edge.End);
                    DrawedSet.Add(edge.Number);
                }

                if (_showNormals)
                {
                    var norm = polygon.NormVector(!_showNormalsInside).NormalizeVector().MultiplyByScalar(100);
                    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                    SDL.SDL_RenderDrawLine(renderer,
                        (int) (norm.X + polygon.Center.X), 
                        (int) (norm.Y + polygon.Center.Y),
                        (int) polygon.Center.X, 
                        (int) polygon.Center.Y
                    );
                }
            }
        }

        private static int Comparison(Polygon polygon, Polygon polygon1)
        {
            if (polygon.Center.Z < polygon1.Center.Z)
                return -1;
            return Math.Abs(polygon.Center.Z - polygon1.Center.Z) < E ? 0 : 1;
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

            var p1 = new Vertex
            {
                X = startPoint.X,
                Y = startPoint.Y
            };

            var p2 = new Vertex
            {
                X = startPoint.X + dt*(endPoint.X - startPoint.X),
                Y = startPoint.Y + dt*(endPoint.Y - startPoint.Y)
            };
            
            for (var j = 0; j < dashCount; j += 2)
            {
                SDL.SDL_RenderDrawLine(renderer, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
                p1 = new Vertex
                {
                    X = p1.X + 2 * dt * (endPoint.X - startPoint.X),
                    Y = p1.Y + 2 * dt * (endPoint.Y - startPoint.Y)
                };
                p2 = new Vertex
                {
                    X = p2.X + 2 * dt * (endPoint.X - startPoint.X),
                    Y = p2.Y + 2 * dt * (endPoint.Y - startPoint.Y)
                };
            }
        }

        #endregion

        #region Additional functions

        private static void FindLineVisiblePoints(List<Edge> visibleLines, List<Edge> notVisibleLines,
                    List<Vertex> viewWindowPoints, Edge line)
        {
            //This is line that potentially intercepts with view window
            //This vector shows direction of the vector.
            var vectorD = new Vertex { X = line.End.X - line.Start.X, Y = line.End.Y - line.Start.Y };
            var exitingWindowParameter = 0.0;
            var enteringWindowParameter = 1.0;
            var isVisible = false;
            viewWindowPoints.Add(viewWindowPoints[0]);

            int currentPoint = 0;

            for (int j = 0; j < viewWindowPoints.Count-1; j++)
            {
                currentPoint = j;
                var normVector = FindVectorNorm(viewWindowPoints[j], viewWindowPoints[j + 1]);
                var vectorW = new Vertex
                {
                    X = line.Start.X - viewWindowPoints[j].X,
                    Y = line.Start.Y - viewWindowPoints[j].Y
                };

                var vectorDScalar = ScalarComposition(vectorD, normVector);
                var vectorWScalar = ScalarComposition(vectorW, normVector);

                if (vectorDScalar == 0)
                {
                    if (vectorWScalar <= 0)
                    {
                        isVisible = true;
                        break;
                    }
                }
                else
                {
                    var t = -vectorWScalar / (double)vectorDScalar;

                    if (vectorDScalar > 0)
                    {
                        if (t > 1)
                        {
                            isVisible = true;
                            break;
                        }
                        exitingWindowParameter = Math.Max(exitingWindowParameter, t);
                    }
                    else if (vectorDScalar < 0)
                    {
                        if (t < 0)
                        {
                            isVisible = true;
                            break;
                        }
                        enteringWindowParameter = Math.Min(enteringWindowParameter, t);
                    }
                }
            }

            var exitPoint = FromParametric(line, exitingWindowParameter);
            var enterPoint = FromParametric(line, enteringWindowParameter);

            var t1 = enterPoint.IsOnLine(viewWindowPoints[currentPoint + 1], viewWindowPoints[currentPoint]);
            var t2 = exitPoint.IsOnLine(viewWindowPoints[currentPoint + 1], viewWindowPoints[currentPoint]);

            if ((!t1 || !t2) && isVisible)
            {
                notVisibleLines.Add(line);
                return;
            }

            if (exitingWindowParameter <= enteringWindowParameter)
                AddLinesToLists(line, visibleLines, notVisibleLines,
                    enteringWindowParameter, exitingWindowParameter, isVisible);
            else
            {
                notVisibleLines.Add(line);
            }
        }

        private static void AddLinesToLists(Edge line,
            List<Edge> visibleLines, List<Edge> notVisibleLines,
            double enteringWindowParameter, double exitingWindowParameter, bool isVisible)
        {


            if (Math.Abs(enteringWindowParameter - 1) < E && Math.Abs(exitingWindowParameter) < E && !isVisible)
            {
                visibleLines.Add(line);
                return;
            }

            if (Math.Abs(enteringWindowParameter - 1) < E && Math.Abs(exitingWindowParameter) < E && isVisible)
            {
                notVisibleLines.Add(line);
                return;
            }

            var enterPoint = FromParametric(line, enteringWindowParameter);
            var exitPoint = FromParametric(line, exitingWindowParameter);

            if (Math.Abs(enteringWindowParameter - 1) > E && Math.Abs(exitingWindowParameter) > E)
            {
                notVisibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = line.Start,
                    End = exitPoint
                });
                notVisibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = enterPoint,
                    End = line.End
                });

                visibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = enterPoint,
                    End = exitPoint
                });
                return;
            }

            if (Math.Abs(exitingWindowParameter) > E)
            {
                visibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = line.End,
                    End = exitPoint
                });

                notVisibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = exitPoint,
                    End = line.Start
                });

                return;
            }

            if (Math.Abs(enteringWindowParameter - 1) > E)
            {
                notVisibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = line.End,
                    End = enterPoint
                });

                visibleLines.Add(new Edge
                {
                    IsVisible = line.IsVisible,
                    Number = line.Number,
                    Color = line.Color,
                    Start = enterPoint,
                    End = line.Start
                });
            }

        }

        private static Vertex FromParametric(Edge line, double param)
        {
            var result = new Vertex
            {
                X = line.Start.X + (line.End.X - line.Start.X)*param,
                Y = line.Start.Y + (line.End.Y - line.Start.Y)*param
            };
            return result;
        }

        private static int ScalarComposition(Vertex vectorPointA, Vertex vectorPointB)
        {
            return (int)(vectorPointA.X * vectorPointB.X + vectorPointA.Y * vectorPointB.Y);
        }

        private static Vertex FindVectorNorm(Vertex pointA, Vertex pointB)
        {
            var mainVectorCD = new Vertex { X = pointB.X - pointA.X, Y = pointB.Y - pointA.Y };
            return new Vertex { X = -mainVectorCD.Y, Y = mainVectorCD.X };
        }

        private static int PointOnLeftOrRightSide(Vertex pointA,
            Vertex pointB, Vertex pointC)
        {
            var ABVectorXCord = pointB.X - pointA.X;
            var ABVectorYCord = pointB.Y - pointA.Y;
            var BCVectorXCord = pointC.X - pointB.X;
            var BCVectorYCord = pointC.Y - pointB.Y;

            return (int)((ABVectorXCord * BCVectorYCord) - (ABVectorYCord * BCVectorXCord));
        }

        private static bool IsLinesIntercept(Vertex A, Vertex B,
            Vertex C, Vertex D)
        {
            return (double)PointOnLeftOrRightSide(A, B, C) *
                   PointOnLeftOrRightSide(A, B, D) <= 0
                   &&
                   (double)PointOnLeftOrRightSide(C, D, A) *
                   PointOnLeftOrRightSide(C, D, B) < 0;
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
