//Get the light map from a normal map.

float3 LightDirection;

sampler NormalSampler : register(s1)
{ 
    Texture = (NormalTexture);
};

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 normal = tex2D(NormalSampler, texCoord);

	// Compute lighting.
    float lightAmount = dot(normal.xyz, LightDirection);
	color.rgb = lightAmount;

    return color;
}

technique Normalmap
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}
