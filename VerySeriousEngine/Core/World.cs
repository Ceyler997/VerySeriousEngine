using SharpDX;
using System;
using System.Collections.Generic;
using VerySeriousEngine.Interfaces;
using VerySeriousEngine.Utils;

namespace VerySeriousEngine.Core
{
    //
    // Summary:
    //     World class, contains all objects on the scene with their components
    public class World : IDisposable
    {
        public List<GameObject> GameObjects { get; }
        public List<IRenderable> Renderable { get; }

        public string WorldName { get; }
        public PointOfView WorldPointOfView { get; set; }
        public bool IsValid { get; private set; }

        public World(string worldName)
        {
            IsValid = false;

            WorldName = worldName ?? throw new ArgumentNullException(nameof(worldName));
            WorldPointOfView = new PointOfView();
            GameObjects = new List<GameObject>();
            Renderable = new List<IRenderable>();

            var instance = Game.GameInstance;
            if (instance == null)
                throw new InvalidOperationException("No Game Instance");

            if (instance.GameWorlds.Find(world => world.WorldName == worldName) != null)
                throw new ArgumentException("World with this name already exists");
            
            instance.GameWorlds.Add(this);
        }

        public override string ToString()
        {
            var gameName = Game.GameInstance.ToString();
            return gameName + ": " + WorldName;
        }

        public override bool Equals(object obj)
        {
            return WorldName.Equals(obj);
        }

        public override int GetHashCode()
        {
            return WorldName.GetHashCode();
        }
        
        public void Dispose()
        {
            while (GameObjects.Count > 0)
                GameObjects[0].Destroy();
        }

        public void Update(float frameTime)
        {
            UpdateInput(frameTime);
            UpdatePhysics(frameTime);
            UpdateLogic(frameTime);
            RenderObjects(frameTime);
        }

        private void RenderObjects(float frameTime)
        {
            var renderer = Game.GameInstance.GameRenderer;
            renderer.StartFrame();
            var ViewProjectionMatrix = WorldPointOfView.ViewMatrix * WorldPointOfView.ProjectionMatrix;
            foreach (var renderable in Renderable)
            {
                if(!renderable.IsRendered)
                    continue;
                Matrix WVP = renderable.WorldMatrix * ViewProjectionMatrix;
                renderer.RenderObject(renderable, WVP);
            }
            renderer.FinishFrame();
        }

        private void UpdateLogic(float frameTime)
        {
            foreach (var gameObject in GameObjects)
                gameObject.InternalUpdate(frameTime);
        }

        private void UpdatePhysics(float frameTime)
        {
            Logger.LogWarning("Physics update not implemented");
        }

        private void UpdateInput(float frameTime)
        {
            Logger.LogWarning("Input update not implemented");
        }
    }
}