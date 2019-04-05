using SharpDX;
using System;
using System.Windows.Forms;
using VerySeriousEngine.Components;
using VerySeriousEngine.Core;
using VerySeriousEngine.Input;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Shaders;
using VerySeriousEngine.Utils;

namespace TestProject
{
    class TestGame
    {
        static void Main(string[] args)
        {
            var gameWidth = 800;
            var gameHeight = 600;

            var solarSystemGame = Game.CreateGame("Space", gameWidth, gameHeight, true);

            var testModel = MeshImporter.ImportModelFromFile("Models/Earth/Earth.obj");
            var planet = testModel[0];
            var tableMesh = MeshImporter.ImportModelFromFile("Models/Table/table.obj")[0];

            solarSystemGame.CurrentWorld = new World("Solar System");

            SetupInput(solarSystemGame.InputManager);

            var center = new WorldObject(objectName: "System center")
            {
                WorldLocation = Vector3.Up * 400,
            };

            var CameraLocation = new Vector3(0, 1000, 2000);
            var camera = new SimpleControllableCamera(objectName: "Camera")
            {
                ForwardAxis = "Forward",
                RightAxis = "Right",
                UpAxis = "Up",
                TurnRightAxis = "Turn Right",
                TurnUpAxis = "Turn Up",
                WorldLocation = CameraLocation,
                WorldRotation = Quaternion.Invert(Quaternion.LookAtRH(CameraLocation, center.WorldLocation, Vector3.Up)),
            };

            var table = new WorldObject(objectName: "Table mesh");
            var tableComponent = new StaticMeshComponent(table)
            {
                Mesh = tableMesh,
                DefaultShader = new PhongVertexColorShader()
                {
                    AmbientReflection = .23125f,
                    DiffuseReflection = 0.2775f,
                    SpecularReflection = .773911f,
                    Shininess = 89.6f,
                },
            };

            var directionalLight = new DirectionalLightComponent(center)
            {
                Direction = Vector3.Normalize(Vector3.Down + Vector3.Right),
                Intensity = 5,
            };
            var sun = new Planet(0, planet, center, "Sun")
            {
                PlanetSize = 100,
                RotationAngularSpeed = 0.0f,
                TurningAngularSpeed = 1.0f,
            };
            var sunPointLight = new PointLightComponent(sun);
            var mercury = new Planet(400, planet, center, "Mercury")
            {
                PlanetSize = 3,
                RotationAngularSpeed = 5.0f,
                TurningAngularSpeed = 2.0f,
            };
            var venus = new Planet(700, planet, center, "Venus")
            {
                PlanetSize = 9,
                RotationAngularSpeed = 3.0f,
                TurningAngularSpeed = -1.0f,
            };
            var earth = new Planet(1000, planet, center, "Earth")
            {
                PlanetSize = 10,
                RotationAngularSpeed = 1.0f,
                TurningAngularSpeed = 1.0f,
            };
            var moon = new Planet(10, planet, earth.PlanetCenter, "Moon")
            {
                PlanetSize = 2,
                RotationAngularSpeed = 2.0f,
                TurningAngularSpeed = 1.0f,
            };
            var moonPointLight = new PointLightComponent(moon);
            var mars = new Planet(1500, planet, center, "Mars")
            {
                PlanetSize = 5f,
                RotationAngularSpeed = 1.0f,
                TurningAngularSpeed = 0.8f,
            };

            solarSystemGame.GameRenderer.LightingModel.AddDirectionalLight(directionalLight);
            solarSystemGame.GameRenderer.LightingModel.AddPointLight(sunPointLight);
            solarSystemGame.GameRenderer.LightingModel.AddPointLight(moonPointLight);
            solarSystemGame.StartGame();
            solarSystemGame.Dispose();
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
