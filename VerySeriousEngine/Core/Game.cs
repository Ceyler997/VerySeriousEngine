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
        public float FrameTime { get; private set; }

        private readonly string gameName;

        private readonly RenderForm form;
        private readonly Device device;
        private readonly SwapChain swapChain;
        private readonly Texture2D backBuffer;
        private readonly RenderTargetView renderView;
        private readonly Buffer worldTransformMatrixBuffer;

        private Game(string name, int windowWidth, int windowHeight, bool isWindowed)
        {
            Logger.Log("Game construction");

            gameName = name ?? throw new ArgumentNullException(nameof(gameName));

            form = new RenderForm(name)
            {
                ClientSize = new System.Drawing.Size(windowWidth, windowHeight),
            };

            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(windowWidth, windowHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = isWindowed,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(4, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out device, out swapChain);

            backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
            renderView = new RenderTargetView(device, backBuffer);

            GameConstructor = new Constructor(device);
            worldTransformMatrixBuffer = GameConstructor.CreateEmptyBuffer(Matrix.SizeInBytes, BindFlags.ConstantBuffer);
            GameRenderer = new Renderer(device, renderView, swapChain, worldTransformMatrixBuffer);

            SetupInput();
            SetupPhysics();
            SetupRender(windowWidth, windowHeight);

            GameWorlds = new List<World>();
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

            worldTransformMatrixBuffer.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            swapChain.Dispose();
            device.Dispose();
            form.Dispose();

            GameInstance = null;
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

        private void SetupRender(int windowWidth, int windowHeight)
        {
            var context = device.ImmediateContext;

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.Rasterizer.State = new RasterizerState(
                device,
                new RasterizerStateDescription()
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.Solid,
                });
            context.Rasterizer.SetViewport(new Viewport(0, 0, windowWidth, windowHeight));
            context.OutputMerger.SetTargets(renderView);
        }

        private void GameLoop()
        {
            UpdateFrameTime();

            if (CurrentWorld != null)
                CurrentWorld.Update(FrameTime);
            else
                Logger.LogWarning("No Game World");
        }

        private void UpdateFrameTime()
        {
            Logger.LogWarning("Frame time not implemented");
        }

    }
}
