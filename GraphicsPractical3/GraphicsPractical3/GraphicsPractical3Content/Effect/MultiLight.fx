//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define LightSource (0,0,10)
#define LightSource2 (-10,20,-80)

//------------------------------------- Top Level Variables -------------------------------------
float4x4 World, View, Projection, InvTransWorld;
float4 DiffuseColor,LightColor, LightColor2;

// TODO: add effect parameters here.

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

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = normalize(mul(input.normal, InvTransWorld));
	float3 L = normalize(LightSource - input.Position3D);
	float3 L2 = normalize(LightSource2 - input.Position3D);

	float4 diffuse = max(0,dot(N,L))*LightColor*DiffuseColor;
	float4 diffuse2 = max(0,dot(N,L2))*LightColor2*DiffuseColor;
    // TODO: add your pixel shader code here.
    return diffuse+diffuse2;
}

technique MultiLight
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
