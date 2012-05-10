using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical2
{
    public struct Material
    {
        // Color of the ambient light
        public Color AmbientColor;
        // Intensity of the ambient light
        public float AmbientIntensity;
        // The color of the surface (can be ignored if texture is used, or not if you want to blend)
        public Color DiffuseColor;
        // The texture of the surface
        public Texture DiffuseTexture;
        // The normal displacement texture
        public Texture NormalMap;
        // The normal displacement factor, to apply the effect in a subtle manner
        public float DisplacementFactor;
        // Color of the specular highlight (mostly equal to the color of the light source)
        public Color SpecularColor;
        // The intensity factor of the specular highlight, controls it's size
        public float SpecularIntensity;
        // The power term of the specular highlight, controls it's smoothness
        public float SpecularPower;
        // Special surface color, use normals as color
        public bool NormalColoring;
        // Special surface color, procedural colors
        public bool ProceduralColoring;

        // Using this function requires all these elements to be present as top-level variables in the shader code. Comment out the ones that you don't use
        public void SetEffectParameters(Effect effect)
        {
            effect.Parameters["AmbientColor"].SetValue(AmbientColor.ToVector4());
            effect.Parameters["AmbientIntensity"].SetValue(AmbientIntensity);
            effect.Parameters["DiffuseColor"].SetValue(DiffuseColor.ToVector4());
            effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);
            effect.Parameters["NormalMap"].SetValue(NormalMap);
            effect.Parameters["DisplacementFactor"].SetValue(DisplacementFactor);
            effect.Parameters["SpecularColor"].SetValue(SpecularColor.ToVector4());
            effect.Parameters["SpecularIntensity"].SetValue(SpecularIntensity);
            effect.Parameters["SpecularPower"].SetValue(SpecularPower);
            effect.Parameters["NormalColoring"].SetValue(NormalColoring);
            effect.Parameters["ProceduralColoring"].SetValue(ProceduralColoring);

            effect.Parameters["HasTexture"].SetValue(DiffuseTexture != null);
            effect.Parameters["HasNormalMap"].SetValue(NormalMap != null);
        }
    }
}