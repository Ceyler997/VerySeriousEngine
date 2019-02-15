using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Core
{
    public class GameObject : IDisposable
    {
        virtual public void Dispose()
        {
            throw new NotImplementedException();
        }

        virtual public IRenderable RenderComponent => throw new NotImplementedException();
    }


    public class TestObj : GameObject, IRenderable
    {

        public override IRenderable RenderComponent => this;

        public InputLayout InputLayout { get; }
        public Buffer IndexBuffer { get; }
        public VertexBufferBinding VertexBufferBinding { get; }
        public VertexShader VertexShader { get; }
        public PixelShader PixelShader { get; }
        public int IndexCount { get; }

        public TestObj(Compiler compiler)
        {
            var copilationResult = compiler.CompileVertexShader(
                "Shader.hlsl", "VSMain",
                new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                });
            VertexShader = copilationResult.Item1;
            InputLayout = copilationResult.Item2;

            PixelShader = compiler.CompilePixelShader("Shader.hlsl", "PSMain");

            var points = new[]
            {
                new Vector4(-1.0f,  1.0f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f), // upper left   (black)
                new Vector4( 1.0f,  1.0f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // upper right  (red)
                new Vector4(-1.0f, -1.0f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // lower left   (green)
                new Vector4( 1.0f, -1.0f, 0.5f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), // lower right  (white)
            };
            var vertexBufferDesc = new BufferDescription
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
            };
            var vertexBuffer = compiler.CreateBuffer(points, vertexBufferDesc);
            VertexBufferBinding = new VertexBufferBinding(vertexBuffer, Vector4.SizeInBytes * 2, 0);

            var indices = new[] { 0, 1, 2, 1, 2, 3 };
            var indexBufferDesc = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
            };
            IndexBuffer = compiler.CreateBuffer(indices, indexBufferDesc);
            IndexCount = indices.Length;
        }

        override public void Dispose()
        {

        }
    }
}
