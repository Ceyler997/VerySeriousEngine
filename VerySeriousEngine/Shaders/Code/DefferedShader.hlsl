struct PS_IN
{
    float4 position : SV_POSITION;
    float2 tex: TEXCOORD;
};

Texture2D color;
SamplerState textureSampler : register(s0);
Texture2D normal;
SamplerState normalSampler : register(s1);

PS_IN VSMain( uint id: SV_VertexID )
{
    PS_IN output;
	
	output.tex = float2((id << 1) & 2, id & 2);
	output.tex = output.tex / 2.0f;
	output.position = float4(output.tex * float2(2, -2) + float2(-1, 1), 0, 1);

    return output;
}

float4 PSMain( PS_IN input ) : SV_TARGET
{
	return color.Sample(textureSampler, input.tex);
}
