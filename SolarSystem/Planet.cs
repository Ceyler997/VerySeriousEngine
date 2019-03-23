using System;
using SharpDX;
using VerySeriousEngine.Components;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace TestProject
{
    class Planet : WorldObject
    {
        private class PlanetObject : WorldObject
        {
            public float RotationSpeed { get; set; }
            public Vector3 RotationAxis { get; set; } = Vector3.Up;

            private readonly StaticMeshComponent planetMesh;

            public PlanetObject(Planet parentPlanet, StaticMesh mesh, string objectName = null, bool isActiveAtStart = true) : base(parentPlanet, objectName, isActiveAtStart)
            {
                if (parentPlanet == null)
                    throw new ArgumentNullException(nameof(parentPlanet));

                planetMesh = new StaticMeshComponent(this)
                {
                    Mesh = mesh,
                };
            }

            public override void Update(float frameTime)
            {
                Rotation *= Quaternion.RotationAxis(RotationAxis, frameTime * RotationSpeed);
            }
        }

        private float planetSize = 1.0f;

        // General scale
        public float PlanetSize {
            get => planetSize;
            set {
                planetSize = value;
                PlanetMesh.WorldScale = new Vector3(value);
            }
        }

        // How fast planet is rotating around world axis
        public float RotationAngularSpeed { get; set; }

        // How fast planet is rotating around it's own axis
        public float TurningAngularSpeed {
            get => PlanetMesh.RotationSpeed;
            set => PlanetMesh.RotationSpeed = value;
        }
        public Vector3 PlanetTurningAxis {
            get => PlanetMesh.RotationAxis;
            set => PlanetMesh.RotationAxis = value;
        }
        private PlanetObject PlanetMesh { get; }
        public WorldObject PlanetCenter { get => PlanetMesh; }

        public Planet(float distanceFromCenter, StaticMesh mesh, WorldObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            PlanetMesh = new PlanetObject(this, mesh)
            {
                Location = Vector3.ForwardRH * distanceFromCenter
            };
        }

        public override void Update(float frameTime)
        {
            Rotation *= Quaternion.RotationAxis(Vector3.Up, frameTime * RotationAngularSpeed);
        }
    }
}
