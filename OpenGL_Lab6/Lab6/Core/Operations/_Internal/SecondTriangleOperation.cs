using Core.DataProviders;
using Core.Shaders;
using Unity;

namespace Core.Operations
{
    public class SecondTriangleOperation : Operation
    {
        public override IUnityContainer ConfigureContainer()
        {
            IUnityContainer container = base.ConfigureContainer();
            container.RegisterType<IDataProvider, FigureTextFileDataProvider>();
            container.RegisterType<IShader, ColorfulRectangleShader>(nameof(ColorfulRectangleShader));
            return container;
        }
    }
}