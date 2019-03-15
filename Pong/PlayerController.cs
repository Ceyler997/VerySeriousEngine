using System;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace Pong
{
    class PongPlayerController : GameObject, IActionListener, IAxisListener
    {
        #region Private Fields
        
        private string exitAction;
        private string upAxis;
        private Vector2 movingDirection;
        private Vector2 bounds;
        #endregion

        #region Public Properties

        public Platform ControlledPlatform { get; }

        public GameField Field { get; }

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
                if (ControlledPlatform != null)
                    result += new Vector2(ControlledPlatform.Collider.Width / 2, ControlledPlatform.Collider.Height / 2);

                return result;
            }
            set {
                bounds = value/2;
                if (ControlledPlatform != null)
                    bounds -= new Vector2(ControlledPlatform.Collider.Width / 2, ControlledPlatform.Collider.Height / 2);
            }
        }

        public Vector3 PlatformStartPosition { get; set; }
        #endregion

        public PongPlayerController(GameField gameField, string objectName = null, bool isActiveAtStart = true) : base(null, objectName, isActiveAtStart)
        {
            Field = gameField ?? throw new ArgumentNullException(nameof(gameField));
            gameField.Goal += GameField_Goal;
            PlatformStartPosition = Vector3.Right * (Field.FieldWidth/2 - 100);
            ControlledPlatform = new Platform(20, 100, Color.Blue, objectName: "Player Platform")
            {
                WorldLocation = PlatformStartPosition,
            };
            FieldBounds = new Vector2(Field.FieldWidth, Field.FieldHeight);
        }

        private void GameField_Goal(ESide lostSide)
        {
            ControlledPlatform.WorldLocation = PlatformStartPosition;
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
