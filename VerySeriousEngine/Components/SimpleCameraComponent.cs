using System;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;

namespace VerySeriousEngine.Components
{
    public class SimpleCameraComponent : GameComponent
    {
        private readonly WorldObject worldParent;
        private PointOfView pointOfView;

        private Matrix projectionMatrix;
        bool isCameraPerspective;
        float fov;
        float orthoWidth;
        float orthoHeight;

        public bool IsPerspective {
            get => isCameraPerspective;
            set {
                if (isCameraPerspective == value)
                    return;

                isCameraPerspective = value;
                UpdateProjectionMatrix();
            }
        }
        public float RadPerspectiveFOV {
            get => fov;
            set {
                if (fov == value)
                    return;

                fov = value;
                if (isCameraPerspective)
                    UpdateProjectionMatrix();
            }
        }
        public float DegPerspectiveFOV {
            get => MathUtil.RadiansToGradians(fov);
            set => RadPerspectiveFOV = MathUtil.RadiansToGradians(value);
        }
        public float OrthoWidth {
            get => orthoWidth;
            set {
                if (orthoWidth == value)
                    return;

                orthoWidth = value;
                if (!isCameraPerspective)
                    UpdateProjectionMatrix();
            }
        }
        public float OrthoHeight {
            get => orthoHeight;
            set {
                if (orthoHeight == value)
                    return;

                orthoHeight = value;
                if (!isCameraPerspective)
                    UpdateProjectionMatrix();
            }
        }

        private void UpdateProjectionMatrix()
        {
            if (isCameraPerspective)
            {
                var renderer = Game.GameInstance.GameRenderer;
                projectionMatrix = Matrix.PerspectiveFovRH(fov, (float)renderer.FrameWidth / renderer.FrameHeight, .1f, 100000.0f);
            }
            else
                projectionMatrix = Matrix.OrthoRH(orthoWidth, orthoHeight, .1f, 100000.0f);
        }

        //
        // Summary:
        //     Game component, that responsible for updating world point of view
        public SimpleCameraComponent(WorldObject parent, string componentName = null, bool isActiveAtStart = true) : base(parent, componentName, isActiveAtStart)
        {
            worldParent = parent;
            pointOfView = worldParent.GameWorld.WorldPointOfView;

            // Default values
            IsPerspective = true;
            fov = MathUtil.DegreesToRadians(70);
            var renderer = Game.GameInstance.GameRenderer;
            orthoWidth = renderer.FrameWidth;
            orthoHeight = renderer.FrameHeight;

            UpdateProjectionMatrix();
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            pointOfView.ViewMatrix = Matrix.Invert(worldParent.WorldTransform);
            pointOfView.ProjectionMatrix = projectionMatrix;
        }
    }
}
