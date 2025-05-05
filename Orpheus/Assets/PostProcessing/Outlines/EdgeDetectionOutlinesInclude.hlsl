// MIT License

// Copyright (c) 2020 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// #include <UnityShaderVariables.cginc>
#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// #include <UnityShaderVariables.cginc>
Texture2D _BlitTexture;

// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// The sobel effect runs by sampling the texture around a point to see
// if there are any large changes. Each sample is multiplied by a convolution
// matrix weight for the x and y components seperately. Each value is then
// added together, and the final sobel value is the length of the resulting float2.
// Higher values mean the algorithm detected more of an edge

// These are points to sample relative to the starting point
static float2 sobelSamplePoints[9] = {
    float2(0, 0), float2(0, 1), float2(0, 0),
    float2(-1, 0), float2(0, 0), float2(1, 0),
    float2(0, 0), float2(0, -1), float2(0, 0),
};

// Weights for the x component
static float sobelXMatrix[9] = {
    0, 0, 0,
    1, 0, -1,
    0, 0, 0
};

// Weights for the y component
static float sobelYMatrix[9] = {
    0, 1, 0,
    0, 0, 0,
    0, -1, 0
};

// This function runs the sobel algorithm over the depth texture
void DepthSobel_float(float2 UV, float Thickness, out float Out) {
    #if !defined(SHADERGRAPH_PREVIEW)
        
    //     float2 sobel = 0;
    //     float2 normalizedPixelSize = 1 / _ScreenParams;
    //     // We can unroll this loop to make it more efficient
    //     // The compiler is also smart enough to remove the i=4 iteration, which is always zero
    //     [unroll] for (int i = 0; i < 9; i++) {
    //         float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness * normalizedPixelSize);
    //         sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
    //     }
    //     // Get the final sobel value
    //     Out = length(sobel);

        float2 sobel = 0;
        float2 normalizedPixelSize = 1 / _ScreenParams;
        // We can unroll this loop to make it more efficient
        // The compiler is also smart enough to remove the i=4 iteration, which is always zero

        float2 uvs[4];
        uvs[0] = float2(UV.x, UV.y + normalizedPixelSize.y);
        uvs[1] = float2(UV.x, UV.y - normalizedPixelSize.y);
        uvs[2] = float2(UV.x + normalizedPixelSize.x, UV.y);
        uvs[3] = float2(UV.x - normalizedPixelSize.x, UV.y);

        float centerDepth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
        
        float depth_diff = 0;
        [unroll] for (int i = 0; i < 4; i++) {
            float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uvs[i]);
            depth_diff += centerDepth - depth;
        }
        // Get the final sobel value
        Out = depth_diff;
    #else
        Out = 1;
    #endif
}

// This function runs the sobel algorithm over the opaque texture
void ColorSobel_float(float2 UV, float Thickness, out float Out) {
    #if !defined(SHADERGRAPH_PREVIEW)
    
        // We have to run the sobel algorithm over the RGB channels separately
        float sobelR = 0;
        float sobelG = 0;
        float sobelB = 0;

        float2 normalizedPixelSize = 1 / _ScreenParams;

        float3 centerColor = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV);

        float2 uvs[4];
        uvs[0] = float2(UV.x, UV.y + normalizedPixelSize.y);
        uvs[1] = float2(UV.x, UV.y - normalizedPixelSize.y);
        uvs[2] = float2(UV.x + normalizedPixelSize.x, UV.y);
        uvs[3] = float2(UV.x - normalizedPixelSize.x, UV.y);
        
        // We can unroll this loop to make it more efficient
        // The compiler is also smart enough to remove the i=4 iteration, which is always zero
        [unroll] for (int i = 0; i < 4; i++) {
            // Sample the scene color texture
            float3 rgb = SHADERGRAPH_SAMPLE_SCENE_COLOR(uvs[i]);
            // Create the kernel for this iteration
            // Accumulate samples for each color
            sobelR += centerColor.r - rgb.r;
            sobelG += centerColor.g - rgb.g;
            sobelB += centerColor.b - rgb.b;
        }
        // Get the final sobel value
        // Combine the RGB values by taking the one with the largest sobel value
        Out = max(sobelR, max(sobelG, sobelB));
        // This is an alternate way to combine the three sobel values by taking the average
        // See which one you like better
        //Out = (length(sobelR) + length(sobelG) + length(sobelB)) / 3.0;
    #else
        Out = 1;
    #endif
}

// Sample the depth normal map and decode depth and normal from the texture
void GetDepthAndNormal(float2 uv, out float depth, out float3 normal) {
    depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);

    normal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv);
}

// A wrapper around the above function for use in a custom function node
void CalculateDepthNormal_float(float2 UV, out float Depth, out float3 Normal) {
    #if !defined(SHADERGRAPH_PREVIEW)
    
        GetDepthAndNormal(UV, Depth, Normal);
        // Normals are encoded from 0 to 1 in the texture. Remap them to -1 to 1 for easier use in the graph
        Normal = Normal * 2 - 1;
    #else
        Depth = 1;
        Normal = 1;
    #endif
}

// This function runs the sobel algorithm over the opaque texture
void NormalsSobel_float(float2 UV, float PixelThickness, out float Out) {
    #if !defined(SHADERGRAPH_PREVIEW)
        
        // We have to run the sobel algorithm over the XYZ channels separately, like color
        float3 normalSum = float3(0,0,0);

        float2 normalizedPixelSize = 1 / _ScreenParams;

        float3 centerNormal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV) * 2 - 1;

        float3 normalEdgeBias = float3(1,1,1);
        
        float2 uvs[4];
        uvs[0] = float2(UV.x, UV.y + normalizedPixelSize.y);
        uvs[1] = float2(UV.x, UV.y - normalizedPixelSize.y);
        uvs[2] = float2(UV.x + normalizedPixelSize.x, UV.y);
        uvs[3] = float2(UV.x - normalizedPixelSize.x, UV.y);
        
        // We can unroll this loop to make it more efficient
        // The compiler is also smart enough to remove the i=4 iteration, which is always zero
        [unroll] for (int i = 0; i < 4; i++) {
            float depth;
            float3 normal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uvs[i]) * 2 - 1;

            float3 normalDiff = centerNormal - normal;

            float normalBiasDiff = dot(normalEdgeBias, normalDiff);

            float normalIndicator = smoothstep(-0.01, 0.01, normalBiasDiff);
            
            normalSum += dot(normalDiff, normalDiff) * normalIndicator;
        }
        // Get the final sobel value
        // Combine the XYZ values by taking the one with the largest sobel value
        Out = normalSum;
    #else
        Out = 1;
    #endif
}

void DepthAndNormalsSobel_float(float2 UV, float NormalThreshold, float DepthThreshold, float ReverseDepthThreshold, float DarkenAmount, float LightenAmount, float3 LightDirection, float3 WorldSpaceNormal, out float3 Out, out float3 originalColor) {

    #if !defined(SHADERGRAPH_PREVIEW)
    
        float2 normalizedPixelSize = 1 / _ScreenParams;

        float centerDepth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
        float3 centerNormal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(UV) * 2 - 1;

        float3 normalEdgeBias = -LightDirection;
        
        float2 uvs[4];
        uvs[0] = float2(UV.x, min(1 - 0.001, UV.y + normalizedPixelSize.y));
        uvs[1] = float2(UV.x, max(0, UV.y - normalizedPixelSize.y));
            uvs[2] = float2(min(1 - 0.001, UV.x + normalizedPixelSize.x), UV.y);
        uvs[3] = float2(max(0, UV.x - normalizedPixelSize.x), UV.y);

        float depthDiff = 0;
        float depthDiffReversed = 0;
        float3 normalSum = float3(0,0,0);
        float nearestDepth = centerDepth;
        float2 nearestUV = UV;
        
        [unroll] for (int i = 0; i < 4; i++) {
            float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uvs[i]);

            depthDiff += centerDepth - depth;
            depthDiffReversed += depth - centerDepth;

            if (depth > nearestDepth)
            {
                nearestDepth = depth;
                nearestUV = uvs[i];
            }
            
            float3 normal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uvs[i]) * 2 - 1;

            float3 normalDiff = centerNormal - normal;

            float normalBiasDiff = dot(normalEdgeBias, normalDiff);

            float normalIndicator = smoothstep(-0.01, 0.01, normalBiasDiff);
            
            normalSum += dot(normalDiff, normalDiff) * normalIndicator;
        }

        float depthEdge = step(DepthThreshold, depthDiff);

        float reverseDepthEdge = step(ReverseDepthThreshold, depthDiffReversed);

        float indicator = sqrt(normalSum);
        float normalEdge = step(NormalThreshold, indicator - reverseDepthEdge);
        
        originalColor = LOAD_TEXTURE2D_LOD(_BlitTexture, uint2(UV * _ScreenSize.xy), 0);
        
        float3 nearestColor = LOAD_TEXTURE2D_LOD(_BlitTexture, uint2(nearestUV * _ScreenSize.xy), 0);
    

        float ld = dot(normalize(WorldSpaceNormal), normalize(LightDirection));
    
        float3 edgeMix;

        if (centerDepth > 0)
        {
            if (depthEdge > 0)
            {
                edgeMix = lerp(originalColor, nearestColor * DarkenAmount, depthEdge);
            }
            else
            {
                edgeMix = lerp(originalColor, originalColor * lerp(LightenAmount, DarkenAmount, ld), normalEdge);
            }
        }
        else
        {
            edgeMix = originalColor;
        }

        Out = edgeMix;

    #else
        //Dummy code
        Out = 1;
        originalColor = float3(1,1,1);
    #endif
}

void ViewDirectionFromScreenUV_float(float2 In, out float3 Out) {
    #if !defined(SHADERGRAPH_PREVIEW)
        
        // Code by Keijiro Takahashi @_kzr and Ben Golus @bgolus
        // Get the perspective projection
        float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
        // Convert the uvs into view space by "undoing" projection
        Out = -normalize(float3((In * 2 - 1) / p11_22, -1));
    #else
        Out = 1;
    #endif
}

#endif

