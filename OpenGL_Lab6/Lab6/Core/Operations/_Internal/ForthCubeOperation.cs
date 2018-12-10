using Core.DataProviders;
using Core.Shaders;
using Core.Textures;
using Unity;

namespace Core.Operations
{
    public class ForthCubeOperation : Operation
    {
        public override IUnityContainer ConfigureContainer()
        {
            IUnityContainer container = base.ConfigureContainer();
            container.RegisterType<IDataProvider, FigureTextFileDataProvider>();
            container.RegisterType<IShader, CubeShader>(nameof(CubeShader));
            container.RegisterType<ITextureLoader, TextureLoader>();
            return container;
        }
    }
}