using System;
using System.Text;
using OpenGL;

namespace Core.Shaders
{
    public class ShaderProgramFactory : IShaderProgramFactory
    {
        private readonly IShaderFactory _shaderFactory;

        public ShaderProgramFactory(IShaderFactory shaderFactory)
        {
            _shaderFactory = shaderFactory;
        }

        public uint CreateProgram()
        {
            uint vertexShader = _shaderFactory.GetShader(ShaderType.VertexShader);
            uint fragmentShader = _shaderFactory.GetShader(ShaderType.FragmentShader);
            uint program = Gl.CreateProgram();
            Gl.AttachShader(program, vertexShader);
            Gl.AttachShader(program, fragmentShader);
            Gl.LinkProgram(program);
            VerifySuccess(program);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);
            return program;
        }

        private void VerifySuccess(uint program)
        {
            Gl.GetProgram(program, ProgramProperty.LinkStatus, out int success);
            if (success == 0)
            {
                int maxLength = 1024;
                StringBuilder builder = new StringBuilder(1024);
                Gl.GetProgramInfoLog(program, maxLength, out int length, builder);
                throw new InvalidOperationException($"Unable to compile shader: {builder}");
            }
        }
    }
}