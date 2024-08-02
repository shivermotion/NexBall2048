Shader "Unlit/OutlineShader"
{
    Properties
    {
        _Color ("Color", color) = (0,0,0,1)
        _Size("Outline Size", float) = 1
    }
    SubShader
    {
        Stencil
            {
                Ref 2 // Reference value for the stencil test
                Comp Greater // Comparison function
                Pass Keep // Operation to perform when the stencil test passes
            }
        
        Tags
        {
            "RenderType"="Opaque"
            "Queue" = "Geometry+6"
        }
        LOD 100
        Offset 5,5
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            float _Size;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + (v.normal * _Size * .01));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}