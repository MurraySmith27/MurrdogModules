#ifndef HSV_BLEND_INCLUDED
#define HSV_BLEND_INCLUDED

// ============================================================================
// HSVBlend.hlsl
// Stable HSV color blending for Unity Shader Graph (URP)
//
// Usage in Shader Graph:
//   1. Add a Custom Function node
//   2. Set Type to "File"
//   3. Point Source to this file
//   4. Set Function Name to one of:
//        - HSVBlend
//        - HSVBlendMulti (for 3 or 4 color blends)
//        - RGBtoHSV / HSVtoRGB (standalone converters)
// ============================================================================

// ---------- Helpers ----------

static const float TAU = 6.28318530718;

float3 _RGBtoHSV(float3 rgb)
{
    float cMax  = max(rgb.r, max(rgb.g, rgb.b));
    float cMin  = min(rgb.r, min(rgb.g, rgb.b));
    float delta = cMax - cMin;

    // Hue
    float h = 0.0;
    if (delta > 0.00001)
    {
        if (cMax == rgb.r)
            h = frac((rgb.g - rgb.b) / (delta * 6.0));
        else if (cMax == rgb.g)
            h = (rgb.b - rgb.r) / (delta * 6.0) + (1.0 / 3.0);
        else
            h = (rgb.r - rgb.g) / (delta * 6.0) + (2.0 / 3.0);
        h = frac(h);
    }

    // Saturation
    float s = (cMax > 0.00001) ? (delta / cMax) : 0.0;

    // Value
    float v = cMax;

    return float3(h, s, v);
}

float3 _HSVtoRGB(float3 hsv)
{
    float h = frac(hsv.x) * 6.0;
    float s = saturate(hsv.y);
    float v = saturate(hsv.z);

    float c = v * s;
    float x = c * (1.0 - abs(fmod(h, 2.0) - 1.0));
    float m = v - c;

    float3 rgb;
    if      (h < 1.0) rgb = float3(c, x, 0);
    else if (h < 2.0) rgb = float3(x, c, 0);
    else if (h < 3.0) rgb = float3(0, c, x);
    else if (h < 4.0) rgb = float3(0, x, c);
    else if (h < 5.0) rgb = float3(x, 0, c);
    else              rgb = float3(c, 0, x);

    return rgb + m;
}

// Shortest-path hue lerp (hues in 0-1 range)
float _ShortestHueLerp(float hueA, float hueB, float t)
{
    float delta = hueB - hueA;
    delta -= round(delta); // maps to -0.5..0.5
    return frac(hueA + delta * t);
}

// Chromatic weight: how "meaningful" the hue is for a given color
float _ChromaticWeight(float s, float v)
{
    return s * v;
}


// ============================================================================
// HSVBlend - Blend two HSV colors with stable hue interpolation
// ============================================================================
// Inputs:  HSV_A (float3), HSV_B (float3), T (float 0-1)
// Output:  Out (float3 HSV)
//
// Hue is interpolated along the shortest path around the color wheel,
// weighted by each color's chromatic strength (S*V) so that desaturated
// or dark colors don't pollute the hue.
// Saturation and Value are interpolated linearly.
// ============================================================================
void HSVBlend_float(float3 HSV_A, float3 HSV_B, float T, out float3 Out)
{
    float weightA = _ChromaticWeight(HSV_A.y, HSV_A.z);
    float weightB = _ChromaticWeight(HSV_B.y, HSV_B.z);

    // Bias hue T toward the more chromatic color
    float hueT = weightB / (weightA + weightB + 0.0001);
    // Blend the bias with the user T so the overall progression is respected
    // At T=0 we want hueA, at T=1 we want hueB, but the midpoint favors
    // whichever color has stronger chroma
    float blendedHueT = lerp(0.0, 1.0, T) * lerp(1.0, hueT / max(T, 0.0001), saturate(weightA + weightB));
    // Simplified: just remap T using the chromatic ratio
    blendedHueT = lerp(T, hueT, 0.5 * saturate(1.0 - min(weightA, weightB) * 4.0));

    float outH = _ShortestHueLerp(HSV_A.x, HSV_B.x, blendedHueT);
    float outS = lerp(HSV_A.y, HSV_B.y, T);
    float outV = lerp(HSV_A.z, HSV_B.z, T);

    // Snap hue when saturation is negligible to prevent drift
    if (outS < 0.01)
    {
        outH = (weightA > weightB) ? HSV_A.x : HSV_B.x;
    }

    Out = float3(outH, outS, outV);
}

// Half precision variant (required by Shader Graph)
void HSVBlend_half(half3 HSV_A, half3 HSV_B, half T, out half3 Out)
{
    float3 result;
    HSVBlend_float((float3)HSV_A, (float3)HSV_B, (float)T, result);
    Out = (half3)result;
}


// ============================================================================
// HSVBlendRGB - Convenience: takes RGB in, returns RGB out
// ============================================================================
void HSVBlendRGB_float(float3 RGB_A, float3 RGB_B, float T, out float3 Out)
{
    float3 hsvA = _RGBtoHSV(RGB_A);
    float3 hsvB = _RGBtoHSV(RGB_B);
    float3 blended;
    HSVBlend_float(hsvA, hsvB, T, blended);
    Out = _HSVtoRGB(blended);
}

void HSVBlendRGB_half(half3 RGB_A, half3 RGB_B, half T, out half3 Out)
{
    float3 result;
    HSVBlendRGB_float((float3)RGB_A, (float3)RGB_B, (float)T, result);
    Out = (half3)result;
}


// ============================================================================
// HSVBlendMulti3 - Blend 3 HSV colors using circular hue averaging
// ============================================================================
// Weights should sum to 1 (e.g. barycentric coordinates)
void HSVBlendMulti3_float(
    float3 HSV_A, float3 HSV_B, float3 HSV_C,
    float WeightA, float WeightB, float WeightC,
    out float3 Out)
{
    // Circular mean for hue
    float cA = _ChromaticWeight(HSV_A.y, HSV_A.z);
    float cB = _ChromaticWeight(HSV_B.y, HSV_B.z);
    float cC = _ChromaticWeight(HSV_C.y, HSV_C.z);

    float wA = WeightA * cA;
    float wB = WeightB * cB;
    float wC = WeightC * cC;
    float totalW = wA + wB + wC + 0.0001;

    float sumSin = wA * sin(HSV_A.x * TAU)
                 + wB * sin(HSV_B.x * TAU)
                 + wC * sin(HSV_C.x * TAU);
    float sumCos = wA * cos(HSV_A.x * TAU)
                 + wB * cos(HSV_B.x * TAU)
                 + wC * cos(HSV_C.x * TAU);

    float outH = frac(atan2(sumSin, sumCos) / TAU);
    float outS = WeightA * HSV_A.y + WeightB * HSV_B.y + WeightC * HSV_C.y;
    float outV = WeightA * HSV_A.z + WeightB * HSV_B.z + WeightC * HSV_C.z;

    // Snap when achromatic
    if (outS < 0.01)
    {
        float maxW = max(wA, max(wB, wC));
        outH = (maxW == wA) ? HSV_A.x : ((maxW == wB) ? HSV_B.x : HSV_C.x);
    }

    Out = float3(outH, outS, outV);
}

void HSVBlendMulti3_half(
    half3 HSV_A, half3 HSV_B, half3 HSV_C,
    half WeightA, half WeightB, half WeightC,
    out half3 Out)
{
    float3 result;
    HSVBlendMulti3_float(
        (float3)HSV_A, (float3)HSV_B, (float3)HSV_C,
        (float)WeightA, (float)WeightB, (float)WeightC,
        result);
    Out = (half3)result;
}


// ============================================================================
// Standalone converters for use in other Custom Function nodes
// ============================================================================
void RGBtoHSV_float(float3 RGB, out float3 Out)
{
    Out = _RGBtoHSV(RGB);
}

void RGBtoHSV_half(half3 RGB, out half3 Out)
{
    Out = (half3)_RGBtoHSV((float3)RGB);
}

void HSVtoRGB_float(float3 HSV, out float3 Out)
{
    Out = _HSVtoRGB(HSV);
}

void HSVtoRGB_half(half3 HSV, out half3 Out)
{
    Out = (half3)_HSVtoRGB((float3)HSV);
}

#endif // HSV_BLEND_INCLUDED
