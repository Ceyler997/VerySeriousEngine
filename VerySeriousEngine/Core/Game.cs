using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using VerySeriousEngine.Utils;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

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
        public TimeManager TimeManager { get; }

        private readonly string gameName;
        private readonly RenderForm form;

        private Game(string name, int windowWidth, int windowHeight, bool isWindowed)
        {
            Logger.Log("Game construction");

            gameName = name ?? throw new ArgumentNullException(nameof(gameName));

            form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(windowWidth, windowHeight),
            };

            GameRenderer = new Renderer(form, isWindowed);
            GameConstructor = new Constructor(GameRenderer.Device);
            TimeManager = new TimeManager();
            GameWorlds = new List<World>();

            TimeManager.Setup();
            SetupInput();
            SetupPhysics();
            GameRenderer.Setup(GameConstructor);
        }

        public override string ToString()
        {
            return gameName;
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
            Logger.Log("Disposing game " + gameName);

            foreach (var world in GameWorlds)
                world.Dispose();

            GameRenderer.Dispose();
            form.Dispose();

            if (GameInstance == this)
                GameInstance = null;
            else
                Logger.LogWarning("Game Instance is not this at Dispose");
        }

        public void StartGame()
        {
            Logger.Log("Starting game " + gameName);

            if (CurrentWorld == null && GameWorlds.Count > 0)
                CurrentWorld = GameWorlds[0];
            
            RenderLoop.Run(form, GameLoop);
        }
        
        private void SetupInput()
        {
            Logger.LogWarning("Input setup not inpmlemented");
        }

        private void SetupPhysics()
        {
            Logger.LogWarning("Physics setup not inpmlemented");
        }

        private void GameLoop()
        {
            TimeManager.UpdateFrameTime();

            if (CurrentWorld != null)
                CurrentWorld.Update(TimeManager.FrameTime);
            else
                Logger.LogWarning("No Game World");
        }
    }
}
