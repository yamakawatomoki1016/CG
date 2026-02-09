using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ToneMappingRenderFeature : ScriptableRendererFeature
{
    // ポストエフェクト計算用のマテリアル
    [SerializeField]
    private Material postEffectMaterial_;

    // URPに渡すRenderPass
    private ToneMappingRenderPass renderPass_;

    // このクラスがURPによって生成されたときに呼ばれる関数
    public override void Create()
    {
        // RenderPassを生成
        renderPass_ = new ToneMappingRenderPass(postEffectMaterial_);

        // レンダリング完了後、他のポストエフェクトが適用される前
        renderPass_.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // パスを追加する関数
    public override void AddRenderPasses(ScriptableRenderer rendererPass, ref RenderingData renderingData)
    {
        if (renderPass_ != null)
        {
            rendererPass.EnqueuePass(renderPass_);
        }
    }
}