using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using Device = SharpDX.Direct3D11.Device;

namespace VerySeriousEngine.Core
{
    public class Game : IDisposable
    {
        public List<GameObject> GameObjects { get; }
        public Compiler Compiler { get; }
        public float FrameTime { get; private set; }

        private readonly RenderForm form;
        private readonly Device device;
        private readonly SwapChain swapChain;
        private readonly Texture2D backBuffer;
        private readonly RenderTargetView renderView;

        public Game(string name, int windowWidth, int windowHeight, bool isWindowed)
        {
            GameObjects = new List<GameObject>();

            form = new RenderForm("")
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

            Compiler = new Compiler(device);

            SetupRender(windowWidth, windowHeight);
            SetupInput();
        }

        public void StartGame()
        {
            RenderLoop.Run(form, GameLoop);
        }

        public void Dispose()
        {
            foreach (var gameObject in GameObjects)
                gameObject.Dispose();

            renderView.Dispose();
            backBuffer.Dispose();
            swapChain.Dispose();
            device.Dispose();
            form.Dispose();
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

        private void SetupInput()
        {

        }

        private void GameLoop()
        {
            device.ImmediateContext.ClearRenderTargetView(renderView, Color.Black);

            UpdateFrameTime();
            CollectInput(FrameTime);
            UpdateObjects(FrameTime);
            RenderObjects(FrameTime);
            
            swapChain.Present(1, PresentFlags.None);
        }

        private void CollectInput(float frameTime)
        {
            // Collect and handle input keys
        }

        private void RenderObjects(float frameTime)
        {
            var context = device.ImmediateContext;
            foreach(var gameObject in GameObjects)
            {
                var renderComponent = gameObject.RenderComponent;
                if (renderComponent == null)
                    continue;

                context.InputAssembler.InputLayout = renderComponent.InputLayout;
                context.InputAssembler.SetIndexBuffer(renderComponent.IndexBuffer, Format.R32_UInt, 0);
                context.InputAssembler.SetVertexBuffers(0, renderComponent.VertexBufferBinding);
                context.VertexShader.Set(renderComponent.VertexShader);
                context.PixelShader.Set(renderComponent.PixelShader);
                context.DrawIndexed(renderComponent.IndexCount, 0, 0);
            }
        }

        private void UpdateObjects(float frameTime)
        {
            foreach (var gameObject in GameObjects)
            {
                // update
            }
        }

        private void UpdateFrameTime()
        {

        }
    }
}
