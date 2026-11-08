// Tepet's Pyramid - Arena dentro de la vasija.
// La arena se corta a una altura (_Fill 0..1 entre _MinY y _MaxY, en espacio del objeto).
// Las caras traseras dibujan la "tapa" de arena plana, asi el corte se ve solido.
// Uso: un mesh de relleno (cilindro / forma interior de la vasija) dentro de la vasija con este material.
// El juego solo tiene que cambiar _Fill (MaterialPropertyBlock o el componente VasijaArenaVisual).
Shader "Tepet/ArenaVasija"
{
    Properties
    {
        _Fill ("Llenado (0 vacia - 1 llena)", Range(0, 1)) = 0.5
        _MinY ("Altura minima (objeto)", Float) = -1
        _MaxY ("Altura maxima (objeto)", Float) = 1
        [MainTexture] _SandTex ("Textura de arena", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 2
        _TexStrength ("Fuerza de la textura", Range(0, 1)) = 0.45
        [MainColor] _SandColor ("Color arena (costados)", Color) = (0.78, 0.62, 0.40, 1)
        _SandTopColor ("Color arena (superficie)", Color) = (0.90, 0.75, 0.50, 1)
        _NoiseScale ("Ondulacion escala", Float) = 6
        _NoiseStrength ("Ondulacion altura", Range(0, 0.1)) = 0.015
        _EdgeDarken ("Oscurecer borde", Range(0, 1)) = 0.35
        _EdgeWidth ("Ancho borde", Range(0.001, 0.2)) = 0.04
        _Glint ("Brillo de granos", Range(0, 2)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_SandTex); SAMPLER(sampler_SandTex);
        CBUFFER_START(UnityPerMaterial)
            float4 _SandTex_ST;
            float _Fill, _MinY, _MaxY, _Tiling;
            float4 _SandColor, _SandTopColor;
            float _NoiseScale, _NoiseStrength, _EdgeDarken, _EdgeWidth, _Glint, _TexStrength;
        CBUFFER_END

        float Hash21(float2 p) { p = frac(p * float2(123.34, 456.21)); p += dot(p, p + 45.32); return frac(p.x * p.y); }
        float ValueNoise(float2 p)
        {
            float2 i = floor(p), f = frac(p);
            float a = Hash21(i), b = Hash21(i + float2(1, 0)), c = Hash21(i + float2(0, 1)), d = Hash21(i + float2(1, 1));
            float2 u = f * f * (3 - 2 * f);
            return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
        }
        // altura del corte en espacio del objeto, con una ondulacion suave
        float CutHeight(float3 posOS)
        {
            float h = lerp(_MinY, _MaxY, saturate(_Fill));
            h += (ValueNoise(posOS.xz * _NoiseScale) - 0.5) * 2 * _NoiseStrength;
            return h;
        }
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float2 uv         : TEXCOORD3;
                float  fog        : TEXCOORD4;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                VertexPositionInputs p = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = p.positionCS;
                o.positionWS = p.positionWS;
                o.positionOS = v.positionOS.xyz;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                o.fog = ComputeFogFactor(p.positionCS.z);
                return o;
            }

            half3 Shade(float3 posWS, float3 n, half3 albedo)
            {
                float4 sc = TransformWorldToShadowCoord(posWS);
                Light ml = GetMainLight(sc);
                half3 col = albedo * SampleSH(n);
                col += albedo * ml.color * saturate(dot(n, ml.direction)) * ml.shadowAttenuation * ml.distanceAttenuation;
                #if defined(_ADDITIONAL_LIGHTS)
                uint count = GetAdditionalLightsCount();
                for (uint li = 0u; li < count; ++li)
                {
                    Light l = GetAdditionalLight(li, posWS);
                    col += albedo * l.color * saturate(dot(n, l.direction)) * l.distanceAttenuation * l.shadowAttenuation;
                }
                #endif
                return col;
            }

            half4 frag(Varyings i, bool front : SV_IsFrontFace) : SV_Target
            {
                float h = CutHeight(i.positionOS);
                clip(h - i.positionOS.y);

                // textura proyectada desde arriba (para la tapa) y de costado (para las paredes)
                float3 objScale = float3(length(UNITY_MATRIX_M._m00_m10_m20), length(UNITY_MATRIX_M._m01_m11_m21), length(UNITY_MATRIX_M._m02_m12_m22));
                float3 p = i.positionOS * objScale * _Tiling;
                half3 texTop  = SAMPLE_TEXTURE2D(_SandTex, sampler_SandTex, p.xz).rgb;
                half3 texSide = SAMPLE_TEXTURE2D(_SandTex, sampler_SandTex, float2(atan2(p.z, p.x) * 0.5, p.y)).rgb;
                // la textura del piso es oscura: se mezcla con blanco para que el color lo defina _SandColor
                texTop  = lerp(1.0.xxx, texTop * 2.2, _TexStrength);
                texSide = lerp(1.0.xxx, texSide * 2.2, _TexStrength);

                half3 albedo; float3 n;
                if (front)
                {
                    albedo = texSide * _SandColor.rgb;
                    n = normalize(i.normalWS);
                    // borde superior mas oscuro (arena humeda / sombra del labio)
                    float edge = saturate((h - i.positionOS.y) / _EdgeWidth);
                    albedo *= lerp(1 - _EdgeDarken, 1, edge);
                }
                else
                {
                    // cara trasera = superficie de la arena, plana hacia arriba
                    albedo = texTop * _SandTopColor.rgb;
                    n = TransformObjectToWorldNormal(float3(0, 1, 0));
                    float g = step(0.985, Hash21(floor(p.xz * 90)));
                    albedo += g * _Glint * 0.25;
                }
                half3 col = Shade(i.positionWS, n, albedo);
                col = MixFog(col, i.fog);
                return half4(col, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            Cull Off
            ZWrite On ZTest LEqual ColorMask 0

            HLSLPROGRAM
            #pragma vertex vertS
            #pragma fragment fragS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            float3 _LightDirection;
            struct A { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct V { float4 positionCS : SV_POSITION; float3 positionOS : TEXCOORD0; };
            V vertS(A v)
            {
                V o;
                float3 ws = TransformObjectToWorld(v.positionOS.xyz);
                float3 nws = TransformObjectToWorldNormal(v.normalOS);
                o.positionCS = TransformWorldToHClip(ApplyShadowBias(ws, nws, _LightDirection));
                #if UNITY_REVERSED_Z
                    o.positionCS.z = min(o.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    o.positionCS.z = max(o.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                o.positionOS = v.positionOS.xyz;
                return o;
            }
            half4 fragS(V i) : SV_Target { clip(CutHeight(i.positionOS) - i.positionOS.y); return 0; }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            Cull Off
            ZWrite On ColorMask R

            HLSLPROGRAM
            #pragma vertex vertD
            #pragma fragment fragD
            struct A { float4 positionOS : POSITION; };
            struct V { float4 positionCS : SV_POSITION; float3 positionOS : TEXCOORD0; };
            V vertD(A v) { V o; o.positionCS = TransformObjectToHClip(v.positionOS.xyz); o.positionOS = v.positionOS.xyz; return o; }
            half4 fragD(V i) : SV_Target { clip(CutHeight(i.positionOS) - i.positionOS.y); return 0; }
            ENDHLSL
        }
    }
    FallBack Off
}
