using Core.Events;
using Unity;

namespace Core.Operations
{
    public abstract class Operation
    {
        public virtual IUnityContainer ConfigureContainer()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterSingleton<IEventAggregator, EventAggregator>();
            container.RegisterType<IGraphicsCore, GraphicsCore>();
            return container;
        }
    }
}