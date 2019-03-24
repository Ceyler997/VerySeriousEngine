using SharpDX.Direct3D11;
using System;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;

namespace VerySeriousEngine.Shaders
{
    public class Shader : IDisposable
    {
        public InputLayout InputLayout { get; private set; }
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public Shader(string shaderFileName, InputElement[] inputElements, string vertexShaderEntryPoint, string pixelShaderEntryPoint)
        {
            var constructor = Game.GameInstance.GameConstructor;
            var vertexShaderCompileResult = constructor.CompileVertexShader(shaderFileName, vertexShaderEntryPoint, inputElements);
            VertexShader = vertexShaderCompileResult.Item1;
            InputLayout = vertexShaderCompileResult.Item2;
            PixelShader = constructor.CompilePixelShader(shaderFileName, pixelShaderEntryPoint);
        }

        public void Dispose()
        {
            InputLayout.Dispose();
            VertexShader.Dispose();
            PixelShader.Dispose();
        }
    }

    public class VertexColorShader : Shader
    {
        public VertexColorShader() : base("Shaders/Code/VertexColorShader.hlsl", Vertex.InputElements, "VSMain", "PSMain")
        { }
    }
}
