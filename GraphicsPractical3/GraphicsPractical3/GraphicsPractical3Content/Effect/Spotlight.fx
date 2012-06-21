//------------------------------------------- Defines -------------------------------------------

#define Pi 3.14159265
#define LightSource (50,50,50)
#define Phi 0.5
#define Theta 0.2
#define falloff 1 //kan tussen 0.5 en 1.75 liggen


//------------------------------------- Top Level Variables -------------------------------------

// Top level variables can and have to be set at runtime

// Matrices for 3D perspective projection 
float4x4 View, Projection, World, InvTransWorld;
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





//---------------------------------------- Technique: Spotlight ----------------------------------------

VertexShaderOutput VertexShader(VertexShaderInput input)
{
	// Allocate an empty output struct
	VertexShaderOutput output = (VertexShaderOutput)0;

	// Do the matrix multiplications for perspective projection and the world transform
	float4 worldPosition = mul(input.Position3D, World);
    float4 viewPosition  = mul(worldPosition, View);
	output.Position2D    = mul(viewPosition, Projection);

	normal = normalize(mul(normal, rotationAndScale));

	float fDistance = distance( worldPosition, LightSource );
	float fLinearAtten = lerp( 1.0f, 0.0f, fDistance / fLightRange );

	// Compute the direction to the light
    float3 vLight = normalize( LightSource - pWorld );
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

	output.color = float4(fSpotAtten*DiffuseColor,1.0f);


	return output;
}

float4 PixelShader(VertexShaderOutput input) : COLOR0
{
	float4 color = NormalColor();

	return color;
}






technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShader();
        PixelShader = compile ps_3_0 PixelShader();
    }
}
