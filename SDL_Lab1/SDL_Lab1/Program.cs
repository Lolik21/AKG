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
            DrawCurve(renderer);
            SDL.SDL_RenderPresent(renderer);
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
            centerPoint.x = width / 2;
            centerPoint.y = height / 2;
        }

        private static void DrawCurve(IntPtr renderer)
        {
            List<SDL.SDL_Point> allPointsToDraw = new List<SDL.SDL_Point>();
            FindCenter(out var centerPoint);
            allPointsToDraw.Add(centerPoint);
            var N = 10000;
            var a = 300;
            double angle = 0;
            for (int i = 1; i < N; i++)
            {
                CalculateXY(angle, a,  out int x, out int y);
                MapToPoint(x, y, centerPoint, out SDL.SDL_Point newPoint);
                allPointsToDraw.Add(newPoint);
                angle = angle + (Math.PI / N);
            }
            SDL.SDL_SetRenderDrawColor(renderer, 42, 34, 255, 255);
            SDL.SDL_RenderDrawPoints(renderer, allPointsToDraw.ToArray(), allPointsToDraw.Count);
        }

        private static void MapToPoint(int x, int y, SDL.SDL_Point centralPoint , out SDL.SDL_Point newPoint)
        {
            newPoint.x = x + centralPoint.x;
            newPoint.y = y + centralPoint.y;
        }

        private static void CalculateXY(double angle, int a, out int x, out int y)
        {
            var tangents = Math.Tan(angle);
            x = (int)(a * Math.Pow(tangents, 2) / (1 + Math.Pow(tangents, 2)));
            y = (int)(a * Math.Pow(tangents, 3) / (1 + Math.Pow(tangents, 2)));
        }
    }
}
