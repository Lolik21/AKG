using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Events;
using Core.Shaders;
using OpenGL;
using Unity;

namespace Core
{
    public class GraphicsCore : IGraphicsCore
    {
        private readonly List<IShader> _shaders;

        public GraphicsCore(IUnityContainer container, IEventAggregator eventAggregator)
        {
            _shaders = container.ResolveAll<IShader>().ToList();
            eventAggregator.OnKeyDown += OnKeyDown;
        }

        public void Paint(int viewPortWidth, int viewPortHeight)
        {
            Gl.Viewport(0, 0, viewPortWidth, viewPortHeight);
            foreach (var shader in _shaders)
            {
                shader.Draw(viewPortWidth, viewPortHeight);
            }
        }

        public void Initialize()
        {
            Gl.Initialize();
            foreach (var shader in _shaders)
            {
                shader.Initialize();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.P:
                    Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    break;
                case Keys.O:
                    Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    break;
            }
        }
    }
}
