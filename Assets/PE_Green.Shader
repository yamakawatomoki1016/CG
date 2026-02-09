Shader "PostEffect/PE_Green"
{
    SubShader
    {
        // URP用であると記述
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // ポストエフェクトでは不要なので機能を切る
            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            // CGではなくHLSL1を使う
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation

            // URP用のシェーダー機能群
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ポストエフェクト用の機能群
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 Frag(Varyings input) : SV_Target
            {
                // テクスチャの読み込み
                half4 output = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat, input.texcoord);

                // 赤成分と青成分を無くす
                output.r = 0;
                output.b = 0;
                return output;
            }
            ENDHLSL
        }
    }
}