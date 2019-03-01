using SharpDX;
using SharpDX.Direct3D11;

namespace VerySeriousEngine.Interfaces
{
    //
    // Summary:
    //     Interface, that should be implemented by all supposed to be rendered objects/components
    public interface IRenderable
    {
        bool IsRendered { get; }
        InputLayout InputLayout { get; }
        Buffer IndexBuffer { get; }
        VertexBufferBinding VertexBufferBinding { get; }
        VertexShader VertexShader { get; }
        PixelShader PixelShader { get; }
        int IndexCount { get; }
        Matrix WorldMatrix { get; }
    }
}
