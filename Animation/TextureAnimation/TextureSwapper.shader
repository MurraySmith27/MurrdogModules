Shader "Unlit/TextureSwapper"
{
    Properties
    {
        _MainTex ("Texture Array", 2DArray) = "" {}
        _SliceRange ("Slice", Range(0,16)) = 6
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog
            #pragma require 2darray

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTex);
            float4 _MainTex_ST;
            float _SliceRange;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.uv.z = _SliceRange;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 color = UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, color);
                color *= _Color;
                return color;
            }
            ENDCG
        }
    }
}
