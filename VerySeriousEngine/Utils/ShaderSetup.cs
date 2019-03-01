using SharpDX.Direct3D11;
using VerySeriousEngine.Core;

namespace VerySeriousEngine.Utils
{
    public class ShaderSetup
    {
        public VertexShader VertexShader { get; private set; }
        public InputLayout InputLayout { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public ShaderSetup(string shaderFileName, InputElement[] inputElements, string vertexShaderEntryPoint = "VSMain", string pixelShaderEntryPoint = "PSMain")
        {
            SetupVertexShader(shaderFileName, vertexShaderEntryPoint, inputElements);
            SetupPixelShader(shaderFileName, pixelShaderEntryPoint);
        }

        public void SetupVertexShader(string shaderFileName, string entryPoint, InputElement[] inputElements)
        {
            var constructor = Game.GameInstance.GameConstructor;
            var result = constructor.CompileVertexShader(shaderFileName, entryPoint, inputElements);
            VertexShader = result.Item1;
            InputLayout = result.Item2;
        }

        public void SetupPixelShader(string shaderFileName, string entryPoint)
        {
            var constructor = Game.GameInstance.GameConstructor;
            PixelShader = constructor.CompilePixelShader(shaderFileName, entryPoint);
        }
    }
}
