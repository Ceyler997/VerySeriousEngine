using System.Collections.Generic;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Shaders;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components
{
    public class StaticMeshComponent : GameComponent, IRenderable
    {
        private readonly WorldObject worldOwer;
        private StaticMesh mesh;
        private Shader defaultShader;

        public Shader DefaultShader {
            get => defaultShader;
            set {
                defaultShader = value;
                foreach(var setup in MeshSetup.Values)
                {
                    if (setup.ShaderSetup == null)
                        setup.ShaderSetup = value;
                }
            }
        }
        public readonly Dictionary<string, RenderSetup> MeshSetup;
        public StaticMesh Mesh {
            get => mesh;
            set {
                mesh = value;
                UpdateRenderSetup();
            }
        }

        public bool IsRendered => Mesh != null && IsActive;
        public IEnumerable<RenderSetup> Setup {
            get {
                UpdateRenderSetup();
                return MeshSetup.Values;
            }
        }
        public Matrix WorldMatrix => worldOwer.WorldTransform;

        public StaticMeshComponent(WorldObject owner, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwer = owner;
            MeshSetup = new Dictionary<string, RenderSetup>();
        }

        private void UpdateRenderSetup()
        {
            foreach(var key in MeshSetup.Keys)
            {
                if (!Mesh.Geometry.ContainsKey(key))
                    MeshSetup.Remove(key);
            }

            foreach(var key in Mesh.Geometry.Keys)
            {
                if (!MeshSetup.ContainsKey(key))
                    MeshSetup.Add(key, new RenderSetup(DefaultShader, Mesh.Geometry[key]));
            }
        }
    }
}
