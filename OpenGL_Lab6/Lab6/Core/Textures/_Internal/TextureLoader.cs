using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenGL;
using PixelFormat = OpenGL.PixelFormat;

namespace Core.Textures
{
    public class TextureLoader : ITextureLoader
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory + @"Textures\Pictures\";
        public uint GetTexture(string selector)
        {
            uint texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);

            Bitmap bitmap = new Bitmap(_path + selector);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, 
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            Gl.GenerateMipmap(TextureTarget.Texture2d);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            return texture;
        }

        public uint GetWorldMapTexture(string[] selectors)
        {
            uint texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.TextureCubeMap, texture);
            for (int i = 0; i < selectors.Length; i++)
            {
                Bitmap bitmap = new Bitmap(_path + selectors[i]);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
            }

            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, Gl.CLAMP_TO_EDGE);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, Gl.CLAMP_TO_EDGE);
            Gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, Gl.CLAMP_TO_EDGE);

            return texture;
        }
    }
}