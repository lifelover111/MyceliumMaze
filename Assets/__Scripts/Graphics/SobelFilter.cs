using Assets.Pixelation.Example.Scripts;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Sobel")]
public class SobelFilter : ImageEffectBase
{
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float k = Camera.main.aspect;
        Vector2 count = new Vector2(256, 256 / k);
        Vector2 size = new Vector2(1.0f / count.x, 1.0f / count.y);
        //
        material.SetVector("BlockCount", count);
        material.SetVector("BlockSize", size);
        Graphics.Blit(source, destination, material);
    }
}
