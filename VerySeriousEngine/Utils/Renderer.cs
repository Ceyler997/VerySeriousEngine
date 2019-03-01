using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using VerySeriousEngine.Interfaces;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Utils
{
    public class Renderer
    {
        private readonly Device device;
        private readonly RenderTargetView renderView;
        private readonly SwapChain swapChain;
        private readonly Buffer worldTransformMatrixBuffer;

        public Renderer(Device device, RenderTargetView renderView, SwapChain swapChain, SharpDX.Direct3D11.Buffer worldTransformMatrixBuffer)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.renderView = renderView ?? throw new ArgumentNullException(nameof(renderView));
            this.swapChain = swapChain ?? throw new ArgumentNullException(nameof(swapChain));
            this.worldTransformMatrixBuffer = worldTransformMatrixBuffer ?? throw new ArgumentNullException(nameof(worldTransformMatrixBuffer));
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

    }
}
