using System;
using SharpDX;
using SharpDX.Direct3D11;
using VerySeriousEngine.Components;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Core
{
    public abstract class LightingModel : IDisposable
    {
        public abstract bool AddPointLight(PointLightComponent pointLight);
        public abstract bool RemovePointLight(PointLightComponent pointLight);

        public abstract bool AddDirectionalLight(DirectionalLightComponent directionalLight);
        public abstract bool RemoveDirectionalLight(DirectionalLightComponent directionalLight);

        public abstract void UpdateBuffers(Renderer renderer);
        public abstract void Dispose();
    }

    public class RestrictedLightingModel : LightingModel
    {
        private readonly Vector4[] lightSources;
        private readonly Buffer lightingBuffer;

        private DirectionalLightComponent directionalLight;
        private readonly PointLightComponent[] pointLights;

        public RestrictedLightingModel(Constructor constructor, Renderer renderer)
        {
            lightSources = new Vector4[9];
            pointLights = new PointLightComponent[8];
            lightingBuffer = constructor.CreateBuffer(lightSources, BindFlags.ConstantBuffer);
            renderer.Context.PixelShader.SetConstantBuffer(1, lightingBuffer);
        }

        public override bool AddDirectionalLight(DirectionalLightComponent directionalLight)
        {
            if (this.directionalLight != null)
                return false;

            if (directionalLight == null)
                return false;

            this.directionalLight = directionalLight;
            return true;
        }

        public override bool RemoveDirectionalLight(DirectionalLightComponent directionalLight)
        {
            if (directionalLight == null)
                return false;

            if (this.directionalLight != directionalLight)
                return false;

            this.directionalLight = null;
            lightSources[0] = Vector4.Zero;
            return true;
        }

        public override bool AddPointLight(PointLightComponent pointLight)
        {
            var freeIndex = Array.FindIndex(pointLights, source => source == null);

            if (freeIndex == -1)
                return false;

            pointLights[freeIndex] = pointLight;
            return true;
        }

        public override bool RemovePointLight(PointLightComponent pointLight)
        {
            var sourceIndex = Array.FindIndex(pointLights, source => source == pointLight);

            if (sourceIndex == -1)
                return false;

            pointLights[sourceIndex] = null;
            return true;
        }

        public override void UpdateBuffers(Renderer renderer)
        {
            if(directionalLight != null)
            {
                lightSources[0].X = directionalLight.Direction.X;
                lightSources[0].Y = directionalLight.Direction.Y;
                lightSources[0].Z = directionalLight.Direction.Z;
                lightSources[0].W = directionalLight.Intensity;
            }

            for (int i = 0; i < pointLights.Length; ++i)
            {
                if (pointLights[i] != null)
                {
                    lightSources[i + 1].X = pointLights[i].WorldOwner.WorldLocation.X;
                    lightSources[i + 1].Y = pointLights[i].WorldOwner.WorldLocation.Y;
                    lightSources[i + 1].Z = pointLights[i].WorldOwner.WorldLocation.Z;
                    lightSources[i + 1].W = directionalLight.Intensity;
                }
                else
                    lightSources[i + 1] = Vector4.Zero;
            }

            Game.GameInstance.GameRenderer.Context.UpdateSubresource(lightSources, lightingBuffer);
        }

        public override void Dispose()
        {
            lightingBuffer.Dispose();
        }
    }
}
