//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

//------------------------------------- Top Level Variables -------------------------------------

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, InvTransWorld;
float4 AmbientColor, DiffuseColor;
float3 Eye, LightSource;
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

//---------------------------------------- Technique: Cook Torrance ----------------------------------------

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
	float3 N = normalize(mul(input.normal, InvTransWorld));
	float3 E = normalize(Eye - input.Position3D);
	float3 L = normalize(LightSource - input.Position3D);
	float3 H = normalize(L+E);

	float m = 0.5f;
	float alpha = acos(dot(N,H));
	float D = exp(-pow(tan(alpha)/m,2))/(Pi*m*m*pow(cos(alpha),4));
	
	float F0 = 1.42f;
	float F = F0 + (1-F0)*pow(1-dot(E,H),5);
	
	float G = min(1, min( 2*dot(H,N)*dot(E,N)/dot(E,H) , 2*dot(H,N)*dot(L,N)/dot(E,H) ));

	float specular = D*F*G/dot(E,N);

	// The three lighting factors are calculated seperatly
	// Ambient light is just a constant
	float4 ambient = mul(AmbientIntensity,AmbientColor);

	// Diffuse light is dependant on the orientation of the incident light and the normal of the surface
	float4 diffuse = max(0,dot(N,L))*DiffuseColor;

    return ambient+diffuse+specular;
}

technique Simple
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 SimpleVertexShader();
		PixelShader  = compile ps_3_0 SimplePixelShader();
	}
}