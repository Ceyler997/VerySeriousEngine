using SharpDX.Direct3D11;
using VerySeriousEngine.Shaders;

namespace VerySeriousEngine.Utils
{
    public class RenderSetup
    {
        public Shader ShaderSetup;
        public Geometry BufferSetup;

        public RenderSetup(Shader shaderSetup, Geometry bufferSetup)
        {
            ShaderSetup = shaderSetup;
            BufferSetup = bufferSetup;
        }
    }
    
    public class Geometry
    {
        public Buffer IndexBuffer;
        public VertexBufferBinding VertexBufferBinding;
        public int IndexCount;

        public Geometry(Buffer indexBuffer, VertexBufferBinding vertexBufferBinding, int indexCount)
        {
            IndexBuffer = indexBuffer;
            VertexBufferBinding = vertexBufferBinding;
            IndexCount = indexCount;
        }
    }
}
