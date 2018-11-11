using System;
using System.Collections.Generic;
using System.Linq;
using SDL2;

namespace SDL_Lab1
{
    public class Figure
    {
        public Figure(){}

        public Figure(Vertex pivot, List<Polygon> polygons)
        {
            Pivot = pivot ?? throw new ArgumentNullException(nameof(pivot));
            Polygons = polygons ?? throw new ArgumentNullException(nameof(polygons));
        }

        //public Dictionary<Vertex, List<Vertex>> Links { get; set; }

        public List<Vertex> Vertexes => Edges.SelectMany(edge => new List<Vertex> {edge.Start, edge.End}).ToList();

        public List<Edge> Edges => Polygons.SelectMany(polygon => polygon.Edges).ToList();

        public List<Polygon> Polygons { get; set; }

        public Vertex Pivot { get; private set; }

        public Figure RotateByAngleAndAxis(double angle, Axis axis)
        {
            return new Figure
            {
                Pivot = Pivot,
                Polygons = Polygons
                    .Select(polygon => polygon.RotateEdgeByAngleAndAxisAroundPoint(angle, axis, Pivot))
                    .ToList()
            };
        }
    }
}