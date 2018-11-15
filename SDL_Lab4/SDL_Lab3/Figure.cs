using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

        public List<Vertex> Vertexes => Edges.SelectMany(edge => new List<Vertex> {edge.Start, edge.End}).ToList();

        public List<Edge> Edges => Polygons.SelectMany(polygon => polygon.Edges).ToList();

        public List<Polygon> Polygons { get; set; }

        public Vertex Pivot { get; set; }

        public Vertex RotationVector { get; set; }

        public Figure RotateByAngleAndAxis(double angle, Axis axis)
        {
            return RotateByAngleAndAxisAroundPoint(angle, axis, Pivot);
        }

        public Figure RotateByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            return new Figure
            {
                Pivot = Pivot.RotateByAngleAndAxisAroundPoint(angle, axis, point),
                Polygons = Polygons
                    .Select(polygon => polygon.RotatePolygonByAngleAndAxisAroundPoint(angle, axis, point))
                    .ToList(),
                RotationVector = RotationVector.RotateByAngleAndAxisAroundPoint(angle, axis, point)
            };
        }

        public Figure RotateByAngleAroundVector(double angle)
        {
            return new Figure
            {
                Pivot = Pivot,
                Polygons = Polygons
                    .Select(polygon => polygon.RotatePolygonByAngleAroundVector(angle, RotationVector.Sum(Pivot.Negotiate()), Pivot))
                    .ToList(),
                RotationVector = RotationVector
            };
        }

        public Figure MoveByAxis(double delta, Axis axis)
        {
            return new Figure
            {
                Pivot = Pivot.MoveByAxis(delta, axis),
                Polygons = Polygons
                    .Select(polygon => polygon.MoveByAxis(delta, axis))
                    .ToList(),
                RotationVector = RotationVector.MoveByAxis(delta, axis)
            };
        }

        public Figure PerspectiveProjection(double distanse, Vertex point)
        {
            return new Figure
            {
                Pivot = Pivot.PerspectiveProjection(distanse, point),
                Polygons = Polygons
                    .Select(polygon => polygon.PerspectiveProjection(distanse, point))
                    .ToList(),
                RotationVector = RotationVector.PerspectiveProjection(distanse, point)
            };
        }
    }
}