cbuffer ShaderData : register(b0)
{
    float4x4 worldTransform;
    float4x4 viewTransform;
    float4x4 projectionTransform;
};

struct VS_IN
{
    float3 position : POSITION;
    float3 normal   : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color	: COLOR;
};

struct PS_IN
{
    float4 position : SV_POSITION;
    float4 color	: COLOR;
};

PS_IN VSMain( VS_IN input )
{
    PS_IN output;
    float4x4 worldViewProj = mul(worldTransform, mul(viewTransform, projectionTransform));
    
    float4 position = float4(input.position.xyz, 1.0f);
    output.position = mul(position, worldViewProj);
    output.color	= input.color;

    return output;
}

float4 PSMain( PS_IN input ) : SV_TARGET
{
    return input.color;
}
