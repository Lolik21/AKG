using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SDL2;

namespace SDL_Lab1
{
    static class Program
    {
        private const int WindowHeight = 720;
        private const int WindowWidth = 1280;
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
            }

            return false;
        }

        private static void DrawGreatLine(IntPtr screenIntPtr)
        {
            var renderer = _renderer;
            SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            SDL.SDL_RenderClear(renderer);
            DrawAxis(renderer);
            DrawRectangle(renderer);
        }

        private static void DrawRectangle(IntPtr renderer)
        {
            var u = 0.1;
            var countOfRectangles = 10;
            List<SDL.SDL_Point> points = null;
            InitStartRectanglePoints(out points);
            var currentRectanglePoints = points;
            currentRectanglePoints.Add(currentRectanglePoints.FirstOrDefault());
            currentRectanglePoints = MapListOfPoints(currentRectanglePoints);
            SDL.SDL_SetRenderDrawColor(renderer, 255, 34, 45, 255);
            SDL.SDL_RenderDrawLines(renderer, currentRectanglePoints.ToArray(), points.Count);

            for (var i = 0; i < countOfRectangles; i++)
            {
                currentRectanglePoints = CalculateNewRectanglePoints(currentRectanglePoints, u);
                currentRectanglePoints.Add(currentRectanglePoints.FirstOrDefault());
                SDL.SDL_RenderDrawLines(renderer, currentRectanglePoints.ToArray(), currentRectanglePoints.Count);
            }
            SDL.SDL_RenderPresent(renderer);
        }

        private static List<SDL.SDL_Point> CalculateNewRectanglePoints(List<SDL.SDL_Point> currentRectanglePoints, double u)
        {
            var result = new List<SDL.SDL_Point>();
            for (var j = 0; j < currentRectanglePoints.Count - 1; j++)
            {
                var x = currentRectanglePoints[j].x;
                var xPlus1 = currentRectanglePoints[j + 1].x;
                var y = currentRectanglePoints[j].y;
                var yPlus1 = currentRectanglePoints[j + 1].y;

                var resultX = (1 - u) * x + u * xPlus1;
                var resultY = (1 - u) * y + u * yPlus1;

                var point = new SDL.SDL_Point {x = (int) resultX, y = (int) resultY};

                result.Add(point);
            }

            return result;
        }

        private static void InitStartRectanglePoints(out List<SDL.SDL_Point> points)
        {
            points = new List<SDL.SDL_Point>
            {
                new SDL.SDL_Point{ x = -300, y = -300},
                new SDL.SDL_Point{x = 300, y = -300},
                new SDL.SDL_Point{x = 300, y = 300},
                new SDL.SDL_Point{x = -300, y = 300}
            };
        }

        private static void DrawAxis(IntPtr renderer)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 255, 34, 255, 255);
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            SDL.SDL_RenderDrawLine(renderer, 0, height / 2, width, height / 2);
            SDL.SDL_RenderDrawLine(renderer, width / 2, 0, width / 2, height);
        }

        private static void FindCenter(ref SDL.SDL_Point centerPoint)
        {
            SDL.SDL_GetWindowSize(_window, out var width, out var height);
            centerPoint.x = width / 2;
            centerPoint.y = height / 2;
        }


        private static void MapToPoint(int x, int y, ref SDL.SDL_Point newPoint)
        {
            var centralPoint = new SDL.SDL_Point();
            FindCenter(ref centralPoint);
            newPoint.x = x + centralPoint.x;
            newPoint.y = y + centralPoint.y;
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
