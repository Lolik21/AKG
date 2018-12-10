using OpenGL;

namespace Core.Shaders
{
    public interface IShaderFactory
    {
        uint GetShader(ShaderType shaderType);
    }
}