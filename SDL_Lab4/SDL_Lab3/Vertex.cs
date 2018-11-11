using System;

namespace SDL_Lab1
{
    public class Vertex
    {
        public Vertex(){}

        public Vertex(int x, int y, int z, bool isVisible = true)
        {
            X = x;
            Y = y;
            Z = z;
            IsVisible = isVisible;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public bool IsVisible { get; set; } = true;

        public Vertex RotateByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            var vertex = Sum(point.Negotiate());

            switch (axis)
            {
                case Axis.Z:
                    vertex = new Vertex(
                        (int) Math.Round(vertex.X*Math.Cos(angle) - vertex.Y*Math.Sin(angle)),
                        (int) Math.Round(vertex.X*Math.Sin(angle) + vertex.Y*Math.Cos(angle)),
                        vertex.Z,
                        IsVisible
                    );
                    break;
                case Axis.Y:
                    vertex = new Vertex(
                        (int)Math.Round(vertex.X * Math.Cos(angle) + vertex.Z * Math.Sin(angle)),
                        vertex.Y,
                        (int)Math.Round(-vertex.X * Math.Sin(angle) + vertex.Z * Math.Cos(angle)),
                        IsVisible
                    );
                    break;
                case Axis.X:
                    vertex = new Vertex(
                        vertex.X,
                        (int)Math.Round(vertex.Y * Math.Cos(angle) - vertex.Z * Math.Sin(angle)),
                        (int)Math.Round(vertex.Y * Math.Sin(angle) + vertex.Z * Math.Cos(angle)),
                        IsVisible
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            return vertex.Sum(point);
        }

        /// <summary>
        /// (X,Y,Z)+(X1,Y1,Z1)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Vertex Sum(Vertex vertex)
        {
            return new Vertex
            {
                X = X + vertex.X,
                Y = Y + vertex.Y,
                Z = Z + vertex.Z,
                IsVisible = IsVisible
            };
        }

        /// <summary>
        /// (X,Y,Z)=>(-X,-Y,-Z)
        /// </summary>
        /// <returns></returns>
        public Vertex Negotiate()
        {
            return new Vertex
            {
                X = -X,
                Y = -Y,
                Z = -Z,
                IsVisible = IsVisible
            };
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public double VectorLength()
        {
            return Math.Sqrt(X*X + Y*Y + Z*Z);
        }
    }
}