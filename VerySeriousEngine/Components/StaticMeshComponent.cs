using System.Collections.Generic;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Components
{
    public class StaticMeshComponent : GameComponent, IRenderable
    {
        private readonly WorldObject worldOwer;

        public StaticMesh Mesh { get; set; }

        public StaticMeshComponent(WorldObject owner, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwer = owner;
        }

        public bool IsRendered => Mesh != null && Mesh.IsRendered;

        public IEnumerable<GeometrySetup> Geometry => Mesh?.Geometry;

        public Matrix WorldMatrix => worldOwer.WorldTransform;
    }
}
