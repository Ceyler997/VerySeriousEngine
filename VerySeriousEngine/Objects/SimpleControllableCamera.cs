using SharpDX;
using VerySeriousEngine.Components;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Objects
{
    public class SimpleControllableCamera : WorldObject, IAxisListener
    {
        private Vector3 movementVector;
        private Vector2 rotationVector;

        private string forwardAxis;
        private string rightAxis;
        private string upAxis;

        private string turnRightAxis;
        private string turnUpAxis;

        public SimpleCameraComponent CameraComponent { get; }

        public string ForwardAxis {
            get => forwardAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if(forwardAxis != null)
                    inputManager.UnsubscribeFromAxis(forwardAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                forwardAxis = value;
            }
        }
        public string RightAxis {
            get => rightAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (rightAxis != null)
                    inputManager.UnsubscribeFromAxis(rightAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                rightAxis = value;
            }
        }
        public string UpAxis {
            get => upAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (upAxis!= null)
                    inputManager.UnsubscribeFromAxis(upAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                upAxis = value;
            }
        }

        public string TurnRightAxis {
            get => turnRightAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (turnRightAxis != null)
                    inputManager.UnsubscribeFromAxis(turnRightAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                turnRightAxis = value;
            }
        }
        public string TurnUpAxis {
            get => turnUpAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (turnUpAxis != null)
                    inputManager.UnsubscribeFromAxis(turnUpAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                turnUpAxis = value;
            }
        }

        public float MovementSpeed { get; set; } = 750.0f;
        public float RotationRadSpeed { get; set; } = 2.5f;

        public SimpleControllableCamera(WorldObject parent = null, string objectName = null ) : base(parent, objectName)
        {
            CameraComponent = new SimpleCameraComponent(this);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            Location += movementVector * frameTime;
            movementVector = Vector3.Zero;

            Rotation *= Quaternion.RotationAxis(Vector3.Right, rotationVector[0] * frameTime) *
                Quaternion.RotationAxis(Vector3.Down, rotationVector[1] * frameTime);
            rotationVector = Vector2.Zero;
        }

        public void OnAxisUpdate(string axis, float value)
        {
            if (value == 0.0f)
                return;

            if (axis == TurnUpAxis)
                rotationVector[0] += RotationRadSpeed * value;

            if (axis == TurnRightAxis)
                rotationVector[1] += RotationRadSpeed * value;

            if (axis == ForwardAxis)
                movementVector += Forward * MovementSpeed * value;

            if (axis == RightAxis)
                movementVector += Right * MovementSpeed * value;

            if (axis == UpAxis)
                movementVector += Up * MovementSpeed * value;
        }
    }
}
