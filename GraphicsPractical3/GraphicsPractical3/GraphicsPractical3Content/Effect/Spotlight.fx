//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265

#define falloff 1 //kan tussen 0.5 en 1.75 liggen


//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, InvTransWorld;
float4  DiffuseColor;
float3 Eye, LightSource;
float Phi, Theta;

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


//------------------------------------------ Functions ------------------------------------------





//---------------------------------------- Technique: Spotlight ----------------------------------------

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position3D, World);
    float4 viewPosition  = mul(worldPosition, View);
	output.Position2D    = mul(viewPosition, Projection);

	float3 normal = normalize(mul(input.normal, InvTransWorld));

	//float fDistance = distance( worldPosition, LightSource );
	//float fLinearAtten = lerp( 1.0f, 0.0f, fDistance / fLightRange );

	// Compute the direction to the light
    float3 vLight = normalize( LightSource - worldPosition.xyz);

	float3 LightDirection = normalize(LightSource);

	float cosAlpha      = max( 0.0f, dot( vLight, -LightDirection ) );

	float fSpotAtten = 0.0f; // default value simplifies branch:
    if( cosAlpha > Theta )
    {
        fSpotAtten = 1.0f;
    }
    else if( cosAlpha > Phi )
    {
        fSpotAtten = pow( (cosAlpha - Phi) / (Theta - Phi), falloff );
    }

	float NdotL = max( 0.0f, dot( normal, vLight ) );
	float3 dif = DiffuseColor.xyz;
	output.normal = float4(NdotL*fSpotAtten*dif,1.0f);


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
