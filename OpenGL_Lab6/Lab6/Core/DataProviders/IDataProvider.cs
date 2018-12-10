namespace Core.DataProviders
{
    public interface IDataProvider
    {
        DataResult GetVertexPoints(string selector);
    }
}