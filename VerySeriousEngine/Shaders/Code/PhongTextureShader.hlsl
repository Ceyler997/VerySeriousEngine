cbuffer MergedShaderBuffer : register(b0)
{
    float4x4 worldViewProj;
};

cbuffer PixelShaderBuffer : register(b1)
{
    float specularRefl;
    float diffuseRefl;
    float ambientRefl;
    float shininessRefl;
};

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
    float3 normal : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color : COLOR;
};


PS_IN VSMain(VS_IN input)
{
    PS_IN output;

    float4 position = float4(input.position.x, input.position.y, input.position.z, 1.0f);
    output.position = mul(worldViewProj, position);
    output.normal = input.normal;
    output.texcoord = input.texcoord;
    output.color = input.color;

    return output;
}

float4 PSMain(PS_IN input) : SV_TARGET
{
    return diffuse.Sample(textureSampler, input.texcoord);
}
