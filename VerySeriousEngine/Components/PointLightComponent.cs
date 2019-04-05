using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;

namespace VerySeriousEngine.Components
{
    public class PointLightComponent : GameComponent
    {
        public WorldObject WorldOwner { get; }
        public float Intensity { get; set; }

        public PointLightComponent(WorldObject owner, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            WorldOwner = owner;
            Intensity = 1.0f;
        }
    }
}
