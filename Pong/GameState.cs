using SharpDX;
using System;
using System.Collections.Generic;
using VerySeriousEngine.Core;
using VerySeriousEngine.Utils;

namespace Pong
{
    class GameState : GameObject
    {
        public GameField GameField { get; }
        public Ball Ball { get; private set; }
        public Dictionary<ESide, int> Score { get; }

        public float BallRadius { get; set; } = 10.0f;
        public Color BallColor { get; set; } = Color.White;
        public int BallSegmentsAmout { get; set; } = 16;
        public float BallSpeed { get; set; } = 500.0f;

        public GameState(GameField gameField, GameObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            GameField = gameField;

            GameField.Goal += GameField_Goal;
            Score = new Dictionary<ESide, int>
            {
                [ESide.Left] = 0,
                [ESide.Right] = 0
            };

            Ball = new Ball(BallRadius, BallColor, BallSegmentsAmout)
            {
                MovementSpeed = BallSpeed,
            };

            ResetBall(ESide.Left);
        }

        private void GameField_Goal(ESide scoredSide)
        {
            Score[scoredSide] += 1;
            ResetBall(scoredSide);
            Logger.Log("Goal! Score is " + Score[ESide.Left] + ":" + Score[ESide.Right]);
        }

        private void ResetBall(ESide scoredSide)
        {
            if (Ball == null)
                return;

            Ball.WorldLocation = GameField.WorldLocation;

            Vector2 movementDirection = Vector2.Zero;
            switch (scoredSide)
            {
                case ESide.Left:
                    Ball.MovementDirection = Vector2.UnitX;
                    break;
                case ESide.Right:
                    Ball.MovementDirection = -Vector2.UnitX;
                    break;
            }
        }
    }
}
