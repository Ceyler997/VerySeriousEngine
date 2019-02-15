using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Core
{
    public class Compiler
    {
        private readonly Device device;

        public Compiler(Device device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public Tuple<VertexShader, InputLayout> CompileVertexShader(string fileName, string entryPoint, InputElement[] inputElements)
        {
            var shaderByteCode = ShaderBytecode.CompileFromFile(fileName, entryPoint, "vs_5_0");
            var shader = new VertexShader(device, shaderByteCode);
            var layout = new InputLayout(device, shaderByteCode, inputElements);
            return new Tuple<VertexShader, InputLayout>(shader, layout);
        }

        public PixelShader CompilePixelShader(string fileName, string entryPoint)
        {
            var shaderByteCode = ShaderBytecode.CompileFromFile(fileName, entryPoint, "ps_5_0");
            return new PixelShader(device, shaderByteCode);
        }

        public Buffer CreateBuffer<T>(T[] data, BufferDescription description) where T : struct
        {
            return Buffer.Create<T>(device, data, description);
        }
    }
}