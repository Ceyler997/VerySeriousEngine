using SharpDX;
using System.Collections.Generic;
using VerySeriousEngine.Core;
using VerySeriousEngine.Objects;

namespace VerySeriousEngine.Components.Physics2D
{
    public delegate void OnOverlapEvent(Physics2DComponent thisComponent, Physics2DComponent otherComponent);

    public abstract class Physics2DComponent : GameComponent
    {
        public float Depth { get => worldOwner.WorldLocation.Z; }
        public Vector2 Location { get => new Vector2(worldOwner.WorldLocation.X, worldOwner.WorldLocation.Y); }
        public float Radius { get; }

        public event OnOverlapEvent OnOverlapBegin;
        public event OnOverlapEvent OnOverlap;
        public event OnOverlapEvent OnOverlapEnd;

        protected WorldObject worldOwner;
        protected List<Physics2DComponent> overlappedComponents;

        public Physics2DComponent(WorldObject owner, string componentName = null, bool isActiveAtStart = true) : base(owner, componentName, isActiveAtStart)
        {
            worldOwner = owner;
            owner.GameWorld.Physics2DComponents.Add(this);
        }

        public abstract bool IsOverlappedWith(Physics2DComponent other);

        public void UpdateOverlapWith(Physics2DComponent other)
        {
            if (IsActive == false)
            {
                if (overlappedComponents.Count > 0)
                {
                    foreach (var component in overlappedComponents)
                        component.OnOverlapEnd(this, component);

                    overlappedComponents.Clear();
                }

                return;
            }

            bool isOverlapped = IsOverlappedWith(other);

            if(isOverlapped)
            {
                if (overlappedComponents.Contains(other))
                    OnOverlap(this, other);
                else
                {
                    overlappedComponents.Add(other);
                    OnOverlapBegin(this, other);
                }
            }
            else
            {
                if(overlappedComponents.Contains(other))
                {
                    overlappedComponents.Remove(other);
                    OnOverlapEnd(this, other);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            worldOwner.GameWorld.Physics2DComponents.Remove(this);
        }
    }
}
