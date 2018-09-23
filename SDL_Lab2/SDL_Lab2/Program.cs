using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;
        private const int CountOfVertex = 10;
        private const int CountOfInnerFigure = 10;
        private const int CircleRadius = 300;
        private const double DeltaAngle = Math.PI/180;
        private const int DeltaTrans = 10;
        private const int DeltaScale = 2;
        private const double InnerAngle = Math.PI/4;
        private static double _scale = 1d;
        private static double _angle = 0d;
        private static int _transX = 0;
        private static int _transY = 0;
        private static IntPtr _window;
        private static IntPtr _renderer;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
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
                var screenSurfaceStruct = Marshal.PtrToStructure<SDL.SDL_Surface>(screenSurface);
                _renderer = SDL.SDL_CreateRenderer(window, -1, 0);

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
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    switch (sdlEvent.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_UP:
                            _angle += DeltaAngle;
                            break;
                        case SDL.SDL_Keycode.SDLK_DOWN:
                            _angle -= DeltaAngle;
                            break;
                        case SDL.SDL_Keycode.SDLK_RIGHT:
                            _scale *= DeltaScale;
                            break;
                        case SDL.SDL_Keycode.SDLK_LEFT:
                            _scale /= DeltaScale;
                            break;
                        case SDL.SDL_Keycode.SDLK_s:
                            _transY += DeltaTrans;
                            break;
                        case SDL.SDL_Keycode.SDLK_w:
                            _transY -= DeltaTrans;
                            break;
                        case SDL.SDL_Keycode.SDLK_d:
                            _transX += 10;
                            break;
                        case SDL.SDL_Keycode.SDLK_a:
                            _transX -= 10;
                            break;
                        case SDL.SDL_Keycode.SDLK_z:
                            _transX = 0;
                            _transY = 0;
                            break;
                        case SDL.SDL_Keycode.SDLK_F4:
                            return true;
                    }
                    break;
            }

            return false;
        }

        private static void DrawGreatLine(IntPtr screenIntPtr)
        {
            var renderer = _renderer;
            SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            SDL.SDL_RenderClear(renderer);
            DrawAxis(renderer);
            DrawFigure(renderer);
        }

        private static void DrawFigure(IntPtr renderer)
        {
            var currentAngle = InnerAngle/CountOfInnerFigure;
            var u = Math.Tan(currentAngle)/(1+Math.Tan(currentAngle));
            InitStartFigurePoints(CountOfVertex, out var points);
            var currentFigurePoints = points;
            currentFigurePoints.Add(currentFigurePoints.FirstOrDefault());
            currentFigurePoints = MapListOfPoints(currentFigurePoints);
            SDL.SDL_SetRenderDrawColor(renderer, 255, 34, 45, 255);
            SDL.SDL_RenderDrawLines(renderer, currentFigurePoints.ToArray(), points.Count);

            for (var i = 0; i < CountOfInnerFigure; i++)
            {
                currentFigurePoints = CalculateNewFigurePoints(currentFigurePoints, u);
                currentFigurePoints.Add(currentFigurePoints.FirstOrDefault());
                SDL.SDL_RenderDrawLines(renderer, currentFigurePoints.ToArray(), currentFigurePoints.Count);
            }
            SDL.SDL_RenderPresent(renderer);
        }

        private static List<SDL.SDL_Point> CalculateNewFigurePoints(List<SDL.SDL_Point> currentFigurePoints, double u)
        {
            var result = new List<SDL.SDL_Point>();
            for (var j = 0; j < currentFigurePoints.Count - 1; j++)
            {
                var x = currentFigurePoints[j].x;
                var xPlus1 = currentFigurePoints[j + 1].x;
                var y = currentFigurePoints[j].y;
                var yPlus1 = currentFigurePoints[j + 1].y;

                var resultX = (1 - u) * x + u * xPlus1;
                var resultY = (1 - u) * y + u * yPlus1;

                var point = new SDL.SDL_Point {x = (int) resultX, y = (int) resultY};

                result.Add(point);
            }

            return result;
        }

        private static void InitStartFigurePoints(int vertexCount, out List<SDL.SDL_Point> points)
        {
            points = new List<SDL.SDL_Point>();
            for (var i = 0; i < vertexCount; i++)
            {
                var xi = (int) (CircleRadius*Math.Cos(-Math.PI/2 + 2*Math.PI*i/vertexCount));
                var yi = (int) (CircleRadius*Math.Sin(-Math.PI/2 + 2*Math.PI*i/vertexCount));
                points.Add(new SDL.SDL_Point { x = xi, y = yi });
            }
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
                x = width/2,
                y = height/2
            };

        }

        private static void MapToPoint(int x, int y, ref SDL.SDL_Point newPoint)
        {
            FindCenter(out var centralPoint);
            var tx = (int)(x*Math.Cos(_angle) - y*Math.Sin(_angle));
            var ty = (int)(x*Math.Sin(_angle) + y*Math.Cos(_angle));
            newPoint.x = centralPoint.x + (int)(tx*_scale) + _transX;
            newPoint.y = centralPoint.y + (int)(ty*_scale) + _transY;
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
