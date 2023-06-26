// Modified version of outline renderer feature by Alexander Ameye.
// https://alexanderameye.github.io/
// https://twitter.com/alexanderameye/status/1332286868222775298

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    public Settings OutlineSettings;
    private OutlinePass _outlinePass;
    private StencilPass _stencilPass;

    [Serializable]
    public class Settings
    {
        [Header("Visual")]
        [ColorUsage(true, true)]
        public Color Color = new Color(0.2f, 0.4f, 1, 1f);

        [Range(0.0f, 20.0f)]
        public float Width = 5f;

        [Header("Rendering")]
        public LayerMask LayerMask = 0;

        // TODO: Try this again when render layers are working with hybrid renderer.
        // [Range(0, 32)]
        // public int RenderLayer = 1;

        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        public SortingCriteria SortingCriteria = SortingCriteria.CommonOpaque;
    }

    public override void Create()
    {
        if (OutlineSettings == null)
        {
            return;
        }
        _stencilPass = new StencilPass(OutlineSettings);
        _outlinePass = new OutlinePass(OutlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (OutlineSettings == null)
        {
            return;
        }
        renderer.EnqueuePass(_stencilPass);
        renderer.EnqueuePass(_outlinePass);
    }
}