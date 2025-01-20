#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

#include "DecodeDepthNormals.hlsl"

TEXTURE2D(_DepthNormalsTexture); SAMPLER(sampler_DepthNormalsTexture);

static float2 sobelSamplePoints[4] = {
    float2(-1, 1), float2(1, 1),float2(-1, -1),float2(1, -1),
};

void GetDepthAndNormal(float2 UV, out float depth, out float3 normal)
{
    float4 coded = SAMPLE_TEXTURE2D(_DepthNormalsTexture, sampler_DepthNormalsTexture, UV);
    DecodeDepthNormal(coded, depth, normal);
}

void NormalsAndDepthSobel_float(float2 UV, float Thickness, out float Normals, out float Depth)
{
    float2 sobelX = 0;
    float2 sobelY = 0;
    float2 sobelZ = 0;
    float2 sobelDepth = 0;

    [unroll] for (int i = 0; i < 4; i++)
    {
        float depth;
        float3 normal;
        GetDepthAndNormal(UV + sobelSamplePoints[i] * Thickness, depth, normal);
        float2 kernel = sobelSamplePoints[i];
        sobelX += normal.x * kernel;
        sobelY += normal.y * kernel;
        sobelZ += normal.z * kernel;
        sobelDepth += depth * kernel;
    }

    Normals = max(length(sobelX), max(length(sobelY), length(sobelZ)));
    Depth = length(sobelDepth);
}


#endif