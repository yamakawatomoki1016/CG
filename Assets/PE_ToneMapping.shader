Shader "PostEffect/PE_ToneMapping"
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
        
            // CGではなくHLSLを使う
            HLSLPROGRAM
        
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma editor_sync_compilation
        
            // URP用のシェーダー機能群
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
            // ポストエフェクト用の機能群
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        
            // 色から輝度の算出
            half GetLuminance(half3 color)
            {
                return dot(color, half3(0.2126, 0.7152, 0.0722));
            }
        
            // 受け取った色をそのまま返す（ドーピング無し）
            half Linear(half luminance)
            {
                return luminance;
            }

            // 全体的に割り算
            // half Division(half luminance, half divisor)
            // {
            //     return luminance / divisor;
            // }

            half Reinhard(half lIn)
            {
                return lIn / (1.0 + lIn);
            }

            // ACES色空間に変換
            half3 MulAcesInputMatrix(half3 col)
            {
                half3x3 acesInputMatrix = half3x3(
                    0.59719, 0.35458, 0.04823,
                    0.07600, 0.90834, 0.01566,
                    0.02840, 0.13383, 0.83777
                );
                return mul(acesInputMatrix, col);
            }
            
            // 人の感覚に近く、出力に適した色に変換
            half3 RRTAndODTFit(half3 v)
            {
                half3 a = v * (v + 0.0245786) - 0.000090537;
                half3 b = v * (0.983729 * v + 0.4329510) + 0.238081;
                return a / b;
            }

            // sRGB空間に変換
            half3 MulAcesOutputMatrix(half3 col)
            {
                half3x3 acesOutputMatrix = half3x3(
                     1.60475, -0.53108, -0.07367,
                    -0.10208,  1.10813, -0.00605,
                    -0.00327, -0.07276,  1.07602
                );
                return mul(acesOutputMatrix, col);
            }

            // ピクセルシェーダー
            half4 Frag(Varyings input) : SV_Target
            {
                // テクスチャの読み込み
                half4 output = SAMPLE_TEXTURE2D(_BlitTexture,sampler_LinearRepeat, input.texcoord);
        
                // 輝度情報を算出
                //half lIn = GetLuminance(output.rgb);
        
                // 輝度情報からトーンマッピングを算出
                //half lOut = Division(lIn,10);
                //half lOut = Reinhard(lIn);
        
                // トーンマッピング値を色を乗算
                //half4 outputColor = output * lOut / max(lIn,0.001);

                half3 acesInput = MulAcesInputMatrix(output.rgb);
                half3 transpose = RRTAndODTFit(acesInput);
                half3 acesOutput = MulAcesOutputMatrix(transpose);
                half4 outputColor = half4(acesOutput, 1);

                // アルファ値は1固定
                outputColor.a = 1;
        
                return outputColor;
            }
            ENDHLSL
        }
    }
}