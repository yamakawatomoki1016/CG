Shader "PostEffect/ToneCorrection"
{
    Properties
    {
        seturation("彩度",range(0,1)) = 1
        contrast("コントラスト",range(0,2)) = 1
    }

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
        
            half seturation;
            half contrast;

            half4 Frag(Varyings input) : SV_Target
            {
                // テクスチャの読み込み
                half4 output = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat, input.texcoord);

                half grayscale = 0.2126 * output.r +
                                 0.7152 * output.g +
                                 0.0722 * output.b;

                half4 monochromeColor = half4(grayscale, grayscale, grayscale, 1);

                half4 outputColor = lerp(monochromeColor, output, seturation);

                outputColor = (outputColor - 0.5) * contrast + 0.5;

                return outputColor;
            }
            ENDHLSL
        }
    }
}