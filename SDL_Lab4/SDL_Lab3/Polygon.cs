using System.Collections.Generic;
using System.Linq;

namespace SDL_Lab1
{
    public class Polygon
    {
        public List<Edge> Edges { get; set; }
        public bool IsVisible { get; set; } = true;

        public Polygon RotateEdgeByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            return new Polygon
            {
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.RotateEdgeByAngleAndAxisAroundPoint(angle, axis, point))
                    .ToList()
            };
        }

        public Polygon PerspectiveProjection(double distanse, Vertex pivot)
        {
            return new Polygon
            {
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.PerspectiveProjection(distanse, pivot))
                    .ToList()
            };
        }

        public Vertex NormVector(bool external = true)
        {
            var b = Edges[0].End.Sum(Edges[0].Start.Negotiate());
            var a = Edges[1].Start.Sum(Edges[1].End.Negotiate());
            if (!external)
            {
                var c = b;
                b = a;
                a = c;
            }
            return new Vertex(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }
    }
}