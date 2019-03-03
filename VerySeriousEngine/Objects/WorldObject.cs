using SharpDX;
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

        public Vector3 Location {
            get => TransformComponent.Location;
            set => TransformComponent.Location = value;
        }
        public Quaternion Rotation {
            get => TransformComponent.Rotation;
            set => TransformComponent.Rotation = value;
        }
        public Vector3 Scale {
            get => TransformComponent.Scale;
            set => TransformComponent.Scale = value;
        }

        public Vector3 WorldLocation {
            get => TransformComponent.WorldLocation;
            set => TransformComponent.WorldLocation = value;
        }
        public Quaternion WorldRotation {
            get => TransformComponent.WorldRotation;
            set => TransformComponent.WorldRotation = value;
        }
        public Vector3 WorldScale {
            get => TransformComponent.WorldScale;
            set => TransformComponent.WorldScale = value;
        }

        public Vector3 Right {
            get => TransformComponent.Right;
        }
        public Vector3 Up {
            get => TransformComponent.Up;
        }
        public Vector3 Forward {
            get => TransformComponent.Forward;
        }

        public Matrix LocalTransform {
            get => TransformComponent.LocalTransform;
            set => TransformComponent.LocalTransform = value;
        }
        public Matrix WorldTransform {
            get => TransformComponent.WorldTransform;
            set => TransformComponent.WorldTransform = value;
        }

        public WorldObject(GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            TransformComponent = new TransformComponent(this);
        }
    }
}
