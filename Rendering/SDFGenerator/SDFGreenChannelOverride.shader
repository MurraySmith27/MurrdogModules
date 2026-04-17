Shader "Hidden/SDFGreenChannelOverride"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        ZTest Always ZWrite Off Cull Off
        ColorMask G

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 frag(v2f_img i) : SV_Target
            {
                return fixed4(0, 1, 0, 0);
            }
            ENDCG
        }
    }
}
