using System;
using SharpDX;
using VerySeriousEngine.Components;
using VerySeriousEngine.Objects;
using VerySeriousEngine.Utils;

namespace TestProject
{
    class Planet : WorldObject
    {
        private class PlanetObject : WorldObject
        {
            SimplePointsMeshComponent PlanetMesh { get; }

            public PlanetObject(Planet parentPlanet, string objectName = null, bool isActiveAtStart = true) : base(parentPlanet, objectName, isActiveAtStart)
            {
                if (parentPlanet == null)
                    throw new ArgumentNullException(nameof(parentPlanet));

                ShaderSetup shaderSetup = new ShaderSetup("Shaders/VertexColorShader.hlsl", SimplePoint.InputElements);
                var points = new SimplePoint[]
                {
                    new SimplePoint(new Vector4( 50.0f,  50.0f,  50.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f)),
                    new SimplePoint(new Vector4(-50.0f,  50.0f,  50.0f, 1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f)),
                    new SimplePoint(new Vector4( 50.0f, -50.0f,  50.0f, 1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f)),
                    new SimplePoint(new Vector4(-50.0f, -50.0f,  50.0f, 1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f)),
                    new SimplePoint(new Vector4( 50.0f,  50.0f, -50.0f, 1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f)),
                    new SimplePoint(new Vector4(-50.0f,  50.0f, -50.0f, 1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f)),
                    new SimplePoint(new Vector4( 50.0f, -50.0f, -50.0f, 1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f)),
                    new SimplePoint(new Vector4(-50.0f, -50.0f, -50.0f, 1.0f), new Color(0.0f, 0.0f, 0.0f, 1.0f)),
                };
                var indices = new int[]
                {
                    0, 1, 2, 3, 1, 2,
                    1, 3, 5, 7, 3, 5,
                    5, 4, 7, 6, 4, 7,
                    4, 0, 6, 2, 0, 6,
                    0, 1, 4, 5, 1, 4,
                    2, 3, 6, 7, 3, 6,
                };
                PlanetMesh = new SimplePointsMeshComponent(this, shaderSetup, points, indices);
            }
        }

        public Planet(WorldObject parent = null, string objectName = null, bool isActiveAtStart = true) : base(parent, objectName, isActiveAtStart)
        {
            new PlanetObject(this);
        }
    }
}
