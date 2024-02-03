using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform target;
    [SerializeField] Camera cam;
    [SerializeField][Range(0, 1f)] float fadedAlpha = 0.33f;
    [SerializeField] bool retainShadows = true;
    [SerializeField] Vector3 targetPositionOffset = Vector3.up;
    [SerializeField] float fadeSpeed = 1;

    [Header("Read Only Data")]
    [SerializeField] List<FadingObject> objectsBlockingView = new List<FadingObject>();
    Dictionary<FadingObject, Coroutine> runningCoroutines = new Dictionary<FadingObject, Coroutine>();
    RaycastHit[] hits = new RaycastHit[12];

    private void Start()
    {
        cam = Camera.main;
        StartCoroutine(CheckForObjects());
    }

    IEnumerator CheckForObjects()
    {
        while(true)
        {
            int hits = Physics.RaycastNonAlloc(
                cam.transform.position,
                (target.transform.position + targetPositionOffset - cam.transform.position).normalized,
                this.hits,
                Vector3.Distance(cam.transform.position, target.transform.position + targetPositionOffset),
                layerMask
            );

            if(hits > 0)
            {
                for(int i = 0; i< hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(this.hits[i]);

                    if(fadingObject != null && !objectsBlockingView.Contains(fadingObject))
                    {
                        if(runningCoroutines.ContainsKey(fadingObject))
                        {
                            if (runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutines[fadingObject]);
                            }

                            runningCoroutines.Remove(fadingObject);
                        }
                        runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null;
        }
    }
    IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", retainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;
        while (fadingObject.materials[0].color.a > fadedAlpha)
        {
            foreach(Material material in fadingObject.materials)
            {
                if(material.HasProperty("_Color"))
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.Lerp(fadingObject.initialAlpha, fadedAlpha, time * fadeSpeed));

                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if(runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float time = 0;
        while (fadingObject.materials[0].color.a < fadingObject.initialAlpha)
        {
            foreach (Material material in fadingObject.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b, Mathf.Lerp(fadedAlpha, fadingObject.initialAlpha, time * fadeSpeed));

                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new List<FadingObject>(objectsBlockingView.Count);
        foreach(FadingObject fadingObject in objectsBlockingView)
        {
            bool objectIsBeingHit = false;
            for (int i = 0; i < hits.Length; i++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(hits[i]);
                if (hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit) 
            {
                if(runningCoroutines.ContainsKey(fadingObject))
                {
                    if (runningCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(runningCoroutines[fadingObject]);
                    }
                    runningCoroutines.Remove(fadingObject);
                }

                runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach(FadingObject removeObject in objectsToRemove)
        {
            objectsBlockingView.Remove(removeObject);
        }
    }

    void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
    }

    FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null;
    }
}
