using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorMultiplierFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ColorMultiplierSettings
    {
        public int multiplier = 1;
    }

    public ColorMultiplierSettings settings = new ColorMultiplierSettings();

    class ColorMultiplierPass : ScriptableRenderPass
    {
        public Material material;

        public ColorMultiplierPass(Material material)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            this.material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                Debug.LogWarning("Missing material in ColorMultiplierPass.");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("ColorMultiplierPass");

            // Set shader properties
            //material.SetInt("_Multiplier", ((ColorMultiplierFeature)renderingData.rendererFeatures[0]).settings.multiplier);

            // Draw fullscreen quad
            cmd.DrawProcedural(Matrix4x4.identity, material, 0, MeshTopology.Triangles, 3, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    ColorMultiplierPass colorMultiplierPass;

    public override void Create()
    {
        Material colorMultiplierMaterial = new Material(Shader.Find("Hidden/ColorMultiplierShader"));
        colorMultiplierPass = new ColorMultiplierPass(colorMultiplierMaterial);
        colorMultiplierPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(colorMultiplierPass);
    }
}