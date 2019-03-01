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

        private Vector3 localScale;
        private Quaternion localRotation;
        private Vector3 localTranslation;

        public Vector3 LocalTranslation {
            get => localTranslation;
            set => localTranslation = value;
        }
        public Quaternion LocalRotation {
            get => localRotation;
            set => localRotation = value;
        }
        public Vector3 LocalScale {
            get => localScale;
            set => localScale = value;
        }

        public Vector3 WorldTranslation {
            get => WorldTransform.TranslationVector;
            set {
                Matrix inversedParentTransform = Matrix.Invert(GetParentWorldTransform());
                Matrix worldTranslationMatrix = Matrix.Translation(value);
                Matrix localTranslationMatrix = worldTranslationMatrix * inversedParentTransform;
                LocalTranslation = localTranslationMatrix.TranslationVector;
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
                localRotationMatrix.Decompose(out _, out localRotation, out _);
            }
        }
        public Vector3 WorldScale {
            get => WorldTransform.ScaleVector;
            set {
                Matrix inversedParentTransform = Matrix.Invert(GetParentWorldTransform());
                Matrix worldScaleMatrix = Matrix.Scaling(value);
                Matrix localScaleMatrix = worldScaleMatrix * inversedParentTransform;
                LocalScale = localScaleMatrix.ScaleVector;
            }
        }

        public Matrix LocalTransform {
            get {
                Matrix scaleMatrix = Matrix.Scaling(LocalScale);
                Matrix rotationMatrix = Matrix.RotationQuaternion(LocalRotation);
                Matrix translationMatrix = Matrix.Translation(LocalTranslation);

                return scaleMatrix * rotationMatrix * translationMatrix;
            }
            set => value.Decompose(out localScale, out localRotation, out localTranslation);
        }
        public Matrix WorldTransform {
            get => LocalTransform * GetParentWorldTransform();
            set => LocalTransform = value * Matrix.Invert(GetParentWorldTransform());
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

            return ownerParent.TransformComponent.WorldTransform;
        }
    }
}
