using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VerySeriousEngine.Core;
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

    public class StaticMesh : IDisposable
    {
        public string Name { get; set; } = "Static Mesh";

        private Dictionary<string, Utils.Geometry> geometryPieces;

        public IReadOnlyDictionary<string, Utils.Geometry> Geometry { get => geometryPieces; }

        public StaticMesh(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            geometryPieces = new Dictionary<string, Utils.Geometry>();
        }

        public void AddGeometryPiece(string name, IEnumerable<StaticMeshFace> faces)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (geometryPieces.ContainsKey(name))
                throw new ArgumentException("Mesh already contains piece named " + name);

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

            geometryPieces.Add(name, new Utils.Geometry(indexBuffer, vertexBufferBinding, indices.Count));
        }

        public bool RemoveGeometryPiece(string name)
        {
            if (!geometryPieces.ContainsKey(name))
                return false;

            geometryPieces[name].IndexBuffer.Dispose();
            geometryPieces[name].VertexBufferBinding.Buffer.Dispose();

            return geometryPieces.Remove(name);
        }

        public void Dispose()
        {
            foreach(var piece in geometryPieces.Values)
            {
                piece.IndexBuffer.Dispose();
                piece.VertexBufferBinding.Buffer.Dispose();
            }
        }
    }
}
