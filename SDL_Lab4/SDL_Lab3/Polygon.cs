using System.Collections.Generic;
using System.Linq;

namespace SDL_Lab1
{
    public class Polygon
    {
        public int Number { get; set; }

        public List<Vertex> Vertexes => Edges.SelectMany(edge => new List<Vertex> { edge.Start, edge.End }).ToList();
        public List<Vertex> DistinctVertexes => Edges.Select(edge => edge.Start).ToList();
        public List<Edge> Edges { get; set; }
        public bool IsVisible { get; set; } = true;

        public Polygon RotatePolygonByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
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

        public Polygon RotatePolygonByAngleAroundVector(double angle, Vertex vector, Vertex pivot)
        {
            return new Polygon
            {
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.RotateEdgeByAngleAroundVector(angle, vector, pivot))
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

        public Polygon MoveByAxis(double delta, Axis axis)
        {
            return new Polygon
            {
                Number = Number,
                IsVisible = IsVisible,
                Edges = Edges
                    .Select(edge => edge.MoveByAxis(delta, axis))
                    .ToList()
            };
        }

        public Vertex Center => new Vertex(
            (Edges[0].Start.X + Edges[1].End.X)/2, 
            (Edges[0].Start.Y + Edges[1].End.Y)/2,
            (Edges[0].Start.Z + Edges[1].End.Z)/2
        );

        public Vertex NormVector(bool external = true)
        {
            var b = Center.Sum(Edges[0].End.Negotiate());
            var a = Center.Sum(Edges[0].Start.Negotiate());
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