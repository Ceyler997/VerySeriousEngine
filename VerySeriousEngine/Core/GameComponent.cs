using System;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Core
{
    //
    // Summary:
    //     Basic game component object, contains basic component logic
    public class GameComponent
    {
        private bool isActive;
        private bool shouldCallStart;

        public string ComponentName { get; }
        public GameObject Owner { get; private set; }
        public bool IsActive {
            get { return isActive; }
            set {
                if (isActive == value)
                    return;

                isActive = value;
                if (value)
                    OnActivation();
                else
                    OnDeactivation();
            }
        }

        public GameComponent(GameObject owner, string componentName = null, bool isActiveAtStart = true)
        {
            ComponentName = componentName ?? GetType().Name;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            isActive = isActiveAtStart;
            shouldCallStart = false;
            
            owner.Components.Add(this);
            if (this is IRenderable renderable)
                owner.GameWorld.Renderable.Add(renderable);
        }

        public override string ToString()
        {
            return Owner.ToString() + "." + ComponentName;
        }

        internal void InternalUpdate(float frameTime)
        {
            if (IsActive == false)
                return;

            if (shouldCallStart)
            {
                Start();
                shouldCallStart = false;
            }

            Update(frameTime);
        }

        public void Destroy()
        {
            OnDestroy();
            Owner.Components.Remove(this);
            Logger.Log(ToString() + " component destroy");
        }

        virtual public void Start()
        {
            Logger.Log(ToString() + " component start");
        }

        virtual public void Update(float frameTime) { }

        virtual public void OnActivation() { }

        virtual public void OnDeactivation() { }

        virtual public void OnDestroy() { }
    }
}
