using System;
using Core.DataProviders;
using Core.Events;
using Core.Textures;
using OpenGL;

namespace Core.Shaders
{
    public class TextureShader : Basic2DMovementShader
    {
        private readonly ITextureLoader _textureLoader;
        private int[] indices = {0, 1, 3, 1, 2, 3};
        private uint _texture1;
        private uint _texture2;
        private uint _program;

        public TextureShader(IDataProvider dataProvider, IEventAggregator eventAggregator, 
            ITextureLoader textureLoader) : base(eventAggregator)
        {
            _shaderProgramFactory = new ShaderProgramFactory(new ShaderFactory("Texture"));
            _figureResult = dataProvider.GetVertexPoints("Textures");
            _textureLoader = textureLoader;
        }

        public override void Draw(int viewPortWidth, int viewPortHeight)
        {
            UserProgram(_program);
            Gl.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            base.Draw(viewPortWidth, viewPortHeight);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, _texture2);
            Gl.Uniform1(Gl.GetUniformLocation(_currentProgram, "texture2"), 0);
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, _texture1);
            Gl.Uniform1(Gl.GetUniformLocation(_currentProgram, "texture1"), 1);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindVertexArray(_vertexAttrObject);
            Gl.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
        }

        public override void Initialize()
        {
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
            _texture2 = _textureLoader.GetTexture("smile.png");
            InitVertexAttrBufferInternal(vertexAttributeObject, vertexBufferObject, ebo, figureSizeInBytes, vertexes);
            return vertexAttributeObject;
        }

        private void InitVertexAttrBufferInternal(uint vertexAttributeObject, uint vertexBufferObject, uint ebo,
            uint figureSizeInBytes, float[] vertexes)
        {
            int size = _figureResult.VertexPerLineCount * sizeof(float);

            uint indSize = (uint)indices.Length * sizeof(int);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, indSize, indices, BufferUsage.DynamicDraw);

            Gl.BindVertexArray(vertexAttributeObject);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            Gl.BufferData(BufferTarget.ArrayBuffer, figureSizeInBytes, vertexes, BufferUsage.DynamicDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, size, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, size, (IntPtr)(3 * sizeof(float)));
            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(2, 2, VertexAttribType.Float, false, size, (IntPtr)(6 * sizeof(float)));
            Gl.EnableVertexAttribArray(2);
            Gl.BindVertexArray(0);
        }
    }
}