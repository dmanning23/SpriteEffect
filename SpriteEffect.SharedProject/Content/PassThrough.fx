sampler TextureSampler : register(s0);

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    return tex*color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}