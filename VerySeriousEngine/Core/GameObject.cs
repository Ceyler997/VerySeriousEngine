using System;
using System.Collections.Generic;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Core
{
    //
    // Summary:
    //     Basic game object, contains logic to work with children and components
    public class GameObject
    {
        private bool isActive;
        private bool shouldCallStart;

        public string ObjectName { get; }
        public GameObject Parent { get; private set; }
        public World GameWorld { get; private set; }

        public List<GameObject> Children { get; }
        public List<GameComponent> Components { get; }
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

        public GameObject(GameObject parent = null, string objectName = null, bool isActiveAtStart = true)
        {
            ObjectName = objectName ?? GetType().Name;
            Parent = parent;
            isActive = isActiveAtStart;
            shouldCallStart = true;

            Children = new List<GameObject>();
            Components = new List<GameComponent>();

            if (parent == null)
            {
                var gameInstance = Game.GameInstance;
                if (gameInstance == null)
                    throw new InvalidOperationException("No Game Instance");

                var world = gameInstance.CurrentWorld;
                if (world == null)
                    throw new InvalidOperationException("No world to attach the object");

                world.GameObjects.Add(this);
                GameWorld = world;
            }
            else
            {
                parent.Children.Add(this);
                GameWorld = parent.GameWorld;
            }

            if (this is IRenderable renderable)
                GameWorld.Renderable.Add(renderable);
        }

        public override string ToString()
        {
            string parentName;
            if (Parent != null)
                parentName = Parent.ToString();
            else
                parentName = GameWorld.WorldName;

            return parentName + "/" + ObjectName; 
        }

        public void Destroy()
        {
            while (Children.Count > 0)
                Children[0].Destroy();

            while (Components.Count > 0)
                Components[0].Destroy();

            OnDestroy();

            if (Parent == null)
                GameWorld.GameObjects.Remove(this);
            else
                Parent.Children.Remove(this);

            if (this is IRenderable renderable)
                GameWorld.Renderable.Remove(renderable);
            Logger.Log(ToString() + " object destroy");
        }

        public void InternalUpdate(float frameTime)
        {
            if (IsActive == false)
                return;

            if (shouldCallStart)
            {
                shouldCallStart = false;
                Start();
            }

            foreach (var child in Children)
                child.InternalUpdate(frameTime);

            foreach (var component in Components)
                component.InternalUpdate(frameTime);

            Update(frameTime);
        }

        virtual public void Start()
        {
            Logger.Log(ToString() + " object start");
        }

        virtual public void Update(float frameTime)
        {
            Logger.Log(ToString() + " object update");
        }

        virtual public void OnActivation()
        {
            Logger.Log(ToString() + " object activation");
        }

        virtual public void OnDeactivation()
        {
            Logger.Log(ToString() + " object deactivation");
        }

        virtual public void OnDestroy()
        {
            Logger.Log(ToString() + " object on destroy");
        }
    }
}