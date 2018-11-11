namespace SDL_Lab1
{
    public class Edge
    {
        public Edge(){}

        public Edge(Vertex start, Vertex end, (byte red, byte green, byte blue, byte alpha) color, bool isVisible = true)
        {
            Start = start;
            End = end;
            Color = color;
            IsVisible = isVisible;
        }

        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public bool IsVisible { get; set; } = true;
        public (byte red, byte green, byte blue, byte alpha) Color { get; set; }

        public Edge RotateEdgeByAngleAndAxisAroundPoint(double angle, Axis axis, Vertex point)
        {
            return new Edge
            {
                Start = Start.RotateByAngleAndAxisAroundPoint(angle, axis, point),
                End = End.RotateByAngleAndAxisAroundPoint(angle, axis, point),
                Color = Color,
                IsVisible = IsVisible
            };
        }

        public double Length()
        {
            return End.Sum(Start.Negotiate()).VectorLength();
        }

        public override string ToString()
        {
            return $"({Start},{End})";
        }
    }
}