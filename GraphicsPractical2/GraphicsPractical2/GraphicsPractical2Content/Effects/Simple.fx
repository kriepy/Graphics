//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define LightSource (50,50,50)

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, World2, quadTransform;
// Input variable set up front
float4 DiffuseColor, AmbientColor, SpecularColor, Move;
float AmbientIntensity, SpecularIntensity, SpecularPower;
bool Shading, Ex11, Ex12, Ex21, NormalMapping;
// Textures
Texture Texture, Mapping;

//---------------------------------- Input / Output structures ----------------------------------

// Input of the Vertex Shader
struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float3 normal : NORMAL0;
	float4 color : COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
};

// The output of the vertex shader. After being passed through the interpolator/rasterizer it is also 
// the input of the pixel shader. 
struct VertexShaderOutput
{
	float4 Position2D : POSITION0;
	float3 normal : TEXCOORD0;
	float4 Position3D : TEXCOORD1;
	float2 TextureCoordinate: TEXCOORD2;
};

//------------------------------------------ Functions ------------------------------------------

// Texture samplers
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

// The Coloring using normals assignment is implemented here
// simply returns the normals
float4 NormalColor(float3 normal)
{
	//VertexShaderOutput input = (VertexShaderInput);
	float4 color = float4(0,0,0,1);
	color.rgb = normal;
	return color;
}

// The Procedural texturing assignment is implemented here
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

// Shading is done here
float4 LambertianShading(float3 normal, float3 position, float4 Color)
{
	// The World2 matrix is used for transforming the normals properly in the same way the world is transformed
	float4x4 rotationAndScale = World2;
	normal = normalize(mul(normal, rotationAndScale));

	// The three lighting factors are calculated seperatly
	// Ambient light is just a constant
	float4 ambient = mul(AmbientIntensity,AmbientColor);

	// Diffuse light is dependant on the orientation of the incident light and the normal of the surface
	float4 diffuse = max(0,dot(normalize( normal),normalize(LightSource-position)))*Color;

	// Phong shading --Obsolete, but kept for reference
	//float3 reflection = -normalize(LightSource-position) + 2* dot(normalize(LightSource-position),normal) * normal;
	// float4 specular = SpecularColor * pow(max(0.00001f,dot(normalize(float3(0,50,100)-position),normalize(reflection))),SpecularPower) * SpecularIntensity;

	//Blinn Phong Used to calculate the specular component
	float3 half_vector = normalize(float3(0,50,100) + (LightSource-position));
	float  HdotN = max( 0.00001f, dot( half_vector,  normal) );
	float4 specular = SpecularColor * pow( HdotN, SpecularPower ) * SpecularIntensity;

	//All lighting is combined
	float4 color = ambient + diffuse + specular;
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
	worldPosition = mul(input.Position3D+Move, World);
    float4 viewPosition  = mul(worldPosition, View);

	//fill the output (simply pass certain variables)
	output.Position3D    = input.Position3D+Move;
	output.Position2D    = mul(viewPosition, Projection);
	output.normal        = input.normal;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
	//Select between coloring methods
	if (Ex11)
	{
		float4 color = NormalColor(input.normal);
		return color;
	}
	if (Ex12)
	{
		input.normal = ProceduralColor(input);
		float4 color = NormalColor(input.normal);
		return color;
	}
	else
	{
		float4 color = LambertianShading(input.normal, input.Position3D, DiffuseColor);
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

//---------------------------------------- Technique: Simple2 ----------------------------------------
// Technique used for adding texture

VertexShaderOutput Simple2VertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition;
	worldPosition = mul(input.Position3D+Move, quadTransform);
    float4 viewPosition  = mul(worldPosition, View);

	//fill the output (simply pass certain variables)
	output.Position3D    = input.Position3D+Move;
	output.Position2D    = mul(viewPosition, Projection);
	output.normal        = input.normal;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

// Textures the surface
float4 Simple2PixelShader(VertexShaderOutput input) : COLOR0
{
	if (NormalMapping)
	{
		//Uses normal mapping to add the illusion off height differences
		float4 color = tex2D(TextureSampler,input.TextureCoordinate.xy);
		float4 adjust = tex2D(BumpMap,input.TextureCoordinate.xy);
	
		//Apply shading to the model
		color = LambertianShading(normalize(input.normal+(2*adjust.xyz-1)), input.Position3D, color);

		return color;
	}
	else
	{
		//simply paste the texture on the model
		return tex2D(TextureSampler,input.TextureCoordinate.xy);
	}

}

technique Simple2
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 Simple2VertexShader();
		PixelShader  = compile ps_2_0 Simple2PixelShader();
	}
}