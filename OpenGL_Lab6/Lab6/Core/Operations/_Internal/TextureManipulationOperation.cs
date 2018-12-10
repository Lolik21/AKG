using Core.DataProviders;
using Core.Shaders;
using Core.Textures;
using Unity;

namespace Core.Operations
{
    public class TextureManipulationOperation : Operation
    {
        public override IUnityContainer ConfigureContainer()
        {
            IUnityContainer container = base.ConfigureContainer();
            container.RegisterType<IDataProvider, FigureTextFileDataProvider>();
            container.RegisterType<IShader, TextureShader>(nameof(TextureShader));
            container.RegisterType<ITextureLoader, TextureLoader>();
            return container;
        }
    }
}