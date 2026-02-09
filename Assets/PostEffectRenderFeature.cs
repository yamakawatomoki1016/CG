using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PostEffectRenderFeature : ScriptableRendererFeature
{
    // ポストエフェクト用マテリアル
    [SerializeField]
    private Material blurMaterial_;

    // Blit用マテリアル
    [SerializeField]
    private Material passThroughMaterial_;

    // URPに渡すRenderPass
    private PostEffectRenderPass renderPass_;

    // このクラスがURPによって生成されたときに呼ばれる関数
    public override void Create()
    {
        // RenderPassを生成
        renderPass_ = new PostEffectRenderPass(blurMaterial_, passThroughMaterial_);

        // レンダリング完了後、他のポストエフェクトが適用される例
        renderPass_.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    // パスを追加する関数
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderPass_ != null)
        {
            renderer.EnqueuePass(renderPass_);
        }
    }
}