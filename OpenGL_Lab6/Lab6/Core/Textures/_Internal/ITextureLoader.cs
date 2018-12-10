namespace Core.Textures
{
    public interface ITextureLoader
    {
        uint GetTexture(string selector);
        uint GetWorldMapTexture(string[] selectors);
    }
}