using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace VerySeriousEngine.Utils
{
    public struct SimplePoint
    {
        public Vector4 Location;
        public Vector4 Color;

        public SimplePoint(Vector4 location, Color color)
        {
            Location = location;
            Color = color.ToVector4();
        }

        public static InputElement[] InputElements = new InputElement[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
            new InputElement("COLOR",    0, Format.R32G32B32A32_Float, Vector4.SizeInBytes, 0),
        };
    }
}
