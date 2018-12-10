using Core.DataProviders;
using Core.Shaders;
using Unity;

namespace Core.Operations
{
    public class FirstTriangleOperation : Operation
    {
        public override IUnityContainer ConfigureContainer()
        {
            IUnityContainer container = base.ConfigureContainer();
            container.RegisterType<IDataProvider, FigureTextFileDataProvider>();
            container.RegisterType<IShader, RedRectangleShader>(nameof(FirstTriangleOperation));
            return container;
        }
    }
}