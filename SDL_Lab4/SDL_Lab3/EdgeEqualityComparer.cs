using System;
using System.Collections.Generic;

namespace SDL_Lab1
{
    public class EdgeEqualityComparer : IEqualityComparer<Edge>
    {
        private const double E = 0.0001;

        public bool Equals(Edge x, Edge y)
        {
            if (Math.Abs(x.Start.X - y.Start.X) < E 
                && Math.Abs(x.Start.Y - y.Start.Y) < E 
                && Math.Abs(x.Start.Z - y.Start.Z) < E
                && Math.Abs(x.End.X - y.End.X) < E
                && Math.Abs(x.End.Y - y.End.Y) < E
                && Math.Abs(x.End.Z - y.End.Z) < E
                )
            {
                return true;
            }

            var dy = new Edge
            {
                Start = y.End,
                End = y.Start
            };

            return Math.Abs(x.Start.X - dy.Start.X) < E
                   && Math.Abs(x.Start.Y - dy.Start.Y) < E
                   && Math.Abs(x.Start.Z - dy.Start.Z) < E
                   && Math.Abs(x.End.X - dy.End.X) < E
                   && Math.Abs(x.End.Y - dy.End.Y) < E
                   && Math.Abs(x.End.Z - dy.End.Z) < E;
        }

        public int GetHashCode(Edge obj)
        {
            return obj.End.GetHashCode() + obj.Start.GetHashCode() + obj.Color.GetHashCode();
        }
    }
}