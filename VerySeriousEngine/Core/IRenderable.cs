using SharpDX.Direct3D11;

namespace VerySeriousEngine.Core
{
    public interface IRenderable
    {
        InputLayout InputLayout { get; }

        Buffer IndexBuffer { get; }

        VertexBufferBinding VertexBufferBinding { get; }

        VertexShader VertexShader { get; }

        PixelShader PixelShader { get; }

        int IndexCount { get; }
    }
}