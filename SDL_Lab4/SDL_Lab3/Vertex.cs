using System;
using System.Collections.Generic;

namespace SDL_Lab1
{
    public class Vertex
    {
        public Vertex(){}

        public Vertex(double x, double y, double z, bool isVisible = true)
        {
            X = x;
            Y = y;
            Z = z;
            IsVisible = isVisible;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsVisible { get; set; } = true;

        public Vertex RotateByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            var vertex = Sum(point.Negotiate());

            switch (axis)
            {
                case Axis.Z:
                    vertex = new Vertex(
                        Math.Round(vertex.X*Math.Cos(angle) - vertex.Y*Math.Sin(angle)),
                        Math.Round(vertex.X*Math.Sin(angle) + vertex.Y*Math.Cos(angle)),
                        vertex.Z,
                        IsVisible
                    );
                    break;
                case Axis.Y:
                    vertex = new Vertex(
                        Math.Round(vertex.X * Math.Cos(angle) + vertex.Z * Math.Sin(angle)),
                        vertex.Y,
                        Math.Round(-vertex.X * Math.Sin(angle) + vertex.Z * Math.Cos(angle)),
                        IsVisible
                    );
                    break;
                case Axis.X:
                    vertex = new Vertex(
                        vertex.X,
                        Math.Round(vertex.Y * Math.Cos(angle) - vertex.Z * Math.Sin(angle)),
                        Math.Round(vertex.Y * Math.Sin(angle) + vertex.Z * Math.Cos(angle)),
                        IsVisible
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            return vertex.Sum(point);
        }

        public Vertex RotateByAngleAroundVector(double angle, Vertex rotationVector, Vertex pivot)
        {
            var vertex = Sum(pivot.Negotiate());

            rotationVector = rotationVector.NormalizeVector();

            var x = vertex.X * (Math.Cos(angle) + (1 - Math.Cos(angle))*rotationVector.X*rotationVector.X)
                    + vertex.Y *((1 - Math.Cos(angle))*rotationVector.X*rotationVector.Y - Math.Sin(angle)*rotationVector.Z)
                    + vertex.Z *((1 - Math.Cos(angle))*rotationVector.X*rotationVector.Z + Math.Sin(angle)*rotationVector.Y);

            var y = vertex.X *((1 - Math.Cos(angle))*rotationVector.X*rotationVector.Y + Math.Sin(angle)*rotationVector.Z)
                    + vertex.Y *(Math.Cos(angle) + (1 - Math.Cos(angle))*rotationVector.Y*rotationVector.Y)
                    + vertex.Z *((1 - Math.Cos(angle))*rotationVector.Y*rotationVector.Z - Math.Sin(angle)*rotationVector.X);

            var z = vertex.X *((1 - Math.Cos(angle))*rotationVector.X*rotationVector.Z - Math.Sin(angle)*rotationVector.Y)
                    + vertex.Y *((1 - Math.Cos(angle))*rotationVector.Y*rotationVector.Z + Math.Sin(angle)*rotationVector.X)
                    + vertex.Z *(Math.Cos(angle) + (1 - Math.Cos(angle))*rotationVector.Z*rotationVector.Z);

            return new Vertex
            {
                IsVisible = IsVisible,
                X = x,
                Y = y,
                Z = z
            }.Sum(pivot);
        }

        public Vertex MoveByAxis(double delta, Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return Sum(new Vertex(delta, 0, 0));
                case Axis.Y:
                    return Sum(new Vertex(0, delta, 0));
                case Axis.Z:
                    return Sum(new Vertex(0, 0, delta));
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
            
        }

        public Vertex PerspectiveProjection(double distanse, Vertex pivot)
        {
            var vertex = Sum(pivot.Negotiate());
            vertex = new Vertex
            {
                X = vertex.X/(vertex.Z/distanse),
                Y = vertex.Y/(vertex.Z/distanse),
                Z = vertex.Z,
                IsVisible = IsVisible
            };

            return vertex.Sum(pivot);
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

        public bool IsOnLine(Vertex p1, Vertex p2)
        {
            return Math.Abs((Y - p1.Y)/(p2.Y - p1.Y) - (X - p1.X)/(p2.X - p1.X)) < 0.0001;
        }

        public override string ToString()
        {
            return $"({X};{Y};{Z})";
        }

        #region Vectors functions

        public double VectorLength()
        {
            return Math.Sqrt(X*X + Y*Y + Z*Z);
        }

        public Vertex VectorMultiply(Vertex vector)
        {
            return new Vertex(Y*vector.Z - Z*vector.Y, Z*vector.X - X*vector.Z, X*vector.Y - Y*vector.X);
        }

        public double ScalarMultiply(Vertex vector)
        {
            return X*vector.X + Y*vector.Y + Z*vector.Z;
        }

        public Vertex NormalizeVector()
        {
            return new Vertex
            {
                X = X / VectorLength(),
                Y = Y / VectorLength(),
                Z = Z / VectorLength(),
            };
        }

        public Vertex MultiplyByScalar(int scalar)
        {
            return new Vertex
            {
                X = X*scalar,
                Y = Y*scalar,
                Z = Z*scalar
            };
        }
        
        #endregion

        #region Equality

        protected bool Equals(Vertex other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && IsVisible == other.IsVisible;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vertex) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ Z.GetHashCode();
                hashCode = (hashCode*397) ^ IsVisible.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}