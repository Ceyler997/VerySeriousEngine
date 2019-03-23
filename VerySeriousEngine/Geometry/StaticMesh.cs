using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VerySeriousEngine.Core;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Geometry
{
    public struct Vertex
    {
        public Vector3 Location;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Color;

        public static InputElement[] InputElements = new InputElement[]
        {
            new InputElement("POSITION",    0, Format.R32G32B32_Float,      0, 0),
            new InputElement("NORMAL",      0, Format.R32G32B32_Float,      Vector3.SizeInBytes, 0),
            new InputElement("TEXCOORD",    0, Format.R32G32_Float,         Vector3.SizeInBytes * 2, 0),
            new InputElement("COLOR",       0, Format.R32G32B32A32_Float,   Vector3.SizeInBytes * 2 + Vector2.SizeInBytes, 0),
        };
    }

    public struct StaticMeshFace
    {
        public Vertex Vertex1;
        public Vertex Vertex2;
        public Vertex Vertex3;
    }

    public class StaticMesh : IRenderable
    {
        public string Name { get; set; } = "Static Mesh";

        private Dictionary<GeometrySetup, string> geometryPieces;
        private ShaderSetup shader;

        public ShaderSetup Shader {
            get => shader;
            set {
                if (shader == value)
                    return;
                shader = value;
                foreach (var piece in geometryPieces)
                    piece.Key.ShaderSetup = value;
            }
        }

        public bool IsRendered => true;
        public Matrix WorldMatrix => Matrix.Identity;

        public IEnumerable<GeometrySetup> Geometry { get => geometryPieces.Keys; }

        public StaticMesh(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            geometryPieces = new Dictionary<GeometrySetup, string>();
        }

        public void AddGeometryPiece(IEnumerable<StaticMeshFace> faces, string name)
        {
            var constructor = Game.GameInstance.GameConstructor;
            var vertices = new List<Vertex>();
            var indices = new List<int>();

            int indexCount = 0;
            foreach(var face in faces)
            {
                vertices.Add(face.Vertex1);
                indices.Add(indexCount++);
                vertices.Add(face.Vertex2);
                indices.Add(indexCount++);
                vertices.Add(face.Vertex3);
                indices.Add(indexCount++);
            }

            var vertexBuffer = constructor.CreateBuffer(vertices.ToArray(), BindFlags.VertexBuffer);
            var vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Marshal.SizeOf<Vertex>(), 0);
            var indexBuffer = constructor.CreateBuffer(indices.ToArray(), BindFlags.IndexBuffer);
            BufferSetup geometrySetup = new BufferSetup(indexBuffer, vertexBufferBinding, indices.Count);

            geometryPieces.Add(new GeometrySetup(Shader, geometrySetup), name);
        }
    }
}
