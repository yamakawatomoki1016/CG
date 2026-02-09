using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class PostEffectRenderPass : ScriptableRenderPass
{
    private Material material_ = null;

    public PostEffectRenderPass(Material postEffectMaterial)
    {
        // Shaderからマテリアルを生成
        material_ = postEffectMaterial;
    }

    // RenderGraphへの描画設定や描画実行など一連の操作
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (material_ == null)
        {
            // materialがnullならば従来通りの描画を行う
            base.RecordRenderGraph(renderGraph, frameData);
            return;
        }

        // このフレームの描画リソースを取得する
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        // 取得したResourceDataがBackBufferであれば仕様上再読み込み不可なので早期リターン
        if (resourceData.isActiveTargetBackBuffer)
        {
            return;
        }

        // カメラ(描画予定)のテクスチャを取得
        TextureHandle cameraTexture = resourceData.activeColorTexture;

        // ポストエフェクトを適用したテクスチャを作るためにカメラの情報を取得する
        TextureDesc tempDesc = renderGraph.GetTextureDesc(cameraTexture);

        // 名前などの一部設定は書き換える
        tempDesc.name = "GreenTexture";

        //　深度値は使わない
        tempDesc.depthBufferBits = 0;

        // 仮テクスチャを作成
        TextureHandle tempTexture = renderGraph.CreateTexture(tempDesc);

        // カメラテクスチャにmaterialを適用し仮テクスチャに出力する設定を作成
        RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new RenderGraphUtils.BlitMaterialParameters(cameraTexture, tempTexture, material_, 0);

        // その他の設定をURPに適用
        renderGraph.AddBlitPass(blitMaterialParameters, "BlitGreenPostEffect");

        // URPがポストエフェクトを元のカメラテクスチャにコピーする
        renderGraph.AddCopyPass(tempTexture, cameraTexture, "CopyGreenPostEffect");
    }
}