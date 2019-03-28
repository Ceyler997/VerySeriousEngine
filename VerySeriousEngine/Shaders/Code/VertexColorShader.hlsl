cbuffer ShaderData : register(b0)
{
    float4x4 worldViewProj;
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
    PS_IN output = (PS_IN)0;
    
    float4 position = float4(input.position.x, input.position.y, input.position.z, 1.0f);
    output.position = mul(worldViewProj, position);
    output.color	= input.color;

    return output;
}

float4 PSMain( PS_IN input ) : SV_TARGET
{
    return input.color;
}
