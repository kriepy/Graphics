//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define LightSource (50,50,50)

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, World2, quadTransform;
float4 DiffuseColor, AmbientColor, SpecularColor, Move;
float AmbientIntensity, SpecularIntensity, SpecularPower;
bool Shading;
Texture Texture, Mapping;

//---------------------------------- Input / Output structures ----------------------------------

// Each member of the struct has to be given a "semantic", to indicate what kind of data should go in
// here and how it should be treated. Read more about the POSITION0 and the many other semantics in 
// the MSDN library
struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float3 normal : NORMAL0;
	float4 color : COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
};

// The output of the vertex shader. After being passed through the interpolator/rasterizer it is also 
// the input of the pixel shader. 
// Note 1: The values that you pass into this struct in the vertex shader are not the same as what 
// you get as input for the pixel shader. A vertex shader has a single vertex as input, the pixel 
// shader has 3 vertices as input, and lets you determine the color of each pixel in the triangle 
// defined by these three vertices. Therefor, all the values in the struct that you get as input for 
// the pixel shaders have been linearly interpolated between there three vertices!
// Note 2: You cannot use the data with the POSITION0 semantic in the pixel shader.
struct VertexShaderOutput
{
	float4 Position2D : POSITION0;
	float3 normal : TEXCOORD0;
	float4 Position3D : TEXCOORD1;
	float2 TextureCoordinate: TEXCOORD2;
};

//------------------------------------------ Functions ------------------------------------------

// Texture sampler
sampler2D TextureSampler = 
sampler_state
{
    Texture = Texture;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

sampler2D BumpMap = 
sampler_state
{
    Texture = Mapping;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

// Implement the Coloring using normals assignment here
float4 NormalColor(float3 normal)
{
	//VertexShaderOutput input = (VertexShaderInput);
	//float4 color;
	//color.rgb = normal.xyz
	float4 color = float4(0,0,0,1);
	color.rgb = normal;
	return color;
}

// Implement the Procedural texturing assignment here
float3 ProceduralColor(VertexShaderOutput input)
{
	if (sin(Pi*input.Position3D.x/0.15)>0)
	{
		if (sin(Pi*input.Position3D.y/0.15)>0)
		{
			return input.normal;
		}
		else
		{
			return -input.normal;
		}
	}
	else
	{
		if (sin(Pi*input.Position3D.y/0.15)>0)
		{
			return -input.normal;
		}
		else
		{
			return input.normal;
		}
	}
}

float4 LambertianShading(float3 normal, float3 position, float4 Color)
{
	float4x4 rotationAndScale = World;
	normal = normalize(normal);
	float4 ambient = mul(AmbientIntensity,AmbientColor);
	float4 diffuse = max(0,dot(normalize(mul(rotationAndScale, normal)),normalize(LightSource-position)))*Color;
	float3 reflection = -normalize(LightSource-position) + 2* dot(normalize(LightSource-position),normal) * normal;
	float4 specular = 0;//SpecularColor * pow(max(0,dot(normalize(float3(0,50,100)-position),normalize(reflection))),SpecularPower) * SpecularIntensity;

	float4 color = diffuse+ambient+specular;
	color.a = 1;
	return color;
}

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition;
	if (Shading) {worldPosition = mul(input.Position3D+Move, World);}
	else         {worldPosition = mul(input.Position3D+Move, quadTransform);}
    float4 viewPosition  = mul(worldPosition, View);

	//fill the output
	output.Position3D    = input.Position3D+Move;
	output.Position2D    = mul(viewPosition, Projection);
	output.normal        = input.normal;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	if (Shading) {
	//input.normal = ProceduralColor(input);
	//float4 color = NormalColor(input.normal);
	float4 color = LambertianShading(input.normal, input.Position3D, DiffuseColor);
	return color;}
	else{
	 float4 color = tex2D(TextureSampler,input.TextureCoordinate.xy);
	 float4 adjust = tex2D(BumpMap,input.TextureCoordinate.xy);
	 color = LambertianShading(input.normal+(2*adjust.xyz-1), input.Position3D, color);
	 return color;
	}
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader  = compile ps_2_0 SimplePixelShader();
	}
}