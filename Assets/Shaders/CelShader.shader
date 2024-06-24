Shader "Custom/ExaggeratedCelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range (0.005, 0.1)) = 0.03
    }
    SubShader
    {
        Tags {"Queue" = "Geometry"}

        // Outline Pass
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            Offset 5,5

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            uniform float _OutlineThickness;
            uniform float4 _OutlineColor;

            v2f vert(appdata_t v)
            {
                // make a copy of incoming vertex data but scaled according to normal direction
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex + float4(v.normal * _OutlineThickness, 0));
                o.color = _OutlineColor;
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                return i.color;
            }
            ENDCG
        }

        // Base Pass
        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        fixed4 _Color;
        half _Metallic;
        half _Glossiness;

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the texture and apply color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            c.rgb = floor(c.rgb * 4) / 4; // Quantize the color for cel shading
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            // Apply normal map
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
        }

        // Toon lighting function
        half4 LightingLambert(SurfaceOutput s, half3 lightDir, half atten)
        {
            // Toon lighting calculation
            half NdotL = dot(s.Normal, lightDir);
            if (NdotL > 0.5)
                NdotL = 1.0;
            else if (NdotL > 0.0)
                NdotL = 0.5;
            else
                NdotL = 0.2;

            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
            c.a = s.Alpha;
            return c;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
