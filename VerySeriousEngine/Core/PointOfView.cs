using SharpDX;

namespace VerySeriousEngine.Core
{
    //
    // Summary:
    //     Struct that contains data needed for a camera
    public struct PointOfView
    {
        public Matrix ProjectionMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
    }
}