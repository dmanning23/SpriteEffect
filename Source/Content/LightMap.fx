//Get the light map from a normal map.

float3 LightDirection;

sampler NormalSampler : register(s1)
{ 
    Texture = (NormalTexture);
};

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 normal = 2 * tex2D(NormalSampler, texCoord) - 1.0;

	// Compute lighting.
    float lightAmount = max(dot(normal.xyz, LightDirection), 0.0);
	color.rgb *= lightAmount;

    return color;
}

technique Normalmap
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}
