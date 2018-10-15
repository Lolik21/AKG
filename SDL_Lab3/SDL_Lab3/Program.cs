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
        private class Circle
        {
            public int Radius { get; set; }
            public SDL.SDL_Point Point { get; set; }
        }

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
        private static Circle _circle;
       

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
                InitFigures(out _circle,out _mainWindow,out _rectangle);

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
                    
                    break;
            }

            return false;
        }

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

        private static void InitFigures(out Circle circle, out List<SDL.SDL_Point> mainWindow,
            out List<SDL.SDL_Point> rectangle)
        {
            //Init Circle 
            circle = new Circle { Radius = 150, Point = new SDL.SDL_Point{ x = 0, y = 0 }};

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

        private static void DrawCircle(IntPtr renderer, Circle circle)
        {
            var x = circle.Radius - 1;
            var y = 0;
            var dx = 1;
            var dy = 1;
            var error = dx - circle.Radius * 2;

            var points = new List<SDL.SDL_Point>();

            while (x >= y)
            {
                //Upper right
                points.Add(new SDL.SDL_Point { x = circle.Point.x + x, y = circle.Point.y + y });
                points.Add(new SDL.SDL_Point { x = circle.Point.x + y, y = circle.Point.y + x });
                //Upper left
                points.Add(new SDL.SDL_Point { x = circle.Point.x - x, y = circle.Point.y + y });
                points.Add(new SDL.SDL_Point { x = circle.Point.x - y, y = circle.Point.y + x });
                //Down left
                points.Add(new SDL.SDL_Point { x = circle.Point.x - x, y = circle.Point.y - y });
                points.Add(new SDL.SDL_Point { x = circle.Point.x - y, y = circle.Point.y - x });
                //Down right
                points.Add(new SDL.SDL_Point { x = circle.Point.x + x, y = circle.Point.y - y });
                points.Add(new SDL.SDL_Point { x = circle.Point.x + y, y = circle.Point.y - x });

                if (error <= 0)
                {
                    y = y + 1;
                    error = error + dy;
                    dy = dy + 2;
                }

                if (error > 0)
                {
                    x = x - 1;
                    dx = dx + 2;
                    error = error + dx - circle.Radius * 2;
                }

            }

            
            SDL.SDL_RenderDrawPoints(renderer, MapListOfPoints(points).ToArray(), points.Count);
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
            newPoint.y = centralPoint.y - (int)(ty*_scale) + _transY;
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
