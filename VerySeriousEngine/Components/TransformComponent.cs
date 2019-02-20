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
        private Matrix localTransform;

        public Vector3 LocalTranslation {
            get => localTransform.TranslationVector;
            set => localTransform.TranslationVector = value;
        }
        public Quaternion LocalRotation {
            get {
                localTransform.Decompose(out _, out var rotation, out _);
                return rotation;
            }
            set {
                var translationMatrix = new Matrix
                { TranslationVector = localTransform.TranslationVector };
                var rotationMatrix = Matrix.RotationQuaternion(value);
                var scaleMatrix = new Matrix
                { ScaleVector = localTransform.ScaleVector };

                localTransform = scaleMatrix * rotationMatrix * translationMatrix;
            }
        }
        public Vector3 LocalScale {
            get => localTransform.ScaleVector;
            set => localTransform.ScaleVector = value;
        }

        public Vector3 WorldTranslation {
            get => WorldTransform.TranslationVector;
            set {
                WorldTransform.Decompose(out var scale, out var rotation, out _);

                Matrix scaleMatrix = new Matrix
                { ScaleVector = scale };
                Matrix rotationMatrix = Matrix.RotationQuaternion(rotation);
                Matrix translationMatrix = new Matrix
                { TranslationVector = value };

                WorldTransform = scaleMatrix * rotationMatrix * translationMatrix;
            }
        }
        public Quaternion WorldRotation {
            get {
                WorldTransform.Decompose(out _, out var rotation, out _);
                return rotation;
            }
            set {
                WorldTransform.Decompose(out var scale, out _, out var translation);

                Matrix scaleMatrix = new Matrix
                { ScaleVector = scale };
                Matrix rotationMatrix = Matrix.RotationQuaternion(value);
                Matrix translationMatrix = new Matrix
                { TranslationVector = translation };

                WorldTransform = scaleMatrix * rotationMatrix * translationMatrix;
            }
        }
        public Vector3 WorldScale {
            get => WorldTransform.ScaleVector;
            set {
                WorldTransform.Decompose(out _, out var rotation, out var translation);

                Matrix scaleMatrix = new Matrix
                { ScaleVector = value };
                Matrix rotationMatrix = Matrix.RotationQuaternion(rotation);
                Matrix translationMatrix = new Matrix
                { TranslationVector = translation };

                WorldTransform = scaleMatrix * rotationMatrix * translationMatrix;
            }
        }

        public Matrix LocalTransform { get => localTransform; set => localTransform = value; }
        public Matrix WorldTransform {
            get => localTransform * GetParentWorldTransform();
            set {
                var inverseParentTransform = Matrix.Invert(GetParentWorldTransform());
                localTransform = value * inverseParentTransform;
            }
        }

        public string PrettyLocalTransformString {
            get {
                return "Location: " + LocalTranslation.ToString() + "\n" +
                       "Rotation: " + LocalRotation.ToString() + "\n" +
                       "Scale: " + LocalScale.ToString() + "\n";
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
            localTransform = Matrix.Identity;

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

            return ownerParent.TransformComponent.WorldTransform;
        }
    }
}
