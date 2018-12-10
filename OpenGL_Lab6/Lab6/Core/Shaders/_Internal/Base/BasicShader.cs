using Core.DataProviders;
using OpenGL;

namespace Core.Shaders
{
    public abstract class BasicShader : IShader
    {
        protected uint _currentProgram;
        protected uint _vertexAttrObject;
        protected DataResult _figureResult;
        protected IShaderProgramFactory _shaderProgramFactory;

        public void UserProgram(uint program)
        {
            Gl.UseProgram(program);
            _currentProgram = program;
        }

        public abstract void Draw(int viewPortWidth, int viewPortHeight);
        public abstract void Initialize();
    }
}