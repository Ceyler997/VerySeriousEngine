using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using VerySeriousEngine.Core;
using Rectangle = System.Drawing.Rectangle;

namespace VerySeriousEngine.Utils.Import
{
    public class TextureImporter
    {
        public static ShaderResourceView ImportTextureFromFile(string filePath, bool applyGammaCorrection = true)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            var device = Game.GameInstance.GameRenderer.Device;
            var bitmap = new Bitmap(filePath);
            Format descFormat;
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Canonical:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format24bppRgb:
                    descFormat = applyGammaCorrection ? Format.B8G8R8A8_UNorm_SRgb : Format.B8G8R8A8_UNorm;
                    break;
                default:
                    throw new ArgumentException("Unknown pixel format " + bitmap.PixelFormat);
            }

            var textureDesc = new Texture2DDescription()
            {
                MipLevels = 1,
                Format = descFormat,
                Width = bitmap.Width,
                Height = bitmap.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0)
            };


            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            DataRectangle dataRectangle = new DataRectangle(data.Scan0, data.Stride);
            var buffer = new Texture2D(device, textureDesc, dataRectangle);
            bitmap.UnlockBits(data);

            var resourceView = new ShaderResourceView(device, buffer);
            buffer.Dispose();

            return resourceView;
        }
    }
}
