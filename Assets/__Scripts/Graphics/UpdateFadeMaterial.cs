using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UpdateFadeMaterial : MonoBehaviour
{
    private IEnumerable<Material> materials = new List<Material>();
    private PlayerManager player;
    private float radius = 5;
    private float height = 2;

    private void Start()
    {
        player = PlayersInGameManager.instance.playerList.First();

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            materials = materials.Union(renderer.materials);
        }
        foreach (var material in materials)
        {
            material.SetFloat("_Radius", radius);
            material.SetFloat("_PlayerHeight", height);
        }
    }

    private void Update()
    {
        foreach (var material in materials) 
        {
            material.SetVector("_PlayerPosition", player.transform.position);
        }
    }

}


