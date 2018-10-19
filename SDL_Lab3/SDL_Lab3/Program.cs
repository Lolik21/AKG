using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private const int CircleRadius = 150;
        private const int CountOfCircleQuarters = 100;

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

        public static List<SDL.SDL_Point> PressedFigure { get; private set; }

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
                        MapMousePoint(point.x, point.y, ref point);
                        PressedFigure = FindFigureOnMousePoint(point);
                        PressedPoint = point;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    if (sdlEvent.button.button == SDL.SDL_BUTTON_LEFT)
                    {
                        PressedFigure = null;
                    }
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    if (PressedFigure != null)
                    {
                        var currentPoint = new SDL.SDL_Point
                        {
                            x = sdlEvent.button.x,
                            y = sdlEvent.button.y
                        };
                        MapMousePoint(currentPoint.x, currentPoint.y, ref currentPoint);
                        ApplyShift(PressedFigure, currentPoint, PressedPoint);
                    }
                    break;
            }

            return false;
        }

        public static SDL.SDL_Point PressedPoint { get; set; }

        private static void ApplyShift(List<SDL.SDL_Point> points,
            SDL.SDL_Point currentPoint, SDL.SDL_Point pressedPoint)
        {
            var dx = pressedPoint.x - currentPoint.x;
            var dy = pressedPoint.y - currentPoint.y;
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new SDL.SDL_Point { x = points[i].x - dx, y = points[i].y - dy };
            }

            PressedPoint = currentPoint;
        }

        private static List<SDL.SDL_Point> FindFigureOnMousePoint(SDL.SDL_Point mousePoint)
        {
            if (IsInFigure(_rectangle, mousePoint))
            {
                return _rectangle;
            }

            if (IsInFigure(_circle, mousePoint))
            {
                return _circle;
            }

            if (IsInFigure(_mainWindow, mousePoint))
            {
                return _mainWindow;
            }

            return null;
        }

        private static bool IsInFigure(List<SDL.SDL_Point> figure, SDL.SDL_Point mousePoint)
        {
            var length = figure.Count;
            if (PointOnLeftOrRightSide(figure[0], figure[1], mousePoint) < 0 ||
                PointOnLeftOrRightSide(figure[0], figure[length - 1], mousePoint) > 0)
                return false;
            var p = 1;
            var r = length - 1;
            while (r - p > 1)
            {
                var q = (p + r) / 2;
                if (PointOnLeftOrRightSide(figure[0], figure[q], mousePoint) < 0)
                {
                    r = q;
                }
                else
                {
                    p = q;
                }
            }

            return !IsLinesIntercept(figure[0], mousePoint, figure[p], figure[r]);
        }

        #region FindFigureHelpers
        /// <summary>
        /// https://habr.com/post/144571/ 
        /// </summary>
        private static int PointOnLeftOrRightSide(SDL.SDL_Point pointA,
            SDL.SDL_Point pointB, SDL.SDL_Point pointC)
        {
            // we use formula (Ax*By - Ay*Bx)Z according to
            // Z(0,0,1) - is z coordinate (x,y,z). By the value of z
            // simply if Z<0 it is right rotation from AB to BC vector
            // Z>=0 it is left rotation from AB to BC vector
            // Using this approach helps to determine case when Line1 crossing Line2 or not

            // First we need to initialize vectors AB and BC coordinates
            // For AB : vectorCoord.xCoord = b.xCoord - a.xCoord 

            var ABVectorXCord = pointB.x - pointA.x;
            var ABVectorYCord = pointB.y - pointA.y;
            var BCVectorXCord = pointC.x - pointB.x;
            var BCVectorYCord = pointC.y - pointB.y;

            //Second - use formula)
            return (ABVectorXCord * BCVectorYCord) - (ABVectorYCord * BCVectorXCord);
        }

        /// <summary>
        /// This method will checks whatever AB line intercept CD line
        /// </summary>
        private static bool IsLinesIntercept(SDL.SDL_Point A, SDL.SDL_Point B,
                                             SDL.SDL_Point C, SDL.SDL_Point D)
        {
            return (double)PointOnLeftOrRightSide(A, B, C) *
                   PointOnLeftOrRightSide(A, B, D) <= 0
                   &&
                   (double)PointOnLeftOrRightSide(C, D, A) *
                   PointOnLeftOrRightSide(C, D, B) < 0;
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
            //DrawRectangle(renderer, _mainWindow);
            DrawRectangle(renderer, _rectangle, _mainWindow);
            DrawCirclePoints(renderer, _circle);
        }

        private static void InitFigures(out List<SDL.SDL_Point> circle,
            out List<SDL.SDL_Point> mainWindow,
            out List<SDL.SDL_Point> rectangle)
        {
            //Init Circle 
            circle = new List<SDL.SDL_Point>();
            circle = GetCircleCurvePoints();

            //Init main Rectangle
            var size = WindowWidth / 3;
            mainWindow = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point{ x = size, y = size - 100},
                new SDL.SDL_Point{ x = -size, y = size - 100},
                new SDL.SDL_Point{ x = -size, y = -size + 100},
                new SDL.SDL_Point{ x = size, y = -size + 100},
            };

            //Init Rectangle
            rectangle = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point{ x = 500, y = 75 },
                new SDL.SDL_Point{ x = -500, y = 75},
                new SDL.SDL_Point{ x = -500, y = -75 },
                new SDL.SDL_Point{ x = 500, y = -75 }
            };
        }

        private static void DrawCirclePoints(IntPtr renderer, List<SDL.SDL_Point> circle)
        {
            var points = new List<SDL.SDL_Point>(circle);
            points.Add(points.FirstOrDefault());
            SDL.SDL_RenderDrawLines(renderer, MapListOfPoints(points).ToArray(), points.Count);
        }



        private static List<SDL.SDL_Point> GetCircleCurvePoints()
        {
            var points = new List<SDL.SDL_Point>();
            for (var i = 0; i < CountOfCircleQuarters; i++)
            {
                var xi = (int)(CircleRadius * Math.Cos(-Math.PI / 2 + 2 * Math.PI * i / CountOfCircleQuarters));
                var yi = (int)(CircleRadius * Math.Sin(-Math.PI / 2 + 2 * Math.PI * i / CountOfCircleQuarters));
                points.Add(new SDL.SDL_Point { x = xi, y = yi });
            }
            return points;
        }

        private static void DrawRectangle(IntPtr renderer, List<SDL.SDL_Point> rectangle, List<SDL.SDL_Point> viewWindow)
        {
            var points = new List<SDL.SDL_Point>(rectangle);
            var viewWindowPoints = new List<SDL.SDL_Point>(viewWindow);

            //Here lines will be like line[0] --- line[1] where line[0] is point
            var visibleLines = new List<SDL.SDL_Point>();
            var notVisibleLines = new List<SDL.SDL_Point>();

            //Add last points
            points.Add(points.FirstOrDefault());
            viewWindowPoints.Add(viewWindowPoints.FirstOrDefault());

            for (int i = 0; i < points.Count - 1; i++)
            {
                FindLineVisiblePoints(visibleLines, notVisibleLines, viewWindowPoints, points[i], points[i + 1]);
            }

            SDL.SDL_RenderDrawLines(renderer, points.ToArray(), points.Count);
        }

        private static void FindLineVisiblePoints(List<SDL.SDL_Point> visibleLines, List<SDL.SDL_Point> notVisibleLines,
            List<SDL.SDL_Point> viewWindowPoints, SDL.SDL_Point pointA, SDL.SDL_Point pointB)
        {
            //This is line that potentially intercepts with view window
            var vectorAB = new SDL.SDL_Point { x = pointB.x - pointA.x, y = pointB.y - pointA.y };
            var enteringWindowParameter = 0.0;
            var exitingWindowParameter = 1.0;

            for (int j = 0; j < viewWindowPoints.Count - 1; j++)
            {

                var normVector = FindVectorNorm(viewWindowPoints[j], viewWindowPoints[j + 1]);
                var Fi = viewWindowPoints[j];
                var tempQiVector = new SDL.SDL_Point { x = pointA.x - Fi.x, y = pointA.y - Fi.y };
                // (M > 0 then -|>),  (M < 0 then <|-), (M == 0 then ||)
                var Mi = normVector.x * vectorAB.x + normVector.y * vectorAB.y;
                var Qi = normVector.x * tempQiVector.x + normVector.y * tempQiVector.y;

                if (Mi == 0)
                {
                    if (Qi < 0)
                    {
                        continue;
                    }                
                }
                MiIsGreater(Mi, Qi, ref enteringWindowParameter, ref exitingWindowParameter);
            }
        }

        private static void MiIsGreater(int Mi, int Qi, ref double enteringWindowParameter, ref double exitingWindowParameter)
        {
            var t = (-1) * Qi / (double)Mi;
            if (Mi > 0)
            {
                if (t > 1)
                {
                    return;
                }
                else
                {
                    enteringWindowParameter = Math.Max(enteringWindowParameter, t);
                }
            }
            else
            {
                if (t < 0)
                {
                    return;
                }
                else
                {
                    exitingWindowParameter = Math.Min(exitingWindowParameter, t);
                }
            }
        }

        private static SDL.SDL_Point GetPointFromTParameter(SDL.SDL_Point pointA, SDL.SDL_Point pointB)
        {
            return new SDL.SDL_Point();
        }


        private static SDL.SDL_Point FindVectorNorm(SDL.SDL_Point pointA, SDL.SDL_Point pointB)
        {
            var mainVectorCD = new SDL.SDL_Point { x = pointB.x - pointA.x, y = pointB.y - pointA.y };
            return new SDL.SDL_Point { x = -mainVectorCD.y, y = mainVectorCD.x };
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

        private static void MapMousePoint(int x, int y, ref SDL.SDL_Point mousePoint)
        {
            FindCenter(out var centralPoint);
            mousePoint.x = x - centralPoint.x;
            mousePoint.y = centralPoint.y - y;
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
