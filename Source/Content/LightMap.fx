//Get the light map from a normal map.

sampler NormalSampler : register(s1);

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 normal = tex2D(NormalSampler, texCoord);
    return normal;
}

technique Normalmap
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}
