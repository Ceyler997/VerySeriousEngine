using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Shaders;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components
{
    public class SimplePointsMeshComponent : GameComponent, IRenderable
    {
        private WorldObject worldOwner;

        private readonly RenderSetup[] setup;

        public SimplePointsMeshComponent(WorldObject owner, Shader shaderSetup, Vertex[] points, int[] indices, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwner = owner;
            var constructor = Game.GameInstance.GameConstructor;

            var indexBuffer = constructor.CreateBuffer(indices, BindFlags.IndexBuffer);
            var indexCount = indices.Length;

            var vertexBuffer = constructor.CreateBuffer(points, BindFlags.VertexBuffer);
            var vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<Vertex>(), 0);
            Utils.Geometry geometrySetup = new Utils.Geometry(indexBuffer, vertexBufferBinding, indexCount);

            setup = new RenderSetup[]
            {
                new RenderSetup(shaderSetup, geometrySetup),
            };
        }

        public Matrix WorldMatrix => worldOwner.WorldTransform;

        public bool IsRendered => IsActive;

        public IEnumerable<RenderSetup> Setup => setup;
    }
}
