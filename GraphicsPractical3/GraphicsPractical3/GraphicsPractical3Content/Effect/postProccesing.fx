//Applies the after effects
//-------------------------defines----------------------------
#define GaussSize 7

//-------------------------variables----------------------------
texture ScreenTexture;

bool ApplyGray;
bool ApplyBlur;
const float Gauss[GaussSize] = 
{ 0.00082f, 0.02804f, 0.23392f, 0.47442f, 0.23392f, 0.02804f, 0.00082f };
float2 dirX = (1.0f, 0.0f);
float2 dirY = (0.0f, 1.0f);

//---------------------------------- Input / Output structures ----------------------------------

sampler TextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
 
//---------------------------------------- PostProccesing ----------------------------------------

float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, TextureCoordinate);

	if (ApplyBlur)
	{
		float4 temp;
		color = (0.0f,0.0f,0.0f,0.0f);
		//convolve the image with a 1D gaussian in the x direction
		for(int i=0; i<GaussSize; i++) {
				color += tex2D(TextureSampler, TextureCoordinate + dirX*(i-3)*(0.00125f) ) * Gauss[i];
		}
	}
	return color;  
}
 
float4 PixelShaderFunction2(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, TextureCoordinate);

	if (ApplyBlur)
	{
		float4 temp;
		color = (0.0f,0.0f,0.0f,0.0f);
		//convolve the image with a 1D gaussian in the x direction
		for(int i=0; i<GaussSize; i++) {
				color += tex2D(TextureSampler, TextureCoordinate + dirY*(i-3)*(0.00167f) ) * Gauss[i];
		}
	}
	
	if (ApplyGray)
	{
		//Convert to grayScale
		float gray = color.r*0.3 + color.g*0.59 + color.b*0.11;
		color.r = gray;
		color.g = gray;
		color.b = gray;
	}

	return color;  
}

technique PostProcessing
{
	// The first pass blurs the image in the x direction
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

	// The second pass blurs the image in the y direction optionally transforms the image to grayscale
	pass Pass1
	{
		pixelShader = compile ps_2_0 PixelShaderFunction2();
	}
}
