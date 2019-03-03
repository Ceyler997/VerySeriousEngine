using System;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;

namespace VerySeriousEngine.Components
{
    //
    // Summary:
    //     Game component, that responsible for objects location
    public class TransformComponent : GameComponent
    {
        private WorldObject ownerParent;

        private Vector3 scale;
        private Quaternion rotation;
        private Vector3 location;
        
        public Vector3 Location {
            get => location;
            set => location = value;
        }
        public Quaternion Rotation {
            get => rotation;
            set => rotation = value;
        }
        public Vector3 Scale {
            get => scale;
            set => scale = value;
        }

        public Vector3 WorldLocation {
            get => WorldTransform.TranslationVector;
            set {
                Matrix inversedParentTransform = Matrix.Invert(GetParentWorldTransform());
                Matrix worldTranslationMatrix = Matrix.Translation(value);
                Matrix localTranslationMatrix = worldTranslationMatrix * inversedParentTransform;
                Location = localTranslationMatrix.TranslationVector;
            }
        }
        public Quaternion WorldRotation {
            get {
                WorldTransform.Decompose(out _, out var rotation, out _);
                return rotation;
            }
            set {
                Matrix inversedParentTransform = Matrix.Invert(GetParentWorldTransform());
                Matrix worldRotationMatrix = Matrix.RotationQuaternion(value);
                Matrix localRotationMatrix = worldRotationMatrix * inversedParentTransform;
                localRotationMatrix.Decompose(out _, out rotation, out _);
            }
        }
        public Vector3 WorldScale {
            get => WorldTransform.ScaleVector;
            set {
                Matrix inversedParentTransform = Matrix.Invert(GetParentWorldTransform());
                Matrix worldScaleMatrix = Matrix.Scaling(value);
                Matrix localScaleMatrix = worldScaleMatrix * inversedParentTransform;
                Scale = localScaleMatrix.ScaleVector;
            }
        }

        public Vector3 Right {
            get => WorldTransform.Right;
        }
        public Vector3 Up {
            get => WorldTransform.Up;
        }
        public Vector3 Forward {
            get => WorldTransform.Forward;
        }

        public Matrix LocalTransform {
            get {
                Matrix scaleMatrix = Matrix.Scaling(Scale);
                Matrix rotationMatrix = Matrix.RotationQuaternion(Rotation);
                Matrix translationMatrix = Matrix.Translation(Location);

                return scaleMatrix * rotationMatrix * translationMatrix;
            }
            set => value.Decompose(out scale, out rotation, out location);
        }
        public Matrix WorldTransform {
            get => LocalTransform * GetParentWorldTransform();
            set => LocalTransform = value * Matrix.Invert(GetParentWorldTransform());
        }
        
        public string PrettyLocalTransformString {
            get {
                return "Location: " + Location.ToString() + "\n" +
                       "Rotation: " + Rotation.ToString() + "\n" +
                       "Scale: " + Scale.ToString() + "\n";
            }
        }

        public string PrettyWorldTransformString {
            get {
                WorldTransform.Decompose(out var scale, out var rotation, out var translation);
                return "Location: " + translation.ToString() + "\n" +
                       "Rotation: " + rotation.ToString() + "\n" +
                       "Scale: " + scale.ToString() + "\n";
            }
        }

        public TransformComponent(GameObject owner, string componentName = null, bool isActiveAtStart = false) : base(owner, componentName, isActiveAtStart)
        {
            LocalTransform = Matrix.Identity;

            if (owner.Parent != null)
            {
                ownerParent = owner.Parent as WorldObject;
                if (ownerParent == null)
                    throw new InvalidOperationException("Owner parent should be " + nameof(WorldObject));
            }
        }

        private Matrix GetParentWorldTransform()
        {
            if (ownerParent == null)
                return Matrix.Identity; // Object is a root

            return ownerParent.WorldTransform;
        }
    }
}
