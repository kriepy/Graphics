texture ScreenTexture;
 
sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
 
float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, TextureCoordinate);
	color.r = pow(color.r,1.2f);
	color.g = pow(color.g,1.2f);
	color.b = pow(color.b,1.2f);

    return color;
}
 
technique PostProcessing
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
