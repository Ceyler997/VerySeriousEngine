using SharpDX.Windows;
using System;
using System.Collections.Generic;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Core
{
    //
    // Summary:
    //     Game class, contains all logic to start a game
    public class Game : IDisposable
    {
        public static Game GameInstance { get; private set; }

        public World CurrentWorld { get; set; }
        public List<World> GameWorlds { get; }

        public Constructor GameConstructor { get; }
        public Renderer GameRenderer { get; }
        public PhysicsEngine2D Physics2D {get;}
        public TimeManager TimeManager { get; }
        public InputManager InputManager { get; }

        public string GameName { get; }
        public RenderForm Form { get; }

        private Game(string name, int windowWidth, int windowHeight, bool isWindowed)
        {
            Logger.Log("Game construction");
            SharpDX.Configuration.EnableObjectTracking = true;

            GameName = name ?? throw new ArgumentNullException(nameof(GameName));

            Form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(windowWidth, windowHeight),
            };

            GameRenderer = new Renderer(Form, isWindowed);
            GameConstructor = new Constructor(GameRenderer.Device);
            TimeManager = new TimeManager();
            InputManager = new InputManager();
            Physics2D = new PhysicsEngine2D();
            GameWorlds = new List<World>();

            TimeManager.Setup();
            SetupPhysics();
            GameRenderer.Setup(GameConstructor);
            GameRenderer.LightingModel = new RestrictedLightingModel(GameConstructor, GameRenderer);
        }

        public override string ToString()
        {
            return GameName;
        }

        public static Game CreateGame(string name, int windowWidth, int windowHeight, bool isWindowed)
        {
            Logger.Log("Creating game " + name);
            if (GameInstance != null)
            {
                Logger.LogWarning("Other game already exists");
                return null;
            }

            GameInstance = new Game(name, windowWidth, windowHeight, isWindowed);
            return GameInstance;
        }

        public void Dispose()
        {
            Logger.Log("Disposing game " + GameName);

            foreach (var world in GameWorlds)
                world.Dispose();

            GameRenderer.Dispose();
            Form.Dispose();

            if (GameInstance == this)
                GameInstance = null;
            else
                Logger.LogWarning("Game Instance is not this at Dispose");
            Logger.LogWarning(SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
        }

        public void StartGame()
        {
            Logger.Log("Starting game " + GameName);

            if (CurrentWorld == null && GameWorlds.Count > 0)
                CurrentWorld = GameWorlds[0];
            
            RenderLoop.Run(Form, GameLoop);
        }

        public void ExitGame()
        {
            Form.Close();
        }

        private void SetupPhysics()
        {
            Logger.LogWarning("Physics not inpmlemented yet");
        }

        private void GameLoop()
        {
            try
            {
                TimeManager.UpdateFrameTime();

                InputManager.Update();
                if (CurrentWorld != null)
                    CurrentWorld.Update(TimeManager.FrameTime);
                else
                    Logger.LogWarning("No Game World");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
            }
        }
    }
}
