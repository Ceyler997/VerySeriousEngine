using System;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace Pong
{

    abstract class BasicPlatformController : GameObject
    {
        #region Private Fields

        protected Vector2 movingDirection;
        protected Vector2 bounds;
        #endregion

        #region Public Properties

        public Platform ControlledPlatform { get; }
        public GameField Field { get; }
        public float PlatformSpeed { get; set; } = 350.0f;
        public Vector2 FieldBounds {
            get {
                var result = bounds;
                if (ControlledPlatform != null)
                    result += new Vector2(ControlledPlatform.Collider.Width / 2, ControlledPlatform.Collider.Height / 2);

                return result;
            }
            set {
                bounds = value / 2;
                if (ControlledPlatform != null)
                    bounds -= new Vector2(ControlledPlatform.Collider.Width / 2, ControlledPlatform.Collider.Height / 2);
            }
        }
        public Vector3 PlatformStartLocation { get; set; }
        #endregion

        public BasicPlatformController(GameField gameField, Vector3 platformStartLocation, Color platformColor, string objectName = null) : base(null, objectName)
        {
            Field = gameField ?? throw new ArgumentNullException(nameof(gameField));
            gameField.Goal += GameField_Goal;
            PlatformStartLocation = platformStartLocation;
            ControlledPlatform = new Platform(20, 100, platformColor, objectName: "Game Platform")
            {
                WorldLocation = PlatformStartLocation,
            };
            FieldBounds = new Vector2(Field.FieldWidth, Field.FieldHeight);
        }

        private void GameField_Goal(ESide lostSide)
        {
            ControlledPlatform.WorldLocation = PlatformStartLocation;
        }

        protected void MoveToLocation(Vector3 target, float frameTime)
        {
            if (ControlledPlatform == null)
                return;

            Vector3 location = ControlledPlatform.WorldLocation;

            Vector3 shift = target - location;
            shift.Z = 0;
            float MaxShiftLength = frameTime * PlatformSpeed;
            if(shift.Length() > MaxShiftLength)
                shift = Vector3.Normalize(shift) * MaxShiftLength;

            location += shift;
            location.X = MathUtil.Clamp(location.X, -bounds.X, bounds.X);
            location.Y = MathUtil.Clamp(location.Y, -bounds.Y, bounds.Y);
            ControlledPlatform.WorldLocation = location;
        }
    }

    class PongPlayerController : BasicPlatformController, IActionListener, IAxisListener
    {        
        private string exitAction;
        private string upAxis;

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

        public PongPlayerController(GameField gameField, Vector3 platformStartLocation) : base(gameField, platformStartLocation, Color.Blue, "Player Platform Controller") { }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            MoveToLocation(ControlledPlatform.WorldLocation + Vector3.Up * movingDirection.Y * PlatformSpeed, frameTime);
        }

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
    }

    class PontAIController : BasicPlatformController
    {
        public Ball TrackingBall { get; }

        public PontAIController(GameField gameField, Ball ball, Vector3 platformStartLocation) : base(gameField, platformStartLocation, Color.Red, "AI Platform Controller")
        {
            TrackingBall = ball ?? throw new ArgumentNullException(nameof(ball));
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            Vector3 targetLocation = TrackingBall.WorldLocation;
            targetLocation.X = ControlledPlatform.WorldLocation.X;

            MoveToLocation(targetLocation, frameTime);
        }
    }
}
