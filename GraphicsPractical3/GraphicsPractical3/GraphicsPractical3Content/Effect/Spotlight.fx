//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define falloff 1 //kan tussen 0.5 en 1.75 liggen


//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, InvTransWorld;
float4  DiffuseColor;
float3 Eye, LightSource;
float Phi, Theta; //The angle of the spotlight. Theta is the inner and Phi the outer angle

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
	float4 normal : TEXCOORD0;
	float4 Position3D : TEXCOORD1;
};


//---------------------------------------- Technique: Spotlight ----------------------------------------

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position3D, World); //The WorldPosition for every vertex of the model
    float4 viewPosition  = mul(worldPosition, View); //The View-Matrix
	output.Position2D    = mul(viewPosition, Projection); // The position of every vertix on the 2D screen

	float3 normal = normalize(mul(input.normal, InvTransWorld));

	// Compute the direction to the light
    float3 Light = normalize( LightSource - worldPosition.xyz);
	float3 LightDirection = normalize(LightSource);

	float cosAlpha      = max( 0.0f, dot( Light, LightDirection ) ); // the angle between the light direction and the distanve of the vertex to the LightSource

	float SpotAtten = 0.0f;
    if( cosAlpha > Theta )
    {
		SpotAtten = 1.0f;
    }
    else if( cosAlpha > Phi )
    {
        SpotAtten = pow( (cosAlpha - Phi) / (Theta - Phi), falloff );
    }

	float NdotL = max( 0.0f, dot( normal, Light ) );
	float3 dif = DiffuseColor.xyz;
	output.normal = float4(NdotL*SpotAtten*dif,1.0f);


	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = input.normal;

	return color;
}


technique Spotlight
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}