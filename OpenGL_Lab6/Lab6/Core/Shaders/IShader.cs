namespace Core.Shaders
{
    public interface IShader
    {
        void UserProgram(uint program);
        void Draw(int viewPortWidth, int viewPortHeight);
        void Initialize();
    }
}