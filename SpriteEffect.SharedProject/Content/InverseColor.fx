sampler TextureSampler : register(s0);

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    return float4(1.0 - tex.rgb, 1.0) * color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}