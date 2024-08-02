Shader "Custom/Cel Shader 2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Stencil
            {
                Ref 3 // Reference value for the stencil test
                Comp Always // Comparison function
                Pass Replace // Operation to perform when the stencil test passes
            }
        
        Tags
        {
            "RenderType"="Opaque"
            "Queue" = "Geometry+5"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

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
    FallBack "Diffuse"
}