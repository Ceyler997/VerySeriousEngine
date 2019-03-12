using SharpDX;
using VerySeriousEngine.Components;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Objects
{
    public class SimpleControllableCamera : WorldObject, IAxisListener
    {
        private readonly SimpleCameraComponent cameraComponent;

        private Vector3 movementVector;
        private Vector2 rotationVector;

        private string forwardAxis;
        private string rightAxis;
        private string upAxis;

        private string rightRotateAxis;
        private string upRotateAxis;

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

        public string RightRotateAxis {
            get => rightRotateAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (rightRotateAxis != null)
                    inputManager.UnsubscribeFromAxis(rightRotateAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                rightRotateAxis = value;
            }
        }
        public string UpRotateAxis {
            get => upRotateAxis;
            set {
                // unsubscribe from prev axis
                var inputManager = Game.GameInstance.InputManager;
                if (upRotateAxis != null)
                    inputManager.UnsubscribeFromAxis(upRotateAxis, this);

                inputManager.SubscribeOnAxis(value, this);

                upRotateAxis = value;
            }
        }

        public float MovementSpeed { get; set; } = 750.0f;
        public float RotationRadSpeed { get; set; } = 2.5f;

        public SimpleControllableCamera(WorldObject parent = null, string objectName = null ) : base(parent, objectName)
        {
            cameraComponent = new SimpleCameraComponent(this);
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

            if (axis == UpRotateAxis)
                rotationVector[0] += RotationRadSpeed * value;

            if (axis == RightRotateAxis)
                rotationVector[1] += RotationRadSpeed * value;

            if (axis == ForwardAxis)
                movementVector += Forward * MovementSpeed * value;

            if (axis == RightAxis)
                movementVector += Right * MovementSpeed * value;

            if (axis == UpAxis)
                movementVector += Up * MovementSpeed * value;

            Logger.Log(this + " got axis update: " + axis + " value " + value);
            Logger.Log("Result movement vector: " + movementVector);
        }
    }
}
