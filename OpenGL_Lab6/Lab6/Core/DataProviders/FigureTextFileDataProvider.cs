using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Core.DataProviders
{
    public class FigureTextFileDataProvider : IDataProvider
    {
        private const string FiguresSubPath = @"Figures\";
        private const char Delimiter = ' ';
        private readonly string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

        public DataResult GetVertexPoints(string selector)
        {
            string[] lines = File.ReadAllLines($"{_rootPath}{FiguresSubPath}{selector}.figure");
            int lineSize = lines[0].Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries).Count();
            float[] result = lines.SelectMany(s => s.Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries))
                .Select(vertex => float.Parse(vertex.Remove(vertex.Length-1), CultureInfo.InvariantCulture)).ToArray();
            return new DataResult {Figure = result, VertexPerLineCount = lineSize};
        }
    }
}