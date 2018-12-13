using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core;
using Core.Events;
using Core.Operations;
using OpenGL;
using Unity;

namespace Lab6
{
    public partial class MainWindow : Form
    {
        private readonly List<Operation> _operations;
        private IGraphicsCore _graphicsCore;
        private IEventAggregator _eventAggregator;
        private int _operationIndex;

        public MainWindow()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName.Contains("Core"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Operation)) && !type.IsAbstract);
            _operations = types.Select(type => (Operation)Activator.CreateInstance(type)).ToList();

            _operationIndex = 0;
            ConfigureOperation(_operations[_operationIndex]);
            InitializeComponent();
            this.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            _eventAggregator.MouseScroll(sender, e);
            GLControl.Invalidate();
        }

        private void GLControl_ContextCreated(object sender, GlControlEventArgs e)
        {
            _graphicsCore.Initialize();
        }

        private void GLControl_Render(object sender, GlControlEventArgs e)
        {
            Control senderControl = (Control)sender;
            _graphicsCore.Paint(senderControl.ClientSize.Width, senderControl.ClientSize.Height);
        }

        private void GLControl_KeyDown(object sender, KeyEventArgs e)
        {
            _eventAggregator.KeyDown(sender, e);
            GLControl.Invalidate();
        }

        private void ConfigureOperation(Operation operation)
        {
            IUnityContainer container = operation.ConfigureContainer();
            _graphicsCore = container.Resolve<IGraphicsCore>();
            _eventAggregator = container.Resolve<IEventAggregator>();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int countOfElements = _operations.Count;
            switch (keyData)
            {
                case Keys.Up:
                    _operationIndex++;
                    if (countOfElements == _operationIndex)
                    {
                        _operationIndex--;
                        break;
                    }
                    ConfigureOperation(_operations[_operationIndex]);
                    _graphicsCore.Initialize();
                    GLControl.Invalidate();
                    break;
                case Keys.Down:
                    _operationIndex--;
                    if (_operationIndex < 0)
                    {
                        _operationIndex++;
                        break;
                    }
                    ConfigureOperation(_operations[_operationIndex]);
                    _graphicsCore.Initialize();
                    GLControl.Invalidate();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            _eventAggregator.MouseMove(sender, e);
            GLControl.Invalidate();
        }

        private void GLControl_MouseDown(object sender, MouseEventArgs e)
        {
            _eventAggregator.MouseDown(sender, e);
            GLControl.Invalidate();
        }

        private void GLControl_MouseUp(object sender, MouseEventArgs e)
        {
            _eventAggregator.MouseUp(sender, e);
            GLControl.Invalidate();
        }

        private void GLControl_DragDrop(object sender, DragEventArgs e)
        {
            _eventAggregator.DragDrop(sender, e);
            GLControl.Invalidate();
        }
    }
}
