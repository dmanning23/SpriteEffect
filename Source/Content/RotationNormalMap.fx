// Effect applies normalmapped lighting to a 2D sprite.

float3 LightDirection;
float3 LightColor = 1.0;
float3 AmbientColor = 0.35;
float Rotation = 0.0;

sampler TextureSampler : register(s0);
sampler NormalSampler : register(s1)
{ 
    Texture = (NormalTexture);
};

float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	//Look up the texture value
    float4 tex = tex2D(TextureSampler, texCoord);

	//Dont do these calculations if the alpha channel is empty
	if (tex.a > 0.0)
	{
		//Look up the normalmap value
		float4 normal = 2 * tex2D(NormalSampler, texCoord) - 1.0;

		//compute the rotated light direction
		float3 rotatedLight = LightDirection;
		if (Rotation != 0.0)
		{
			float cs = cos(-Rotation);
			float sn = sin(-Rotation);

			float px = LightDirection.x * cs - LightDirection.y * sn;
			float py = LightDirection.x * sn + LightDirection.y * cs;
			rotatedLight.x = px;
			rotatedLight.y = py;
		}

		//Compute lighting.
		float lightAmount = max(dot(normal.xyz, rotatedLight), 0.0);
		color.rgb *= AmbientColor + (lightAmount * LightColor);
	}

    return tex * color;
}

technique Normalmap
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
    }
}
