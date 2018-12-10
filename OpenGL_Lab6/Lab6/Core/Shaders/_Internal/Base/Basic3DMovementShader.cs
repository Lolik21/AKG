using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Core.Events;
using Core.Extensions;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Core.Shaders
{
    public class Basic3DMovementShader : BasicShader
    {
        protected Matrix4x4 ModelMatrix = Matrix4x4.Identity;
        protected Matrix4x4 ViewMatrix = Matrix4x4.Identity;
        protected Matrix4x4 ProjectionMatrix = Matrix4x4.Identity;

        protected Vector3 _cameraPosition;
        protected Vector3 _cameraUp;
        protected Vector3 _cameraFront;

        private bool _isLeftMousePressed;
        private bool _isRightMousePressed;
        private Vector2 _prevPoint;
        private float _fov = 45.0f;

        private readonly Matrix4x4 _transposeYUp = Matrix4x4.CreateTranslation(0, 0.01f, 0);
        private readonly Matrix4x4 _transposeYDown = Matrix4x4.CreateTranslation(0, -0.01f, 0);
        private readonly Matrix4x4 _transposeXUp = Matrix4x4.CreateTranslation(0.01f, 0, 0);
        private readonly Matrix4x4 _transposeXDown = Matrix4x4.CreateTranslation(-0.01f, 0, 0);
        private readonly Matrix4x4 _rotationLeftX = Matrix4x4.CreateRotationX(0.1f);
        private readonly Matrix4x4 _rotationLeftY = Matrix4x4.CreateRotationY(0.1f);
        private readonly Matrix4x4 _rotationLeftZLeft = Matrix4x4.CreateRotationZ(0.1f, Vector3.Zero);
        private readonly Matrix4x4 _rotationLeftZRight = Matrix4x4.CreateRotationZ(-0.1f, Vector3.Zero);

        private float _yaw = -90.0f;
        private float _pitch = 0.0f;

        public Basic3DMovementShader(IEventAggregator eventAggregator)
        {
            _cameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
            _cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
            _cameraFront = new Vector3(0.0f, 0.0f, -1.0f);

            eventAggregator.OnKeyDown += OnOnKeyDown;
            eventAggregator.OnMouseDown += OnMouseDown;
            eventAggregator.OnMouseMove += OnMouseMove;
            eventAggregator.OnMouseUp += OnMouseUp;
            eventAggregator.OnMouseScroll += OnOnMouseScroll;
        }

        public override void Draw(int viewPortWidth, int viewPortHeight)
        {
            SetView();
            SetProjection(viewPortWidth, viewPortHeight);
            SetModel(ModelMatrix);
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        protected void SetView()
        {
            ViewMatrix = Matrix4x4.CreateLookAt(_cameraPosition, _cameraPosition + _cameraFront, _cameraUp);
            int location = OpenGL.Gl.GetUniformLocation(_currentProgram, "view");
            OpenGL.Gl.UniformMatrix4f(location, 1, false, ref ViewMatrix);
        }

        protected void SetProjection(int viewPortWidth, int viewPortHeight)
        {
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov.ToRadians(),
                (float)viewPortWidth / viewPortHeight, 0.1f, 100.0f);
            int location = OpenGL.Gl.GetUniformLocation(_currentProgram, "projection");
            OpenGL.Gl.UniformMatrix4f(location, 1, false, ref ProjectionMatrix);
        }

        protected void SetModel(Matrix4x4 modelMatrix)
        {
            int location = OpenGL.Gl.GetUniformLocation(_currentProgram, "model");
            OpenGL.Gl.UniformMatrix4f(location, 1, false, ref modelMatrix);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isLeftMousePressed = false;
            _isRightMousePressed = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Control senderControl = (Control)sender;
            GetNormalized(e.Location, senderControl, out float newX, out float newY);
            

            if (_isLeftMousePressed)
            {
                float dx = newX - _prevPoint.X;
                float dy = newY - _prevPoint.Y;
                ModelMatrix *= Matrix4x4.CreateRotationX(-dy, Vector3.Zero);
                ModelMatrix *= Matrix4x4.CreateRotationY(dx, Vector3.Zero);
            }

            if (_isRightMousePressed)
            {
                float dx = newX - _prevPoint.X;
                float dy = newY - _prevPoint.Y;

                float sensitivity = 50.0f;
                dx = dx * sensitivity;
                dy = dy * sensitivity;

                _yaw += dx;
                _pitch += dy;

                if (_pitch > 89.0f)
                    _pitch = 89.0f;
                if (_pitch < -89.0f)
                    _pitch = -89.0f;

                Vector3 newFront;
                newFront.X = (float) (Math.Cos(_yaw.ToRadians()) * Math.Cos(_pitch.ToRadians()));
                newFront.Y = (float) Math.Sin(_pitch.ToRadians());
                newFront.Z = (float) (Math.Sin(_yaw.ToRadians()) * Math.Cos(_pitch.ToRadians()));
                _cameraFront = Vector3.Normalize(newFront);
            }

            _prevPoint.X = newX;
            _prevPoint.Y = newY;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isLeftMousePressed = true;
                GetNormalized(e.Location, sender as Control, out _prevPoint.X, out _prevPoint.Y);
            }

            if (e.Button == MouseButtons.Right)
            {
                _isRightMousePressed = true;
            }

        }

        private void GetNormalized(Point point, Control sender, out float x, out float y)
        {
            x = -1.0f + 2.0f * point.X / sender.Width;
            y = 1.0f - 2.0f * point.Y / sender.Height;
        }

        private void OnOnKeyDown(object sender, KeyEventArgs e)
        {

            if (_isRightMousePressed)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        _cameraPosition += 0.2f * _cameraFront;
                        break;
                    case Keys.A:
                        _cameraPosition -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * 0.2f;
                        break;
                    case Keys.S:
                        _cameraPosition -= 0.2f * _cameraFront;
                        break;
                    case Keys.D:
                        _cameraPosition += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * 0.2f;
                        break;
                }
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                    ModelMatrix = ModelMatrix * _transposeYUp;
                    break;
                case Keys.A:
                    ModelMatrix = ModelMatrix * _transposeXDown;
                    break;
                case Keys.S:
                    ModelMatrix = ModelMatrix * _transposeYDown;
                    break;
                case Keys.D:
                    ModelMatrix = ModelMatrix * _transposeXUp;
                    break;
                case Keys.E:
                    ModelMatrix = ModelMatrix * _rotationLeftZLeft;
                    break;
                case Keys.Q:
                    ModelMatrix = ModelMatrix * _rotationLeftZRight;
                    break;
                case Keys.X:
                    ModelMatrix = ModelMatrix * _rotationLeftX;
                    break;
                case Keys.Y:
                    ModelMatrix = ModelMatrix * _rotationLeftY;
                    break;
            }
        }
        private void OnOnMouseScroll(object sender, MouseEventArgs e)
        {
            if (_fov >= 1.0f && _fov <= 45.0f)
                _fov -= (float)e.Delta / 100;
            if (_fov <= 1.0f)
                _fov = 1.0f;
            if (_fov >= 45.0f)
                _fov = 45.0f;
        }
    }
}