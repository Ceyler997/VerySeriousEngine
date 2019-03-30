using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using VerySeriousEngine.Interfaces;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Core
{
    public class Renderer : IDisposable
    {
        private readonly RenderForm form;
        private readonly Device device;
        private readonly SwapChain swapChain;
        private RenderTargetView renderView;
        private DepthStencilView depthView;
        private Buffer worldTransformMatrixBuffer;

        public Device Device { get => device; }
        public DeviceContext Context { get => Device.ImmediateContext; }

        public LightingModel LightingModel { get; set; }

        public int FrameWidth { get => form.Width; }
        public int FrameHeight { get => form.Height; }

        public Renderer(RenderForm form, bool isWindowed)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));

            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(FrameWidth, FrameHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = isWindowed,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out device, out swapChain);
        }

        public void Setup(Constructor constructor)
        {
            worldTransformMatrixBuffer = constructor.CreateEmptyBuffer(Matrix.SizeInBytes * 3, BindFlags.ConstantBuffer);
            SetupInputAssembler();
            SetupVertexShader();
            SetupRasterizer();
            SetupPixelShader();
            SetupOutputMerger();            
        }

        private void SetupInputAssembler()
        {
            Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        private void SetupVertexShader()
        {
            Context.VertexShader.SetConstantBuffer(0, worldTransformMatrixBuffer);
        }

        private void SetupRasterizer()
        {
            var description = RasterizerStateDescription.Default();
            description.CullMode = CullMode.Front;
            var state = new RasterizerState(Device, description);

            Context.Rasterizer.State = state;
            Context.Rasterizer.SetViewport(new Viewport(0, 0, FrameWidth, FrameHeight));

            state.Dispose();
        }

        private void SetupPixelShader()
        {
            var description = SamplerStateDescription.Default();
            description.AddressU = TextureAddressMode.Wrap;
            description.AddressV = TextureAddressMode.Wrap;
            var state = new SamplerState(Device, description);
            Context.PixelShader.SetConstantBuffer(0, worldTransformMatrixBuffer);
            Context.PixelShader.SetSampler(0, state);
            state.Dispose();
        }

        private void SetupOutputMerger()
        {
            var backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
            renderView = new RenderTargetView(Device, backBuffer);
            backBuffer.Dispose();

            var zBuffer = new Texture2D(Device, new Texture2DDescription()
            {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = FrameWidth,
                Height = FrameHeight,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            depthView = new DepthStencilView(Device, zBuffer);
            zBuffer.Dispose();

            var description = DepthStencilStateDescription.Default();
            description.IsDepthEnabled = true;
            description.DepthComparison = Comparison.LessEqual;
            var state = new DepthStencilState(Device, description);

            Context.OutputMerger.SetDepthStencilState(state);
            Context.OutputMerger.SetTargets(depthView, renderView);

            state.Dispose();
        }

        public void StartFrame()
        {
            Context.ClearRenderTargetView(renderView, Color.Black);
            Context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            if (LightingModel != null)
                LightingModel.UpdateBuffers(this);
        }

        public void RenderObject(IRenderable renderable, ref Matrix WorldMatrix, ref Matrix ViewMatrix, ref Matrix ProjectionMatrix)
        {
            if (renderable == null)
                return;

            if (renderable.IsRendered == false)
                return;

            foreach(var piece in renderable.Setup)
            {
                if (piece == null)
                {
                    Logger.LogError("Trying to render object without setup");
                    continue;
                }

                if (piece.ShaderSetup == null)
                {
                    Logger.LogError("Trying to render object without shaders");
                    continue;
                }

                if (piece.BufferSetup == null)
                {
                    Logger.LogError("Trying to render object without geometry");
                    continue;
                }

                Matrix[] WVP = new Matrix[3] { WorldMatrix, ViewMatrix, ProjectionMatrix };
                Context.UpdateSubresource(WVP, worldTransformMatrixBuffer);
                piece.ShaderSetup.PrepareResources(this);

                Context.InputAssembler.InputLayout = piece.ShaderSetup.InputLayout;
                Context.VertexShader.Set(piece.ShaderSetup.VertexShader);
                Context.PixelShader.Set(piece.ShaderSetup.PixelShader);

                Context.InputAssembler.SetIndexBuffer(piece.BufferSetup.IndexBuffer, Format.R32_UInt, 0);
                Context.InputAssembler.SetVertexBuffers(0, piece.BufferSetup.VertexBufferBinding);


                Context.DrawIndexed(piece.BufferSetup.IndexCount, 0, 0);
            }
        }

        public void FinishFrame()
        {
            swapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            if (LightingModel != null)
                LightingModel.Dispose();
            worldTransformMatrixBuffer.Dispose();
            renderView.Dispose();
            depthView.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

    }
}
