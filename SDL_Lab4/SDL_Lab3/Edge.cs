using System;
using System.Collections.Generic;

namespace SDL_Lab1
{
    public class Edge
    {
        public Edge(){}

        public Edge(Vertex start, Vertex end, (byte red, byte green, byte blue, byte alpha) color, int number, bool isVisible = true)
        {
            Start = start;
            End = end;
            Color = color;
            IsVisible = isVisible;
            Number = number;
        }

        public int Number { get; set; }
        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public bool IsVisible { get; set; } = true;
        public (byte red, byte green, byte blue, byte alpha) Color { get; set; }

        public List<Vertex> Vertexes => new List<Vertex>{Start, End};

        public Edge RotateEdgeByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            return new Edge
            {
                Start = Start.RotateByAngleAndAxisAroundPoint(angle, axis, point),
                End = End.RotateByAngleAndAxisAroundPoint(angle, axis, point),
                Color = Color,
                IsVisible = IsVisible,
                Number = Number
            };
        }

        public Edge PerspectiveProjection(double distanse, Vertex pivot)
        {
            return new Edge
            {
                Color = Color,
                End = End.PerspectiveProjection(distanse, pivot),
                Start = Start.PerspectiveProjection(distanse, pivot),
                IsVisible = IsVisible,
                Number = Number
            };
        }

        public Edge MoveByAxis(double delta, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return new Edge(Start.Sum(new Vertex(delta, 0, 0)), End.Sum(new Vertex(delta, 0, 0)), Color, Number,
                        IsVisible);
                case Axis.Y:
                    return new Edge(Start.Sum(new Vertex(0, delta, 0)), End.Sum(new Vertex(0, delta, 0)), Color, Number,
                        IsVisible);
                case Axis.Z:
                    return new Edge(Start.Sum(new Vertex(0, 0, delta)), End.Sum(new Vertex(0, 0, delta)), Color, Number,
                        IsVisible);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            
        }

        public Vertex Center => new Vertex((Start.X + End.X)/2, (Start.Y + End.Y)/2, (Start.Z + End.Z)/2);

        public double Length()
        {
            return End.Sum(Start.Negotiate()).VectorLength();
        }

        public override string ToString()
        {
            return $"({Start},{End}) ({Number})";
        }
    }
}