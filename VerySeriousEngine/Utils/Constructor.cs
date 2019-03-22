using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Core
{
    public class Constructor
    {
        private readonly Device device;

        public Constructor(Device device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public Tuple<VertexShader, InputLayout> CompileVertexShader(string fileName, string entryPoint, InputElement[] inputElements)
        {
            var shaderByteCode = ShaderBytecode.CompileFromFile(fileName, entryPoint, "vs_5_0", ShaderFlags.Debug);
            var shader = new VertexShader(device, shaderByteCode);
            var layout = new InputLayout(device, shaderByteCode, inputElements);
            return new Tuple<VertexShader, InputLayout>(shader, layout);
        }

        public PixelShader CompilePixelShader(string fileName, string entryPoint)
        {
            var shaderByteCode = ShaderBytecode.CompileFromFile(fileName, entryPoint, "ps_5_0", ShaderFlags.Debug);
            return new PixelShader(device, shaderByteCode);
        }

        public Buffer CreateEmptyBuffer(int sizeInBytes, BindFlags bindFlags, ResourceUsage usage = ResourceUsage.Default)
        {
            var desc = new BufferDescription()
            {
                BindFlags = bindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = usage,
                SizeInBytes = sizeInBytes,
            };
            return new Buffer(device, desc);
        }

        public Buffer CreateBuffer<T>(T[] data, BindFlags bindFlags, ResourceUsage usage = ResourceUsage.Default) where T : struct
        {
            var desc = new BufferDescription()
            {
                BindFlags = bindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = usage,
                SizeInBytes = Marshal.SizeOf<T>() * data.Length,
            };
            return Buffer.Create<T>(device, data, desc);
        }
    }
}