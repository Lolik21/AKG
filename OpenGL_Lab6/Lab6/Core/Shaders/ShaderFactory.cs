using System;
using System.IO;
using System.Linq;
using System.Text;
using OpenGL;

namespace Core.Shaders
{
    public class ShaderFactory : IShaderFactory
    {
        private readonly string _shaderSelector;
        private const string FragmentShaderPath = @"Shaders\FragmentShaders\";
        private const string VertexShaderPath = @"Shaders\VertexShaders\";
        private readonly string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

        public ShaderFactory(string shaderSelector)
        {
            _shaderSelector = shaderSelector;
        }

        public uint GetShader(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.FragmentShader:
                    return GetFragmentShader();
                case ShaderType.VertexShader:
                    return GetVertexShader();
            }

            return default(uint);
        }

        private uint GetVertexShader()
        {
            string[] text = File.ReadAllLines($"{_rootPath}{VertexShaderPath}{_shaderSelector}.glsl")
                .Select(s => s + "\n").ToArray();
            uint shader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(shader, text);
            Gl.CompileShader(shader);
            VerifyCompiled(shader);
            return shader;
        }

        private void VerifyCompiled(uint shader)
        {
            Gl.GetShader(shader, ShaderParameterName.CompileStatus, out int compile);
            if (compile == 0)
            {
                Gl.DeleteShader(shader);
                const int logMaxLength = 1024;
                StringBuilder infoLog = new StringBuilder(logMaxLength);
                Gl.GetShaderInfoLog(shader, logMaxLength, out int infoLogLength, infoLog);
                throw new InvalidOperationException($"Unable to compile shader: {infoLog}");
            }
        }

        private uint GetFragmentShader()
        {
            string[] text = File.ReadAllLines($"{_rootPath}{FragmentShaderPath}{_shaderSelector}.glsl")
                .Select(s => s + "\n").ToArray(); ;
            uint shader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(shader, text);
            Gl.CompileShader(shader);
            VerifyCompiled(shader);
            return shader;
        }
    }
}