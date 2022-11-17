Shader "Custom/Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _DissolveTexture ("DissolveTexture", 2D) = "white" {}
        _DissolveAmount ("DissolveAmount", range(0,1)) = 0
        _DissolveOutline ("Dissolve Outline", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Fade"
        }
        LOD 200
        Cull Off
        //Blend ScrAlpha OneMinusScrAlpha
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        sampler2D _DissolveTexture;
        half _DissolveAmount; //half a float
        fixed4 _DissolveOutline;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            half dissolveValue = tex2D(_DissolveTexture, IN.uv_MainTex).r;
            clip(dissolveValue - _DissolveAmount);
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = _DissolveOutline.rgb * step(dissolveValue - _DissolveAmount, 0.05f);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}