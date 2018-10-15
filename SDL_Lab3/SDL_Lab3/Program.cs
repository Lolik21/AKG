using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;

        private static double _scale = 1d;
        private static double _angle = 0d;
        private static int _transX = 0;
        private static int _transY = 0;
        private static IntPtr _window;
        private static IntPtr _renderer;

        private static List<SDL.SDL_Point> _mainWindow;
        private static List<SDL.SDL_Point> _rectangle;
        private static List<SDL.SDL_Point> _circle;

        public static SDL.SDL_Point PressedPoint { get; private set; }

        [STAThread]
        static void Main()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) >= 0)
            {
                var window = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED,
                    SDL.SDL_WINDOWPOS_UNDEFINED, WindowWidth, WindowHeight,
                    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                _window = window;
                var screenSurface = SDL.SDL_GetWindowSurface(window);
                _renderer = SDL.SDL_CreateRenderer(window, -1, 0);
                InitFigures(out _circle, out _mainWindow, out _rectangle);

                var exitApp = false;
                while (!exitApp)
                {
                    SDL.SDL_Event e;
                    while (SDL.SDL_PollEvent(out e) != 0)
                    {
                        exitApp = SDLWindProc(e);
                    }
                    DrawGreatLine(screenSurface);
                }
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
            }
        }

        private static bool SDLWindProc(SDL.SDL_Event sdlEvent)
        {
            switch (sdlEvent.type)
            {
                case SDL.SDL_EventType.SDL_QUIT:
                    return true;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (sdlEvent.button.button == SDL.SDL_BUTTON_LEFT)
                    {
                        var point = new SDL.SDL_Point { x = sdlEvent.button.x, y = sdlEvent.button.y };
                        MapToPoint(point.x, point.y, ref point);
                        PressedPoint = point;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    if (sdlEvent.button.button == SDL.SDL_BUTTON_LEFT
                        && sdlEvent.button.state == SDL.SDL_PRESSED)
                    {
                        var currentPoint = new SDL.SDL_Point
                        {
                            x = sdlEvent.button.x,
                            y = sdlEvent.button.y
                        };
                        MapToPoint(currentPoint.x, currentPoint.y, ref currentPoint);
                        var points = FindFigureOnMousePoint(currentPoint);
                        ApplyShift(points, currentPoint, PressedPoint);
                    }
                    break;
            }

            return false;
        }

        private static void ApplyShift(List<SDL.SDL_Point> points,
            SDL.SDL_Point currentPoint, SDL.SDL_Point pressedPoint)
        {

        }

        private static List<SDL.SDL_Point> FindFigureOnMousePoint(SDL.SDL_Point mousePoint)
        {

            return null;
        }

        #region FindFigureHelpers
        /// <summary>
        /// https://habr.com/post/144571/ 
        /// </summary>
        private static int IsPointOnLeftOrRightSide(SDL.SDL_Point pointA,
            SDL.SDL_Point pointB, SDL.SDL_Point pointC)
        {
            // we use formula (Ax*By - Ay*Bx)Z according to
            // Z(0,0,1) - is z coordinate (x,y,z). By the value of z coord (>0 or <=0)
            // simply if Z<0 it is right rotation from AB to BC vector
            // Z>=0 it is left retation from AB to BC vector
            // Using this approch helps to determine case when AB crossing BC or not

            //First we need to initialize vectors AB and BC coordinates
            // For AB : vectorCoord.xCoord = b.xCoord - a.xCoord 

            var ABVectorXCoord = pointB.x - pointA.x;
            var ABVectorYCoord = pointB.y - pointB.y;
            var BCVectorXCoord = pointC.x - pointB.x;
            var BCVectorYCoord = pointC.y - pointC.y;

            //Second - use formula)
            return (ABVectorXCoord * BCVectorYCoord) - (ABVectorYCoord * BCVectorXCoord);
        }

        private static bool IsLinesIntersept(SDL.SDL_Point line1XCoord, SDL.SDL_Point line1YCoord,
                                             SDL.SDL_Point line2XCoord, SDL.SDL_Point line2YCoord)
        {

        }
        #endregion   

        private static void DrawGreatLine(IntPtr screenIntPtr)
        {
            var renderer = _renderer;
            SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            SDL.SDL_RenderClear(renderer);
            //DrawAxis(renderer);
            DrawFigures(renderer);
            SDL.SDL_RenderPresent(renderer);
        }

        private static void DrawFigures(IntPtr renderer)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
            DrawRectangle(renderer, _mainWindow);
            DrawRectangle(renderer, _rectangle);
            DrawCircle(renderer, _circle);
        }

        private static void InitFigures(out List<SDL.SDL_Point> circle,
            out List<SDL.SDL_Point> mainWindow,
            out List<SDL.SDL_Point> rectangle)
        {
            //Init Circle 
            circle = new List<SDL.SDL_Point>();

            //Init main Rectangle
            var size = WindowWidth / 3;
            mainWindow = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point{ x = size, y = size - 100},
                new SDL.SDL_Point{ x = size, y = -size + 100},
                new SDL.SDL_Point{ x = -size, y = -size + 100},
                new SDL.SDL_Point{ x = -size, y = size - 100}
            };
            mainWindow.Add(mainWindow.FirstOrDefault());

            //Init Rectangle
            rectangle = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point{ x = 500, y = 75 },
                new SDL.SDL_Point{ x = 500, y = -75 },
                new SDL.SDL_Point{ x = -500, y = -75 },
                new SDL.SDL_Point{ x = -500, y = 75}
            };
            rectangle.Add(rectangle.FirstOrDefault());
        }

        private static void DrawCircle(IntPtr renderer, List<SDL.SDL_Point> circle)
        {
            var points = DrawCircleCurve(circle);
            points.Add(points.FirstOrDefault());
            SDL.SDL_RenderDrawLines(renderer, MapListOfPoints(points).ToArray(), points.Count);
        }

        private static List<SDL.SDL_Point> DrawCircleCurve(List<SDL.SDL_Point> circle)
        {
            const int Radius = 150;
            const int CountOfQuarters = 1000;
            var points = new List<SDL.SDL_Point>();
            for (var i = 0; i < CountOfQuarters; i++)
            {
                var xi = (int)(Radius * Math.Cos(-Math.PI / 2 + 2 * Math.PI * i / CountOfQuarters));
                var yi = (int)(Radius * Math.Sin(-Math.PI / 2 + 2 * Math.PI * i / CountOfQuarters));
                points.Add(new SDL.SDL_Point { x = xi, y = yi });
            }
            return points;
        }

        private static void DrawRectangle(IntPtr renderer, List<SDL.SDL_Point> rectangle)
        {
            var points = MapListOfPoints(rectangle);
            SDL.SDL_RenderDrawLines(renderer, points.ToArray(), points.Count);
        }

        private static void DrawAxis(IntPtr renderer)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 255, 34, 255, 255);
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            SDL.SDL_RenderDrawLine(renderer, 0, height / 2, width, height / 2);
            SDL.SDL_RenderDrawLine(renderer, width / 2, 0, width / 2, height);
        }

        private static void FindCenter(out SDL.SDL_Point centerPoint)
        {
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            centerPoint = new SDL.SDL_Point
            {
                x = width / 2,
                y = height / 2
            };

        }

        private static void MapToPoint(int x, int y, ref SDL.SDL_Point newPoint)
        {
            FindCenter(out var centralPoint);
            var tx = (int)(x * Math.Cos(_angle) - y * Math.Sin(_angle));
            var ty = (int)(x * Math.Sin(_angle) + y * Math.Cos(_angle));
            newPoint.x = centralPoint.x + (int)(tx * _scale) + _transX;
            newPoint.y = centralPoint.y - (int)(ty * _scale) + _transY;
        }

        private static List<SDL.SDL_Point> MapListOfPoints(List<SDL.SDL_Point> points)
        {
            var result = new List<SDL.SDL_Point>();
            for (int i = 0; i < points.Count; i++)
            {
                var point = new SDL.SDL_Point();
                MapToPoint(points[i].x, points[i].y, ref point);
                result.Add(point);
            }

            return result;
        }
    }
}
