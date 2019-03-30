cbuffer MB_WorldProjection : register(b0)
{
    float4x4 worldTransform;
    float4x4 viewTransform;
    float4x4 projectionTransform;
};

cbuffer PSB_LightSources : register(b1)
{
    float4 directionalLight;
    float4 pointLights[8];
};

cbuffer PSB_MaterialSetup : register(b2)
{
    float specularRefl;
    float diffuseRefl;
    float ambientRefl;
    float shininess;
};

cbuffer PSB_Camera : register(b3)
{
    float4 cameraLocation;
}

Texture2D diffuse : register(ps, t0);
SamplerState textureSampler : register(ps, s0);

struct VS_IN
{
    float3 position : POSITION;
    float3 normal : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color : COLOR;
};

struct PS_IN
{
    float4 position : SV_POSITION;
    float3 localPosition : LOCPOSITION;
    float3 normal : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color : COLOR;
};

float GetDirectionalSourceIntensity(float3 sourceDirection, float lightIntensity, float3 pointLocation, float3 pointLocalNormal)
{

    float4 normalVector = float4(pointLocalNormal.xyz, 0.0f);
    normalVector = mul(normalVector, worldTransform);
    float3 pointNormal = normalize(normalVector.xyz);
    
    float3 reflectionDirection = reflect(sourceDirection, pointNormal);
    
    float3 cameraDirection = normalize(cameraLocation.xyz - pointLocation);
    
    float diffuse = lightIntensity * diffuseRefl * dot(sourceDirection * -1, pointNormal); // -1 because it should be direction to the source
    diffuse = max(0, diffuse);
    float specular = lightIntensity * specularRefl * pow(max(0, dot(reflectionDirection, cameraDirection)), shininess);
    specular = max(0, specular);
    return specular + diffuse;
}

float SourceIntensity(PS_IN meshPoint)
{
    return GetDirectionalSourceIntensity(directionalLight.xyz, directionalLight.w, meshPoint.localPosition.xyz, meshPoint.normal);
}

PS_IN VSMain(VS_IN input)
{
    PS_IN output;
    float4x4 worldViewProj = mul(worldTransform, mul(viewTransform, projectionTransform));

    float4 position = float4(input.position.xyz, 1.0f);
    output.position = mul(position, worldViewProj);
    output.localPosition = input.position;
    output.normal = input.normal;
    output.texcoord = input.texcoord;
    output.color = input.color;

    return output;
}

float4 PSMain(PS_IN input) : SV_TARGET
{
    return diffuse.Sample(textureSampler, input.texcoord) * (SourceIntensity(input) + ambientRefl);
}
