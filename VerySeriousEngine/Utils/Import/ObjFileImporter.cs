using SharpDX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using VerySeriousEngine.Geometry;

namespace VerySeriousEngine.Utils.Importers
{
    internal class ObjFileImporter
    {

        private readonly string filePath;
        
        private readonly List<StaticMesh> importedMeshes;
        private readonly List<Vector3> vertices;
        private readonly List<Vector3> normals;
        private readonly List<Vector2> texCoords;
        private readonly List<StaticMeshFace> faces;

        private string currentMaterialName;

        private StaticMesh CurrentMesh {
            get {
                if (importedMeshes.Count == 0)
                    return null;

                return importedMeshes[importedMeshes.Count - 1];
            }
        }

        public ObjFileImporter(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            importedMeshes = new List<StaticMesh>();
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            texCoords = new List<Vector2>();
            faces = new List<StaticMeshFace>();
        }

        internal StaticMesh[] GetResult(bool reimport = false)
        {
            if (importedMeshes.Count > 0 && !reimport)
                return importedMeshes.ToArray();

            List<string> lines = new List<string>();
            // read all valid lines
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        continue;

                    line.Trim();
                    if (line.StartsWith("#") || line.Length == 0)
                        continue;

                    lines.Add(line);
                }
            }

            foreach (var line in lines)
            {
                if (line.StartsWith("v "))
                {
                    AddVertex(line);
                    continue;
                }

                if(line.StartsWith("vt "))
                {
                    AddTexCoord(line);
                    continue;
                }

                if(line.StartsWith("vn "))
                {
                    AddNormal(line);
                    continue;
                }

                if(line.StartsWith("f "))
                {
                    AddFace(line);
                    continue;
                }

                if(line.StartsWith("o ") || line.StartsWith("g "))
                {
                    StartNewMesh(line);
                    continue;
                }

                if(line.StartsWith("usemtl "))
                {
                    StartNewMaterial(line);
                    continue;
                }
            }

            FinishCurrentMesh();

            return importedMeshes.ToArray();
        }

        private void AddVertex(string line)
        {
            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            var vertex = new Vector3
            {
                X = float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat),
                Z = float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat),
            };

            vertices.Add(vertex);
        }

        private void AddNormal(string line)
        {
            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            var normal = new Vector3
            {
                X = float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat),
                Z = float.Parse(elements[3], CultureInfo.InvariantCulture.NumberFormat),
            };

            normals.Add(normal);
        }

        private void AddTexCoord(string line)
        {
            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            var texCoord = new Vector2
            {
                X = float.Parse(elements[1], CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(elements[2], CultureInfo.InvariantCulture.NumberFormat),
            };

            texCoords.Add(texCoord);
        }

        private void AddFace(string line)
        {
            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            var face = new StaticMeshFace
            {
                Vertex1 = ParseVertex(elements[1]),
                Vertex2 = ParseVertex(elements[2]),
                Vertex3 = ParseVertex(elements[3])
            };

            faces.Add(face);
        }

        private Vertex ParseVertex(string line)
        {
            var elements = line.Split('/');
            int vertexIndex = int.Parse(elements[0]) - 1;
            var result = new Vertex()
            {
                Location = vertices[vertexIndex],
                TexCoord = Vector2.Zero,
                Normal = Vector3.UnitY,
                Color = Color.Wheat.ToVector4(),
            };

            if (elements.Length > 1 && elements[1].Length > 0)
            {
                int texCoordIndex = int.Parse(elements[1]) - 1;
                result.TexCoord = texCoords[texCoordIndex];
            }
            if (elements.Length > 2 && elements[2].Length > 0)
            {
                int normalIndex = int.Parse(elements[2]) - 1;
                result.Normal = normals[normalIndex];
            }

            return result;
        }

        private void StartNewMaterial(string line)
        {
            FinishCurrentMaterial();

            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            currentMaterialName = elements[1];
        }

        private void FinishCurrentMaterial()
        {
            if (faces.Count > 0)
                CurrentMesh.AddGeometryPiece(currentMaterialName, faces);
            faces.Clear();
        }
        
        private void FinishCurrentMesh()
        {
            FinishCurrentMaterial();
        }

        private void StartNewMesh(string line)
        {
            FinishCurrentMesh();
            var elements = new List<string>(line.Split(' '));
            elements.RemoveAll(e => e.Length == 0);
            importedMeshes.Add(new StaticMesh(elements[1]));
        }
    }
}