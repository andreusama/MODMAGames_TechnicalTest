Shader "Stylized/URP_SimpleToon"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)

        _LightThreshold("Light Threshold", Range(0,1)) = 0.5
        _Softness("Band Softness", Range(0,0.25)) = 0.05
        _ShadowColor("Shadow Color", Color) = (0.2,0.2,0.2,1)

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(1,8)) = 4
        _RimIntensity("Rim Intensity", Range(0,1)) = 0

        [Toggle]_AlphaClip("Alpha Clipping", Float) = 0
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

        [Toggle]_Outline("Outline", Float) = 1
        _OutlineWidth("Outline Width (World)", Range(0,0.05)) = 0.01
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float _LightThreshold;
            float _Softness;
            float4 _ShadowColor;
            float4 _RimColor;
            float _RimPower;
            float _RimIntensity;
            float _AlphaClip;
            float _Cutoff;
            float _Outline;
            float _OutlineWidth;
            float4 _OutlineColor;
        CBUFFER_END

        struct ToonAppData
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
        };

        struct ToonVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 normalWS   : TEXCOORD1;
            float2 uv         : TEXCOORD2;
        };

        ToonVaryings ToonVert(ToonAppData IN)
        {
            ToonVaryings OUT;
            VertexPositionInputs vpi = GetVertexPositionInputs(IN.positionOS.xyz);
            OUT.positionCS = vpi.positionCS;
            OUT.positionWS = vpi.positionWS;
            OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
            OUT.uv         = IN.uv;
            return OUT;
        }

        float4 ToonFrag(ToonVaryings IN) : SV_Target
        {
            float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
            if (_AlphaClip > 0.5 && albedo.a < _Cutoff) discard;

            float3 N = normalize(IN.normalWS);
            Light main = GetMainLight();
            float ndl = saturate(dot(N, main.direction));

            // 1-2 band toon ramp with optional softness around threshold
            float ramp = step(_LightThreshold, ndl);
            if (_Softness > 0.0)
            {
                float a = saturate((_LightThreshold - _Softness));
                float b = saturate((_LightThreshold + _Softness));
                ramp = smoothstep(a, b, ndl);
            }

            float3 lit = lerp(_ShadowColor.rgb, albedo.rgb, ramp) * main.color.rgb;

            // Optional rim
            float3 V = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
            float rim = pow(1.0 - saturate(dot(N, V)), _RimPower) * _RimIntensity;
            lit += rim * _RimColor.rgb;

            return float4(lit, albedo.a);
        }
        ENDHLSL

        // Forward pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   ToonVert
            #pragma fragment ToonFrag
            ENDHLSL
        }

        // Simple outline (inverted hull)
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 3.0

            struct OAppData { float4 positionOS: POSITION; float3 normalOS: NORMAL; };
            struct OVaryings { float4 positionCS: SV_POSITION; };

            OVaryings OutlineVert(OAppData IN)
            {
                OVaryings OUT;
                float3 offset = (_Outline > 0.5) ? normalize(IN.normalOS) * _OutlineWidth : 0;
                float3 posOS = IN.positionOS.xyz + offset;
                OUT.positionCS = TransformObjectToHClip(posOS);
                return OUT;
            }

            float4 OutlineFrag(OVaryings IN) : SV_Target
            {
                if (_Outline < 0.5) clip(-1);
                return _OutlineColor;
            }

            #pragma vertex   OutlineVert
            #pragma fragment OutlineFrag
            ENDHLSL
        }
    }

    FallBack Off
}
