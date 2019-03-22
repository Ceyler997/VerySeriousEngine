using SharpDX;
using System;
using System.Windows.Forms;
using VerySeriousEngine.Core;
using VerySeriousEngine.Input;
using VerySeriousEngine.Objects;

namespace TestProject
{
    class TestGame
    {
        static void Main(string[] args)
        {
            var gameWidth = 800;
            var gameHeight = 600;

            var SolarSystemGame = Game.CreateGame("Space", gameWidth, gameHeight, true);
            SolarSystemGame.CurrentWorld = new World("Solar System");

            SetupInput(SolarSystemGame.InputManager);

            var camera = new SimpleControllableCamera(objectName: "Camera")
            {
                ForwardAxis = "Forward",
                RightAxis = "Right",
                UpAxis = "Up",
                TurnRightAxis = "Turn Right",
                TurnUpAxis = "Turn Up",
                WorldLocation = Vector3.BackwardRH * 150,
            };

            var Center = new WorldObject();
            var Sun = new Planet(0, Center, "Sun")
            {
                PlanetSize = 5,
                RotationAngularSpeed = 0.0f,
                TurningAngularSpeed = 1.0f,
            };
            var Mercury = new Planet(400, Center, "Mercury")
            {
                PlanetSize = 0.3f,
                RotationAngularSpeed = 5.0f,
                TurningAngularSpeed = 2.0f,
            };
            var Venus = new Planet(700, Center, "Venus")
            {
                PlanetSize = 0.9f,
                RotationAngularSpeed = 3.0f,
                TurningAngularSpeed = -1.0f,
            };
            var Earth = new Planet(1000, Center, "Earth")
            {
                PlanetSize = 1.0f,
                RotationAngularSpeed = 1.0f,
                TurningAngularSpeed = 1.0f,
            };
            var Mars = new Planet(1500, Center, "Mars")
            {
                PlanetSize = 0.5f,
                RotationAngularSpeed = 1.0f,
                TurningAngularSpeed = 0.8f,
            };

            SolarSystemGame.StartGame();
            SolarSystemGame.Dispose();
            Console.WriteLine();
            Console.WriteLine("Game finished. Press Enter.");
            Console.ReadLine();
        }

        private static void SetupInput(InputManager inputManager)
        {
            var exitAction = new KeyboardInput(Keys.Escape);
            inputManager.AddAction(exitAction, "Exit");
            inputManager.SubscribeOnAction("Exit", new ExitObject());

            var forward = new KeyboardInput(Keys.W);
            var backward = new KeyboardInput(Keys.S, -1);
            inputManager.AddAxes(new[] { forward, backward }, "Forward");

            var right = new KeyboardInput(Keys.D);
            var left = new KeyboardInput(Keys.A, -1);
            inputManager.AddAxes(new[] { right, left }, "Right");

            var up = new KeyboardInput(Keys.E);
            var down = new KeyboardInput(Keys.Q, -1);
            inputManager.AddAxes(new[] { up, down }, "Up");

            var turnUp = new KeyboardInput(Keys.Up);
            var turnDown = new KeyboardInput(Keys.Down, -1);
            inputManager.AddAxes(new[] { turnUp, turnDown }, "Turn Up");

            var turnRight = new KeyboardInput(Keys.Right);
            var turnLeft = new KeyboardInput(Keys.Left, -1);
            inputManager.AddAxes(new[] { turnRight, turnLeft }, "Turn Right");
        }
    }

    class ExitObject : IActionListener
    {
        public void OnPressed(string action)
        {
            Game.GameInstance.ExitGame();
        }

        public void OnReleased(string action) { }
    }
}
