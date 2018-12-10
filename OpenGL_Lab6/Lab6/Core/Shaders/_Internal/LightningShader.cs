using System;
using System.Numerics;
using Core.DataProviders;
using Core.Events;
using Core.Textures;
using OpenGL;

namespace Core.Shaders
{
    public class LightningShader : Basic3DMovementShader
    {
        private readonly ITextureLoader _textureLoader;
        private readonly IShaderProgramFactory _lampShaderProgramFactory;
        private readonly IShaderProgramFactory _worldMapProgramFactory;
        private readonly Vector3 _lampLocation = new Vector3(1.2f, 1.0f, 2.0f);
        private uint _texture1;
        private uint _worldMapTexture;
        private uint _lampProgram;
        private uint _worldMapProgram;
        private uint _program;

        private uint _worldMapVAO;
        private string[] _faces = 
        {
            "right.jpg",
            "left.jpg",
            "top.jpg",
            "bottom.jpg",
            "front.jpg",
            "back.jpg"
        };

        private DataResult _worldMapResult;

        public LightningShader(IDataProvider dataProvider, IEventAggregator eventAggregator,
            ITextureLoader textureLoader) : base(eventAggregator)
        {
            _shaderProgramFactory = new ShaderProgramFactory(new ShaderFactory("Lightning"));
            _lampShaderProgramFactory = new ShaderProgramFactory(new ShaderFactory("LightningLamp"));
            _worldMapProgramFactory = new ShaderProgramFactory(new ShaderFactory("WorldMap"));
            _figureResult = dataProvider.GetVertexPoints("Lightning");
            _worldMapResult = dataProvider.GetVertexPoints("WorldMap");
            _textureLoader = textureLoader;
        }

        public override void Draw(int viewPortWidth, int viewPortHeight)
        {
            UserProgram(_program);
            base.Draw(viewPortWidth, viewPortHeight);
            DrawInit();
            DrawFigure();
            DrawLamp(viewPortWidth, viewPortHeight);
            DrawWorldMap(viewPortWidth, viewPortHeight);
        }

        private void DrawInit()
        {
            Gl.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void DrawWorldMap(int viewPortWidth, int viewPortHeight)
        {
            Gl.DepthFunc(DepthFunction.Lequal);
            UserProgram(_worldMapProgram);
            ViewMatrix = Matrix4x4.CreateLookAt(_cameraPosition, _cameraPosition + _cameraFront, _cameraUp);
            ViewMatrix.M41 = 0;
            ViewMatrix.M42 = 0;
            ViewMatrix.M43 = 0;
            ViewMatrix.M44 = 0;
            ViewMatrix.M14 = 0;
            ViewMatrix.M24 = 0;
            ViewMatrix.M34 = 0;
            int location = Gl.GetUniformLocation(_currentProgram, "view");
            Gl.UniformMatrix4f(location, 1, false, ref ViewMatrix);
            SetProjection(viewPortWidth, viewPortHeight);
            Gl.BindVertexArray(_worldMapVAO);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.TextureCubeMap, _worldMapTexture);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
            Gl.BindVertexArray(0);
            Gl.DepthFunc(DepthFunction.Less);
        }

        private void DrawFigure()
        {
            UserProgram(_program);
            //Gl.BindTexture(TextureTarget.Texture2d, _texture1);
            var location = Gl.GetUniformLocation(_currentProgram, "objectColor");
            Gl.Uniform3(location, 1.0f, 0.5f, 0.31f);
            location = Gl.GetUniformLocation(_currentProgram, "lightColor");
            Gl.Uniform3(location, 1.0f, 1.0f, 1.0f);
            location = Gl.GetUniformLocation(_currentProgram, "lightPos");
            Gl.Uniform3(location, _lampLocation.X, _lampLocation.Y, _lampLocation.Z);
            location = Gl.GetUniformLocation(_currentProgram, "cameraPosition");
            Gl.Uniform3(location, _cameraPosition.X, _cameraPosition.Y, _cameraPosition.Z);
            Gl.BindVertexArray(_vertexAttrObject);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, _figureResult.Figure.Length);
            Gl.BindVertexArray(0);
        }

        private void DrawLamp(int viewPortWidth, int viewPortHeight)
        {
            UserProgram(_lampProgram);
            SetView();
            SetProjection(viewPortWidth, viewPortHeight);
            Matrix4x4 modelMatrix = Matrix4x4.CreateScale(0.2f, _lampLocation);
            SetModel(modelMatrix);
            Gl.BindVertexArray(_vertexAttrObject);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, _figureResult.Figure.Length);
            Gl.BindVertexArray(0);
        }

        public override void Initialize()
        {
            Gl.Enable(EnableCap.DepthTest);
            _vertexAttrObject = InitVertexAttrBuffer(_figureResult.Figure);
            _worldMapVAO = InitWorldMap(_worldMapResult.Figure);
            //_texture1 = _textureLoader.GetTexture("orig.jpg");
            _worldMapTexture = _textureLoader.GetWorldMapTexture(_faces);
            _program = _shaderProgramFactory.CreateProgram();
            _lampProgram = _lampShaderProgramFactory.CreateProgram();
            _worldMapProgram = _worldMapProgramFactory.CreateProgram();
        }

        private uint InitVertexAttrBuffer(float[] vertexes)
        {
            uint vertexBufferObject = Gl.GenBuffer();
            uint vertexAttributeObject = Gl.GenVertexArray();
            uint figureSizeInBytes = (uint)(sizeof(float) * vertexes.Length);
            InitVertexAttrBufferInternal(vertexAttributeObject, vertexBufferObject, figureSizeInBytes, vertexes);
            return vertexAttributeObject;
        }

        private void InitVertexAttrBufferInternal(uint vertexAttributeObject, uint vertexBufferObject,
            uint figureSizeInBytes, float[] vertexes)
        {
            int size = _figureResult.VertexPerLineCount * sizeof(float);

            Gl.BindVertexArray(vertexAttributeObject);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            Gl.BufferData(BufferTarget.ArrayBuffer, figureSizeInBytes, vertexes, BufferUsage.DynamicDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, size, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, size, (IntPtr)(3 * sizeof(float)));
            Gl.EnableVertexAttribArray(1);
            Gl.BindVertexArray(0);
        }

        private uint InitWorldMap(float[] vertexes)
        {
            uint vao = Gl.GenVertexArray();
            uint vbo = Gl.GenBuffer();
            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)vertexes.Length * sizeof(float), vertexes, BufferUsage.StaticDraw);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, 3 * sizeof(float), IntPtr.Zero);
            return vao;
        }
    }
}