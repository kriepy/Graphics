//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define LightSource (50,50,50)

//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World;
float4 AmbientColor, DiffuseColor;
float3 Eye;
float AmbientIntensity;

//---------------------------------- Input / Output structures ----------------------------------

struct VertexShaderInput
{
	float4 Position3D : POSITION0;
	float3 normal : NORMAL0;
	float4 color : COLOR0;
};


struct VertexShaderOutput
{
	float4 Position2D : POSITION0;
	float3 normal : TEXCOORD0;
	float4 Position3D : TEXCOORD1;
};

//------------------------------------------ Functions ------------------------------------------

// Implement the Coloring using normals assignment here
float4 NormalColor(/* parameter(s) */)
{
	return float4(1, 0, 0, 1);
}

// Implement the Procedural texturing assignment here
float4 ProceduralColor(/* parameter(s) */)
{
	return float4(0, 0, 0, 1);
}

//---------------------------------------- Technique: Simple ----------------------------------------

VertexShaderOutput SimpleVertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position3D, World);
    float4 viewPosition  = mul(worldPosition, View);
	output.Position2D    = mul(viewPosition, Projection);

	//pass variables
	output.Position3D    = input.Position3D;
	output.normal        = input.normal;

	return output;
}

float4 SimplePixelShader(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	//normal = normalize(mul(normal, rotationAndScale));

	// float3 E = normalize(Eye - input.Position3D);
	float3 L = normalize(LightSource - input.Position3D);
	//float3 H = normalize(L+E);
	float3 N = normalize(input.normal);

	//float D = arccos(
	//float F = 
	//float G = 

	// The three lighting factors are calculated seperatly
	// Ambient light is just a constant
	float4 ambient = mul(AmbientIntensity,AmbientColor);

	// Diffuse light is dependant on the orientation of the incident light and the normal of the surface
	float4 diffuse = max(0,dot(N,L))*DiffuseColor;

	

    return ambient+diffuse;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SimpleVertexShader();
		PixelShader  = compile ps_2_0 SimplePixelShader();
	}
}