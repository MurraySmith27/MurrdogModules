//UNITY_SHADER_NO_UPGRADE
#ifndef SAMPLE_TEXTURE_ARRAY_SHADER_INCLUDED
#define SAMPLE_TEXTURE_ARRAY_SHADER_INCLUDED

void SampleTexture2DArray_float(UnityTexture2DArray myTextureArray, float1 myIndex, float2 myUV, UnitySamplerState mySampler, float1 myLOD, out float4 rgbaOut)
{
    rgbaOut = SAMPLE_TEXTURE2D_ARRAY_LOD(myTextureArray, mySampler, myUV, myIndex, myLOD);
}

#endif //SAMPLE_TEXTURE_ARRAY_SHADER_INCLUDED