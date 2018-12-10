using System;
using Core.DataProviders;
using Core.Events;
using Core.Textures;
using OpenGL;

namespace Core.Shaders
{
    public class CubeShader : Basic3DMovementShader
    {
        private readonly ITextureLoader _textureLoader;
        private uint _texture1;
        private uint _program;

        public CubeShader(IDataProvider dataProvider, IEventAggregator eventAggregator,
            ITextureLoader textureLoader) : base(eventAggregator)
        {
            _shaderProgramFactory = new ShaderProgramFactory(new ShaderFactory("Cube"));
            _figureResult = dataProvider.GetVertexPoints("Cube");
            _textureLoader = textureLoader;
        }

        public override void Draw(int viewPortWidth, int viewPortHeight)
        {
            UserProgram(_program);
            Gl.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.BindTexture(TextureTarget.Texture2d, _texture1);
            base.Draw(viewPortWidth, viewPortHeight);
            Gl.BindVertexArray(_vertexAttrObject);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, _figureResult.Figure.Length);
        }

        public override void Initialize()
        {
            Gl.Enable(EnableCap.DepthTest);
            _vertexAttrObject = InitVertexAttrBuffer(_figureResult.Figure);
            Gl.BindVertexArray(_vertexAttrObject);
            Gl.BindVertexArray(0);
            uint program = _shaderProgramFactory.CreateProgram();
            _program = program;
        }

        private uint InitVertexAttrBuffer(float[] vertexes)
        {
            uint vertexBufferObject = Gl.GenBuffer();
            uint vertexAttributeObject = Gl.GenVertexArray();
            uint ebo = Gl.GenBuffer();
            uint figureSizeInBytes = (uint)(sizeof(float) * vertexes.Length);
            _texture1 = _textureLoader.GetTexture("My.png");
            InitVertexAttrBufferInternal(vertexAttributeObject, vertexBufferObject, ebo, figureSizeInBytes, vertexes);
            return vertexAttributeObject;
        }

        private void InitVertexAttrBufferInternal(uint vertexAttributeObject, uint vertexBufferObject, uint ebo,
            uint figureSizeInBytes, float[] vertexes)
        {
            int size = _figureResult.VertexPerLineCount * sizeof(float);

            Gl.BindVertexArray(vertexAttributeObject);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            Gl.BufferData(BufferTarget.ArrayBuffer, figureSizeInBytes, vertexes, BufferUsage.DynamicDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, size, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, size, (IntPtr)(3 * sizeof(float)));
            Gl.EnableVertexAttribArray(1);
            Gl.BindVertexArray(0);
        }
    }
}