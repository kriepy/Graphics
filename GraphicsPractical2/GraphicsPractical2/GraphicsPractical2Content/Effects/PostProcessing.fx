texture ScreenTexture;

float Gamma;

sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
 
float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, TextureCoordinate);
	color.r = pow(color.r,Gamma);
	color.g = pow(color.g,Gamma);
	color.b = pow(color.b,Gamma);

    return color;
}
 
technique PostProcessing
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
