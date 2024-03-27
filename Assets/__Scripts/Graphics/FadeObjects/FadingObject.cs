using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingObject : MonoBehaviour, System.IEquatable<FadingObject>
{
    public List<Renderer> renderers = new List<Renderer>();
    public Vector3 position;
    public List<Material> materials = new List<Material>();
    [HideInInspector]
    public float initialAlpha;

    private void Awake()
    {
        position = transform.position;
        if(renderers.Count == 0)
        {
            renderers.AddRange(GetComponentsInChildren<Renderer>());
        }
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        initialAlpha = materials[0].color.a;
    }

    public bool Equals(FadingObject other)
    {
        return position.Equals(other.position);
    }
}
