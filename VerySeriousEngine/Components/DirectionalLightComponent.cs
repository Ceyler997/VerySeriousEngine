using SharpDX;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Components
{
    public class DirectionalLightComponent : GameComponent
    {
        public Vector3 Direction { get; set; }
        public float Intensity { get; set; }

        public DirectionalLightComponent(GameObject owner, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            Direction = Vector3.Down;
            Intensity = 1.0f;
        }
    }
}