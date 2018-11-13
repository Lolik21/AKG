using System.Collections.Generic;
using System.Linq;

namespace SDL_Lab1
{
    public class Polygon
    {
        public int Number { get; set; }

        public List<Vertex> Vertexes => Edges.SelectMany(edge => new List<Vertex> {edge.Start, edge.End}).ToList();
        public List<Edge> Edges { get; set; }
        public bool IsVisible { get; set; } = true;

        public Polygon RotateEdgeByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            return new Polygon
            {
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.RotateEdgeByAngleAndAxisAroundPoint(angle, axis, point))
                    .ToList(),
                Number = Number
            };
        }

        public Polygon PerspectiveProjection(double distanse, Vertex pivot)
        {
            return new Polygon
            {
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.PerspectiveProjection(distanse, pivot))
                    .ToList(),
                Number = Number
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
            return a.VectorMultiply(b);
        }

        public override string ToString()
        {
            return $"{Number}";
        }
    }
}