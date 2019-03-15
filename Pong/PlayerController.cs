using SharpDX;
using VerySeriousEngine.Components.Physics2D;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace Pong
{
    class PongPlayerController : GameObject, IActionListener, IAxisListener
    {
        #region Private Fields

        private Platform controlledPlatform;
        private string exitAction;
        private string upAxis;
        private Vector2 movingDirection;
        private Vector2 bounds;
        #endregion

        #region Public Properties

        public Platform ControlledPlatform {
            get => controlledPlatform;
            set {
                if (controlledPlatform == value)
                    return;

                var fieldBounds = FieldBounds;
                controlledPlatform = value;
                // update field bounds to make it match to new platform
                FieldBounds = fieldBounds;
            }
        }

        public string ExitAction {
            get => exitAction;
            set {
                if (exitAction == value)
                    return;

                if(exitAction != null)
                {
                    Game.GameInstance.InputManager.UnsubscribeFromAction(exitAction, this);
                    exitAction = null;
                }

                if(value != null)
                {
                    Game.GameInstance.InputManager.SubscribeOnAction(value, this);
                    exitAction = value;
                }
            }
        }

        public string UpAxis{
            get => upAxis;
            set {
                if (upAxis == value)
                    return;

                if (upAxis != null)
                {
                    Game.GameInstance.InputManager.UnsubscribeFromAxis(upAxis, this);
                    upAxis = null;
                }

                if (value != null)
                {
                    Game.GameInstance.InputManager.SubscribeOnAxis(value, this);
                    upAxis = value;
                }
            }
        }

        public float PlatformSpeed { get; set; } = 250.0f;

        public Vector2 FieldBounds {
            get {
                var result = bounds;
                if (controlledPlatform != null)
                    result += new Vector2(controlledPlatform.Collider.Width / 2, controlledPlatform.Collider.Height / 2);

                return result;
            }
            set {
                bounds = value/2;
                if (controlledPlatform != null)
                    bounds -= new Vector2(controlledPlatform.Collider.Width / 2, controlledPlatform.Collider.Height / 2);
            }
        }
        #endregion

        public PongPlayerController(Platform controlledPlatform, string objectName = null, bool isActiveAtStart = true) : base(null, objectName, isActiveAtStart)
        {
            ControlledPlatform = controlledPlatform;
            FieldBounds = new Vector2(800, 600);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            if (ControlledPlatform == null)
                return;

            var location = ControlledPlatform.WorldLocation;
            location += new Vector3(movingDirection * PlatformSpeed * frameTime, 0);
            location.X = MathUtil.Clamp(location.X, -bounds.X, bounds.X);
            location.Y = MathUtil.Clamp(location.Y, -bounds.Y, bounds.Y);
            ControlledPlatform.Location = location;
        }

        #region Controls Handling

        public void OnPressed(string action)
        {
            if (action == ExitAction)
                Game.GameInstance.ExitGame();
            else
                Logger.LogWarning("Unknown action " + action + " in controller");
        }

        public void OnReleased(string action) { }

        public void OnAxisUpdate(string axis, float value)
        {
            if (axis == UpAxis)
                movingDirection.Y = value;
            else
                Logger.LogWarning("Unknown axis " + axis + " in controller");
        }
        #endregion
    }
}
