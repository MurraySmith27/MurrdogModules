#ifndef HSV_BLEND_NORMALIZED_INCLUDED
#define HSV_BLEND_NORMALIZED_INCLUDED

// ============================================================================
// HSVBlendNormalized.hlsl
// Normalized HSV blending for Unity Shader Graph (URP)
//
// Black/dark colors act as "transparent" — they don't darken the result.
// The final brightness is driven by the brightest contributor, not the average.
//
// Usage in Shader Graph:
//   1. Add a Custom Function node
//   2. Set Type to "File"
//   3. Point Source to this file
//   4. Set Function Name to one of:
//        - HSVBlendNormalized        (HSV in, HSV out)
//        - HSVBlendNormalizedRGB     (RGB in, RGB out)
//        - HSVBlendNormalizedMulti3  (3-way blend)
// ============================================================================

static const float TAU_N = 6.28318530718;

// ---------- Internal helpers ----------

float3 _NRGBtoHSV(float3 rgb)
{
    float cMax  = max(rgb.r, max(rgb.g, rgb.b));
    float cMin  = min(rgb.r, min(rgb.g, rgb.b));
    float delta = cMax - cMin;

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

   float s = (cMax > 0.00001) ? (delta / cMax) : 0.0;
    float v = cMax;

    return float3(h, s, v);
}

float3 _NHSVtoRGB(float3 hsv)
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

float _ShortestHueLerpN(float hueA, float hueB, float t)
{
    float delta = hueB - hueA;
    delta -= round(delta);
    return frac(hueA + delta * t);
}


// ============================================================================
// HSVBlendNormalized - Two-color normalized blend
// ============================================================================
//
// How it works:
//   - Each color's "presence" is its value (brightness). A black color
//     has zero presence and contributes nothing to hue or saturation.
//   - Hue is blended along the shortest path, weighted by each color's
//     chromatic strength (S * V), so dark or gray colors are ignored.
//   - Saturation is blended weighted by value, so a desaturated dark
//     color doesn't wash out a vivid bright one.
//   - Value takes the MAX of the two, scaled by T:
//       * At T=0 you get A's brightness (if A is brighter)
//       * At T=1 you get B's brightness (if B is brighter)
//       * In between, brightness never dips below either contributor's
//         weighted brightness, preventing the darkening artifact.
//
// Inputs:  HSV_A (float3), HSV_B (float3), T (float 0-1)
// Output:  Out (float3 HSV)
// ============================================================================
void HSVBlendNormalized_float(float3 HSV_A, float3 HSV_B, float T, out float3 Out)
{
    float valA = HSV_A.z;
    float valB = HSV_B.z;
    float satA = HSV_A.y;
    float satB = HSV_B.y;

    // -- Presence: how much each color should influence the result --
    // Based on value so black (V=0) has zero say
    float presenceA = valA * (1.0 - T);
    float presenceB = valB * T;
    float totalPresence = presenceA + presenceB + 0.0001;

    // Normalized blend factor (presence-weighted)
    float normT = presenceB / totalPresence;

    // -- Hue: chromatic-weighted shortest path --
    float chromaA = satA * valA;
    float chromaB = satB * valB;
    float chromaWeightA = chromaA * (1.0 - T);
    float chromaWeightB = chromaB * T;
    float totalChroma = chromaWeightA + chromaWeightB + 0.0001;
    float hueT = chromaWeightB / totalChroma;

    float outH = _ShortestHueLerpN(HSV_A.x, HSV_B.x, hueT);

    // -- Saturation: presence-weighted so dark colors don't dilute --
    float outS = lerp(satA, satB, normT);

    // -- Value: preserve the brightest contributor --
    // Each color's contributed brightness at this T
    float contributionA = valA * (1.0 - T);
    float contributionB = valB * T;
    // Take the max so neither color darkens the other
    // Then blend toward the max to keep smooth transitions
    float maxContribution = max(contributionA, contributionB);
    float avgContribution = contributionA + contributionB;
    // Lean toward max but allow some softness to avoid hard edges
    // The 0.7/0.3 split gives a smooth curve that still strongly
    // preserves brightness. Adjust to taste.
    float outV = lerp(avgContribution, maxContribution, 0.7);
    // Clamp: never exceed the brighter of the two inputs
    outV = min(outV, max(valA, valB));

    float a = 0;
    // Snap hue when achromatic
    if (outS < 0.01)
    {
        outH = (chromaA > chromaB) ? HSV_A.x : HSV_B.x;
    }

    Out = float3(outH, saturate(outS), saturate(outV));
}


// ============================================================================
// HSVBlendNormalizedRGB - Convenience: RGB in, RGB out
// ============================================================================
void HSVBlendNormalizedRGB_float(float3 RGB_A, float3 RGB_B, float T, out float3 Out)
{
    float3 hsvA = _NRGBtoHSV(RGB_A);
    float3 hsvB = _NRGBtoHSV(RGB_B);
    float3 blended;
    HSVBlendNormalized_float(hsvA, hsvB, T, blended);
    Out = _NHSVtoRGB(blended);
}

void HSVBlendNormalizedRGB_half(half3 RGB_A, half3 RGB_B, half T, out half3 Out)
{
    float3 result;
    HSVBlendNormalizedRGB_float((float3)RGB_A, (float3)RGB_B, (float)T, result);
    Out = (half3)result;
}


// ============================================================================
// HSVBlendNormalizedMulti3 - Three-color normalized blend
// ============================================================================
// Weights should sum to 1. Black inputs are effectively ignored.
void HSVBlendNormalizedMulti3_float(
    float3 HSV_A, float3 HSV_B, float3 HSV_C,
    float WeightA, float WeightB, float WeightC,
    out float3 Out)
{
    float valA = HSV_A.z;
    float valB = HSV_B.z;
    float valC = HSV_C.z;

    // Presence: value-weighted blend weights
    float pA = valA * WeightA;
    float pB = valB * WeightB;
    float pC = valC * WeightC;
    float totalP = pA + pB + pC + 0.0001;

    // Chromatic weights for hue averaging
    float cA = HSV_A.y * valA * WeightA;
    float cB = HSV_B.y * valB * WeightB;
    float cC = HSV_C.y * valC * WeightC;
    float totalC = cA + cB + cC + 0.0001;

    // Circular mean hue
    float sumSin = cA * sin(HSV_A.x * TAU_N)
                 + cB * sin(HSV_B.x * TAU_N)
                 + cC * sin(HSV_C.x * TAU_N);
    float sumCos = cA * cos(HSV_A.x * TAU_N)
                 + cB * cos(HSV_B.x * TAU_N)
                 + cC * cos(HSV_C.x * TAU_N);

    float outH = frac(atan2(sumSin, sumCos) / TAU_N);

    // Saturation: presence-weighted
    float outS = (pA * HSV_A.y + pB * HSV_B.y + pC * HSV_C.y) / totalP;

    // Value: preserve max brightness
    float contribA = valA * WeightA;
    float contribB = valB * WeightB;
    float contribC = valC * WeightC;
    float maxContrib = max(contribA, max(contribB, contribC));
    float sumContrib = contribA + contribB + contribC;
    float outV = lerp(sumContrib, maxContrib, 0.7);
    outV = min(outV, max(valA, max(valB, valC)));

    // Snap hue when achromatic
    if (outS < 0.01)
    {
        float maxC = max(cA, max(cB, cC));
        outH = (maxC == cA) ? HSV_A.x : ((maxC == cB) ? HSV_B.x : HSV_C.x);
    }

    Out = float3(outH, saturate(outS), saturate(outV));
}

void HSVBlendNormalizedMulti3_half(
    half3 HSV_A, half3 HSV_B, half3 HSV_C,
    half WeightA, half WeightB, half WeightC,
    out half3 Out)
{
    float3 result;
    HSVBlendNormalizedMulti3_float(
        (float3)HSV_A, (float3)HSV_B, (float3)HSV_C,
        (float)WeightA, (float)WeightB, (float)WeightC,
        result);
    Out = (half3)result;
}


// ============================================================================
// HSVBlendNormalizedMax - Strict max-brightness variant
// ============================================================================
// If you want NO darkening at all (pure max behavior instead of the
// soft 70/30 blend), use this. The tradeoff is slightly harder
// transitions when both colors are bright.
void HSVBlendNormalizedMax_float(float3 HSV_A, float3 HSV_B, float T, out float3 Out)
{
    float valA = HSV_A.z;
    float valB = HSV_B.z;

    float chromaA = HSV_A.y * valA;
    float chromaB = HSV_B.y * valB;
    float chromaWeightA = chromaA * (1.0 - T);
    float chromaWeightB = chromaB * T;
    float totalChroma = chromaWeightA + chromaWeightB + 0.0001;
    float hueT = chromaWeightB / totalChroma;

    float presenceA = valA * (1.0 - T);
    float presenceB = valB * T;
    float totalPresence = presenceA + presenceB + 0.0001;
    float normT = presenceB / totalPresence;

    float outH = _ShortestHueLerpN(HSV_A.x, HSV_B.x, hueT);
    float outS = lerp(HSV_A.y, HSV_B.y, normT);

    // Strict max: brightness is always the brightest contributor
    float outV = max(valA * (1.0 - T), valB * T);

    if (outS < 0.01)
    {
        outH = (chromaA > chromaB) ? HSV_A.x : HSV_B.x;
    }

    float overlap = 1.0 - abs(T - 0.5) * 2.0; // 0 at edges, 1 at peak overlap
    float boostStrength = 1; // adjust to taste, 0 = no boost, 1 = full boost
    float boost = 1.0 + overlap * boostStrength;
 
    outV = saturate(outV * boost);
    outS = saturate(outS * boost);
    float hueDist = abs(HSV_B.x - HSV_A.x);
    hueDist = min(hueDist, 1.0 - hueDist); // shortest arc distance, 0-0.5
    
    // Remap: 0 at small distances, 1 at opposite hues
    float hueConflict = saturate(hueDist * 2.0); // 0-1
    
    // Dip saturation at the overlap when hues disagree
    float desat = 1.0 - (hueConflict * overlap);
    outS *= desat;
    

    Out = float3(outH, saturate(outS), outV);
}
#endif // HSV_BLEND_NORMALIZED_INCLUDED