// Tepet's Pyramid - Piso de arena continuo para la sala de la esfinge.
// Las texturas se proyectan en coordenadas de MUNDO (no por baldosa), asi no hay cortes entre baldosas.
// El relieve y el tono cambian suave por la sala: mas arena contra las paredes y esquinas,
// camino del tunel a las placas mas pisado, y una variacion de ruido para que no se vea repetido.
Shader "Tepet/PisoArena"
{
    Properties
    {
        [MainTexture] _BaseMap ("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [Normal] _BumpMap ("Normal", 2D) = "bump" {}
        _BumpScale ("Fuerza normal (base)", Float) = 1.3
        _ParallaxMap ("Altura", 2D) = "gray" {}
        _Parallax ("Relieve maximo", Range(0, 0.08)) = 0.06
        _OcclusionMap ("Oclusion", 2D) = "white" {}
        _OcclusionStrength ("Fuerza oclusion", Range(0, 1)) = 1
        _Smoothness ("Suavidad", Range(0, 1)) = 0.05
        _Tiling ("Metros por repeticion de textura", Float) = 8

        [Header(Variacion por la sala)]
        _RoomMin ("Sala min (x, z)", Vector) = (164.094, -141.015, 0, 0)
        _RoomMax ("Sala max (x, z)", Vector) = (197.768, -112.175, 0, 0)
        _EdgeWidth ("Ancho de la arena contra paredes (m)", Float) = 7
        _CenterRelief ("Relieve en el centro (0-1)", Range(0, 1)) = 0.35
        _EdgeRelief ("Relieve contra paredes (0-1)", Range(0, 1)) = 1
        _EdgeDarken ("Oscurecer contra paredes", Range(0, 0.5)) = 0.14
        _PathX ("Camino: x del centro", Float) = 180.931
        _PathEndZ ("Camino: hasta z", Float) = -127
        _PathWidth ("Camino: ancho (m)", Float) = 6
        _PathRelief ("Camino: cuanto lo aplana", Range(0, 1)) = 0.75
        _PathLighten ("Camino: aclarar", Range(0, 0.3)) = 0.07
        _NoiseScale ("Ruido: escala", Float) = 0.12
        _NoiseAmount ("Ruido: cuanto varia", Range(0, 1)) = 0.45
        _NoiseTint ("Ruido: variacion de tono", Range(0, 0.3)) = 0.06
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
        TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
        TEXTURE2D(_ParallaxMap); SAMPLER(sampler_ParallaxMap);
        TEXTURE2D(_OcclusionMap); SAMPLER(sampler_OcclusionMap);
        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST, _BaseColor;
            float _BumpScale, _Parallax, _OcclusionStrength, _Smoothness, _Tiling;
            float4 _RoomMin, _RoomMax;
            float _EdgeWidth, _CenterRelief, _EdgeRelief, _EdgeDarken;
            float _PathX, _PathEndZ, _PathWidth, _PathRelief, _PathLighten;
            float _NoiseScale, _NoiseAmount, _NoiseTint;
        CBUFFER_END

        float Hash21(float2 p) { p = frac(p * float2(123.34, 456.21)); p += dot(p, p + 45.32); return frac(p.x * p.y); }
        float VNoise(float2 p)
        {
            float2 i = floor(p), f = frac(p); float2 u = f * f * (3 - 2 * f);
            return lerp(lerp(Hash21(i), Hash21(i + float2(1, 0)), u.x), lerp(Hash21(i + float2(0, 1)), Hash21(i + float2(1, 1)), u.x), u.y);
        }
        float Fbm(float2 p) { return VNoise(p) * 0.6 + VNoise(p * 2.03 + 7.1) * 0.3 + VNoise(p * 4.1 + 3.7) * 0.1; }

        // devuelve x = cuanta arena (0-1), y = cuanto camino (0-1), z = cerca de pared (0-1), w = ruido
        float4 Masks(float3 ws)
        {
            float2 p = ws.xz;
            float dx = min(p.x - _RoomMin.x, _RoomMax.x - p.x);
            float dz = min(p.y - _RoomMin.y, _RoomMax.y - p.y);
            float d = min(dx, dz);                                  // distancia a la pared mas cercana (negativa = afuera, ej. el nicho)
            float wall = 1 - saturate(d / max(_EdgeWidth, 0.01));
            float corner = (1 - saturate(dx / _EdgeWidth)) * (1 - saturate(dz / _EdgeWidth));
            float path = (1 - saturate(abs(p.x - _PathX) / max(_PathWidth, 0.01))) * saturate((p.y - _PathEndZ) / 3.0);
            float n = Fbm(p * _NoiseScale);
            float amount = lerp(_CenterRelief, _EdgeRelief, saturate(wall + corner * 0.5));
            amount = saturate(amount - path * _PathRelief + (n - 0.5) * _NoiseAmount);
            return float4(amount, path, wall, n);
        }
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 lightmapUV : TEXCOORD1;
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 2);
                float  fogFactor  : TEXCOORD3;
            };

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                VertexPositionInputs p = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = p.positionCS;
                o.positionWS = p.positionWS;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.staticLightmapUV);
                OUTPUT_SH(o.normalWS, o.vertexSH);
                o.fogFactor = ComputeFogFactor(p.positionCS.z);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 ws = i.positionWS;
                float4 m = Masks(ws);
                float3 V = normalize(GetWorldSpaceViewDir(ws));

                // UV en mundo (x, z): continuo entre baldosas
                float2 uv = ws.xz / max(_Tiling, 0.01) * _BaseMap_ST.xy + _BaseMap_ST.zw;

                // parallax en espacio tangente de un piso: T = +X, B = +Z, N = +Y
                float3 vt = float3(V.x, V.z, V.y);
                float h = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, uv).g;
                float scale = _Parallax * m.x;
                uv += (h - 0.5) * scale * vt.xy / (vt.z + 0.42);

                half3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _BaseColor.rgb;
                float tone = 1 - m.z * _EdgeDarken + m.y * _PathLighten + (m.w - 0.5) * 2 * _NoiseTint;
                albedo *= tone;

                half3 nTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv), _BumpScale * lerp(0.6, 1.6, m.x));
                float3 nWS = normalize(nTS.x * float3(1, 0, 0) + nTS.y * float3(0, 0, 1) + nTS.z * normalize(i.normalWS));
                half ao = lerp(1, SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g, _OcclusionStrength);

                InputData inputData = (InputData)0;
                inputData.positionWS = ws;
                inputData.positionCS = i.positionCS;
                inputData.normalWS = nWS;
                inputData.viewDirectionWS = V;
                inputData.shadowCoord = TransformWorldToShadowCoord(ws);
                inputData.fogCoord = i.fogFactor;
                inputData.bakedGI = SAMPLE_GI(i.staticLightmapUV, i.vertexSH, nWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(i.staticLightmapUV);

                SurfaceData s = (SurfaceData)0;
                s.albedo = albedo;
                s.alpha = 1;
                s.metallic = 0;
                s.smoothness = _Smoothness;
                s.occlusion = ao;
                s.normalTS = nTS;

                half4 c = UniversalFragmentPBR(inputData, s);
                c.rgb = MixFog(c.rgb, inputData.fogCoord);
                return c;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On ZTest LEqual ColorMask 0
            HLSLPROGRAM
            #pragma vertex vertS
            #pragma fragment fragS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            float3 _LightDirection;
            struct A { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            float4 vertS(A v) : SV_POSITION
            {
                float3 ws = TransformObjectToWorld(v.positionOS.xyz);
                float3 n = TransformObjectToWorldNormal(v.normalOS);
                float4 cs = TransformWorldToHClip(ApplyShadowBias(ws, n, _LightDirection));
                #if UNITY_REVERSED_Z
                    cs.z = min(cs.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    cs.z = max(cs.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                return cs;
            }
            half4 fragS() : SV_Target { return 0; }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            ZWrite On ColorMask R
            HLSLPROGRAM
            #pragma vertex vertD
            #pragma fragment fragD
            float4 vertD(float4 positionOS : POSITION) : SV_POSITION { return TransformObjectToHClip(positionOS.xyz); }
            half4 fragD() : SV_Target { return 0; }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormals" }
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vertN
            #pragma fragment fragN
            struct A { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct V { float4 positionCS : SV_POSITION; float3 normalWS : TEXCOORD0; };
            V vertN(A v) { V o; o.positionCS = TransformObjectToHClip(v.positionOS.xyz); o.normalWS = TransformObjectToWorldNormal(v.normalOS); return o; }
            half4 fragN(V i) : SV_Target { return half4(normalize(i.normalWS), 0); }
            ENDHLSL
        }

        // Meta: para hornear la luz con el color de la arena
        Pass
        {
            Name "Meta"
            Tags { "LightMode"="Meta" }
            Cull Off
            HLSLPROGRAM
            #pragma vertex vertM
            #pragma fragment fragM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
            struct A { float4 positionOS : POSITION; float2 uv1 : TEXCOORD1; float2 uv2 : TEXCOORD2; };
            struct V { float4 positionCS : SV_POSITION; float3 positionWS : TEXCOORD0; };
            V vertM(A v)
            {
                V o;
                o.positionCS = UnityMetaVertexPosition(v.positionOS.xyz, v.uv1, v.uv2);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }
            half4 fragM(V i) : SV_Target
            {
                float2 uv = i.positionWS.xz / max(_Tiling, 0.01);
                MetaInput mi = (MetaInput)0;
                mi.Albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).rgb * _BaseColor.rgb;
                mi.Emission = 0;
                return UnityMetaFragment(mi);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
