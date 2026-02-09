using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class ToneMappingRenderPass : ScriptableRenderPass
{
    private Material material_ = null;

    public ToneMappingRenderPass(Material postEffectMaterial)
    {
        // ShaderからMaterialを生成する
        material_ = postEffectMaterial;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (material_ == null)
        {
            base.RecordRenderGraph(renderGraph, frameData);

            return;
        }

        // このフレームの描画リソースを取得する
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        // カメラ(描画予定)のテクスチャを取得
        TextureHandle cameraTexture = resourceData.activeColorTexture;

        // ポストエフェクトを適用したテクスチャを作るためにカメラの情報を取得する
        TextureDesc tempDesc = renderGraph.GetTextureDesc(cameraTexture);

        // 名前などの一部設定は書き換える
        tempDesc.name = "_ToneMapping";

        //　深度値は使わない
        tempDesc.depthBufferBits = 0;

        // 仮テクスチャを作成
        TextureHandle tempTexture = renderGraph.CreateTexture(tempDesc);

        // カメラテクスチャにmaterial_を適用し仮テクスチャに出力する設定を作成
        RenderGraphUtils.BlitMaterialParameters blitMaterialParameters = new RenderGraphUtils.BlitMaterialParameters(cameraTexture, tempTexture, material_, 0);

        // その他の設定をURPに適用
        renderGraph.AddBlitPass(blitMaterialParameters, "BlitToneMapping");

        // URPがポストエフェクトを元のカメラテクスチャにコピーする
        renderGraph.AddCopyPass(tempTexture, cameraTexture, "CopyToneMapping");
    }
}