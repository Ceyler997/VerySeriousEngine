using SharpDX.Direct3D11;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;

namespace VerySeriousEngine.Utils
{
    public class GeometrySetup
    {
        public ShaderSetup ShaderSetup;
        public BufferSetup BufferSetup;

        public GeometrySetup(ShaderSetup shaderSetup, BufferSetup bufferSetup)
        {
            ShaderSetup = shaderSetup;
            BufferSetup = bufferSetup;
        }
    }

    public class ShaderSetup
    {
        public InputLayout InputLayout { get; private set; }
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public ShaderSetup(VertexShader vertexShader, InputLayout inputLayout, PixelShader pixelShader)
        {
            VertexShader = vertexShader;
            InputLayout = inputLayout;
            PixelShader = pixelShader;
        }

        public ShaderSetup(string shaderFileName, string vertexShaderEntryPoint = "VSMain", string pixelShaderEntryPoint = "PSMain")
        {
            var constructor = Game.GameInstance.GameConstructor;
            var vertexShaderCompileResult = constructor.CompileVertexShader(shaderFileName, vertexShaderEntryPoint, Vertex.InputElements);
            VertexShader = vertexShaderCompileResult.Item1;
            InputLayout = vertexShaderCompileResult.Item2;
            PixelShader = constructor.CompilePixelShader(shaderFileName, pixelShaderEntryPoint);
        }

        public ShaderSetup(string shaderFileName, InputElement[] inputElements, string vertexShaderEntryPoint = "VSMain", string pixelShaderEntryPoint = "PSMain")
        {
            var constructor = Game.GameInstance.GameConstructor;
            var vertexShaderCompileResult = constructor.CompileVertexShader(shaderFileName, vertexShaderEntryPoint, inputElements);
            VertexShader = vertexShaderCompileResult.Item1;
            InputLayout = vertexShaderCompileResult.Item2;
            PixelShader = constructor.CompilePixelShader(shaderFileName, pixelShaderEntryPoint);
        }
    }

    public class BufferSetup
    {
        public Buffer IndexBuffer;
        public VertexBufferBinding VertexBufferBinding;
        public int IndexCount;

        public BufferSetup(Buffer indexBuffer, VertexBufferBinding vertexBufferBinding, int indexCount)
        {
            IndexBuffer = indexBuffer;
            VertexBufferBinding = vertexBufferBinding;
            IndexCount = indexCount;
        }
    }
}
