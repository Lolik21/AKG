using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Core.Events;

namespace Core.Shaders
{
    public class Basic2DMovementShader : BasicShader
    {
        protected Matrix4x4 Transform;
        private bool _isMousePressed;
        private readonly float[] _prevPoint = new float[2];

        private readonly Matrix4x4 _transposeYUp = Matrix4x4.CreateTranslation(0, 0.01f, 0);
        private readonly Matrix4x4 _transposeYDown = Matrix4x4.CreateTranslation(0, -0.01f, 0);
        private readonly Matrix4x4 _transposeXUp = Matrix4x4.CreateTranslation(0.01f, 0, 0);
        private readonly Matrix4x4 _transposeXDown = Matrix4x4.CreateTranslation(-0.01f, 0, 0);
        private readonly Matrix4x4 _rotationLeftX = Matrix4x4.CreateRotationX(0.1f);
        private readonly Matrix4x4 _rotationLeftY = Matrix4x4.CreateRotationY(0.1f);
        private readonly Matrix4x4 _rotationLeftZLeft = Matrix4x4.CreateRotationZ(0.1f, Vector3.Zero);
        private readonly Matrix4x4 _rotationLeftZRight = Matrix4x4.CreateRotationZ(-0.1f, Vector3.Zero);

        public Basic2DMovementShader(IEventAggregator eventAggregator)
        {
            eventAggregator.OnKeyDown += OnOnKeyDown;
            eventAggregator.OnMouseDown += OnMouseDown;
            eventAggregator.OnMouseMove += OnMouseMove;
            eventAggregator.OnMouseUp += OnMouseUp;
            Transform = Matrix4x4.Identity;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isMousePressed = false;
            
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMousePressed)
            {
                Control senderControl = (Control)sender;
                GetNormalized(e.Location, senderControl, out float newX, out float newY);
                float dx = newX - _prevPoint[0];
                float dy = newY - _prevPoint[1];
                Transform *= Matrix4x4.CreateTranslation(dx, dy, 0); ;
                _prevPoint[0] = newX;
                _prevPoint[1] = newY;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _isMousePressed = true;
            GetNormalized(e.Location, sender as Control, out _prevPoint[0], out _prevPoint[1]);
        }

        private void GetNormalized(Point point, Control sender, out float x, out float y)
        {
            x = -1.0f + 2.0f * point.X / sender.Width;
            y = 1.0f - 2.0f * point.Y / sender.Height;
        }

        private void OnOnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    Transform = Transform * _transposeYUp;
                    break;
                case Keys.A:
                    Transform = Transform * _transposeXDown;
                    break;
                case Keys.S:
                    Transform = Transform * _transposeYDown;
                    break;
                case Keys.D:
                    Transform = Transform * _transposeXUp;
                    break;
                case Keys.E:
                    Transform = Transform * _rotationLeftZLeft;
                    break;
                case Keys.Q:
                    Transform = Transform * _rotationLeftZRight;
                    break;
                case Keys.X:
                    Transform = Transform * _rotationLeftX;
                    break;
                case Keys.Y:
                    Transform = Transform * _rotationLeftY;
                    break;
            }
        }

        public override void Draw(int viewPortWidth, int viewPortHeight)
        {
            int location = OpenGL.Gl.GetUniformLocation(_currentProgram, "transform");
            OpenGL.Gl.UniformMatrix4f(location, 1, false, ref Transform);
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}