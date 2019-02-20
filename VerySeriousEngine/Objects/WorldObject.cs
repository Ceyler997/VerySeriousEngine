using VerySeriousEngine.Components;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Objects
{
    //
    // Summary:
    //     Game object, that contains transform component
    public class WorldObject : GameObject
    {
        public TransformComponent TransformComponent { get; }

        public WorldObject(GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            TransformComponent = new TransformComponent(this);
        }
    }
}
