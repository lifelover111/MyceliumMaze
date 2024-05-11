using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ToonShaderFeature : ScriptableRendererFeature
{
    class ToonShaderPass : ScriptableRenderPass
    {
        private Material material;

        public ToonShaderPass(Material material)
        {
            this.material = material;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("ToonShaderPass");

            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material material;

    private ToonShaderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new ToonShaderPass(material);

        // Configuring where the pass should be injected.
        scriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Adding the pass to the renderer.
        renderer.EnqueuePass(scriptablePass);
    }
}