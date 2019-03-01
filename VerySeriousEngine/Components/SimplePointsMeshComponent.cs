using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using VerySeriousEngine.Core;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Components
{
    public class SimplePointsMeshComponent : GameComponent, IRenderable
    {
        private WorldObject worldOwner;
        private readonly Buffer indexBuffer;
        private readonly int indexCount;
        private readonly Buffer vertexBuffer;
        private readonly VertexBufferBinding vertexBufferBinding;

        private readonly VertexShader vertexShader;
        private readonly InputLayout inputLayout;
        private readonly PixelShader pixelShader;

        public SimplePointsMeshComponent(WorldObject owner, ShaderSetup shaderSetup, SimplePoint[] points, int[] indices, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwner = owner;

            var constructor = Game.GameInstance.GameConstructor;

            if(shaderSetup == null)
                throw new ArgumentNullException(nameof(shaderSetup));

            vertexShader = shaderSetup.VertexShader;
            pixelShader = shaderSetup.PixelShader;
            inputLayout = shaderSetup.InputLayout;

            indexBuffer = constructor.CreateBuffer(indices, BindFlags.IndexBuffer);
            indexCount = indices.Length;

            vertexBuffer = constructor.CreateBuffer(points, BindFlags.VertexBuffer);
            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<SimplePoint>(), 0);
        }

        public InputLayout InputLayout => inputLayout;

        public Buffer IndexBuffer => indexBuffer;

        public VertexBufferBinding VertexBufferBinding => vertexBufferBinding;

        public VertexShader VertexShader => vertexShader;

        public PixelShader PixelShader => pixelShader;

        public int IndexCount => indexCount;

        public Matrix WorldMatrix => worldOwner.TransformComponent.WorldTransform;

        public bool IsRendered => IsActive;
    }
}
