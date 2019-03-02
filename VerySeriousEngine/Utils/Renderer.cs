using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using VerySeriousEngine.Core;
using VerySeriousEngine.Interfaces;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Utils
{
    public class Renderer : IDisposable
    {
        private readonly Device device;
        private readonly RenderForm form;

        private readonly SwapChain swapChain;
        private readonly RenderTargetView renderView;
        private readonly Texture2D backBuffer;

        private Buffer worldTransformMatrixBuffer;

        public Device Device { get => device; }

        public int FrameWidth { get => form.Width; }
        public int FrameHeight { get => form.Height; }

        public Renderer(RenderForm form, bool isWindowed)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));

            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.Width, form.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = isWindowed,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(4, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out device, out swapChain);

            backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
            renderView = new RenderTargetView(device, backBuffer);
        }

        public void Setup(Constructor constructor)
        {
            worldTransformMatrixBuffer = constructor.CreateEmptyBuffer(Matrix.SizeInBytes, BindFlags.ConstantBuffer);

            var context = device.ImmediateContext;

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.Rasterizer.State = new RasterizerState(
                device,
                new RasterizerStateDescription()
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.Solid,
                });
            context.Rasterizer.SetViewport(new Viewport(0, 0, form.Width, form.Height));
            context.OutputMerger.SetTargets(renderView);
        }

        public void StartFrame()
        {
            device.ImmediateContext.ClearRenderTargetView(renderView, Color.Black);
        }

        public void RenderObject(IRenderable renderable, Matrix WVP)
        {
            if (renderable == null)
                return;

            var context = device.ImmediateContext;

            context.InputAssembler.InputLayout = renderable.InputLayout;
            context.InputAssembler.SetIndexBuffer(renderable.IndexBuffer, Format.R32_UInt, 0);
            context.InputAssembler.SetVertexBuffers(0, renderable.VertexBufferBinding);

            context.UpdateSubresource(ref WVP, worldTransformMatrixBuffer);
            context.VertexShader.SetConstantBuffer(0, worldTransformMatrixBuffer);

            context.VertexShader.Set(renderable.VertexShader);
            context.PixelShader.Set(renderable.PixelShader);

            context.DrawIndexed(renderable.IndexCount, 0, 0);
        }

        public void FinishFrame()
        {
            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            worldTransformMatrixBuffer.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

    }
}
