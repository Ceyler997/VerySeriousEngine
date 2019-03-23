cbuffer ShaderData : register(b0)
{
    float4x4 worldViewProj;
};

struct VS_IN
{
    float4 position : POSITION;
    float3 normal   : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color	: COLOR;
};

struct PS_IN
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float2 texcoord : TEXCOORD;
    float4 color	: COLOR;
};

PS_IN VSMain( VS_IN input )
{
    PS_IN output = (PS_IN)0;

    output.position = mul(worldViewProj, input.position);  // WHY?
    output.color	= input.color;

    return output;
}

float4 PSMain( PS_IN input ) : SV_TARGET
{
    return input.color;
}
