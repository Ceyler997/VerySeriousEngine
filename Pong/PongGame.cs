using System;
using System.Windows.Forms;
using SharpDX;
using VerySeriousEngine.Components;
using VerySeriousEngine.Core;
using VerySeriousEngine.Input;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace Pong
{
    class PongGame
    {
        static void Main(string[] args)
        {
            var pong = Game.CreateGame("Pong", 800, 600, true);
            var world = new World("Pong World");
            pong.CurrentWorld = world;

            SetupInput(pong.InputManager);

            var camera = new SimpleControllableCamera(objectName: "Camera");
            camera.CameraComponent.IsPerspective = false;
            camera.CameraComponent.OrthoWidth = 800;
            camera.CameraComponent.OrthoHeight = 600;

            var platformLeft = new Platform(20, 100, Color.Red, objectName: "Left Platform")
            {
                WorldLocation = new Vector3(-350, 0, -100),
            };
            var platformRight = new Platform(20, 100, Color.Blue, objectName: "Right Platform")
            {
                WorldLocation = new Vector3(350, 0, -100),
            };
            var controller = new PongPlayerController(platformRight)
            {
                ExitAction = "Exit",
                UpAxis = "Up",
            };

            pong.StartGame();
            pong.Dispose();
            Console.WriteLine();
            Console.WriteLine("Game finished. Press Enter.");
            Console.ReadLine();
        }

        private static void SetupInput(InputManager inputManager)
        {
            var exitAction = new KeyboardInput(Keys.Escape);
            inputManager.AddAction(exitAction, "Exit");

            var up = new KeyboardInput(Keys.Up);
            var down = new KeyboardInput(Keys.Down, -1);
            inputManager.AddAxis(up, "Up");
            inputManager.AddAxis(down, "Up");
        }
    }
}
