using VerySeriousEngine.Core;

namespace VerySeriousEngine.Components
{
    class SimpleCameraComponent : GameComponent
    {
        //
        // Summary:
        //     Game component, that responsible for updating world point of view
        public SimpleCameraComponent(GameObject parent, string componentName = null, bool isActiveAtStart = true) : base(parent, componentName, isActiveAtStart)
        {
        }


    }
}
