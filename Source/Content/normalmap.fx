// Effect applies normalmapped lighting to a 2D sprite.

float3 LightDirection;
float3 LightColor = 1.0;
float3 AmbientColor = 0.35;

sampler TextureSampler : register(s0);
sampler NormalSampler : register(s1)
{ 
    Texture = (NormalTexture);
};

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	//Look up the texture value
    float4 tex = tex2D(TextureSampler, texCoord);

    //Look up the normalmap value
    float4 normal = tex2D(NormalSampler, texCoord);
    
    // Compute lighting.
    float lightAmount = max(dot(normal.xyz, LightDirection), 0.0);
    color.rgb *= AmbientColor + (lightAmount * LightColor);

    return tex * color;
}

technique Normalmap
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}
