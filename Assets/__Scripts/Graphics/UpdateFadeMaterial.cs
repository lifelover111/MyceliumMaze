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





Player.y = Player.y + Height;

float a = length(Point - Camera);
float3 vecPlayerToCamera = Player - Camera;
float3 vecPointToCamera = Point - Camera;
float3 vecPlayerToPoint = Point - Player;

vecPlayerToCamera = normalize(vecPlayerToCamera);
float3 vecPlayerToCamera2 = -vecPlayerToCamera;
vecPlayerToCamera2.y = 0;
vecPlayerToPoint.y = 0;
// Нормализуем векторы
vecPlayerToCamera2 = normalize(vecPlayerToCamera2);
vecPointToCamera = normalize(vecPointToCamera);
vecPlayerToPoint = normalize(vecPlayerToPoint);

// Вычисляем дот-произведение векторов
float dotProduct = dot(vecPlayerToCamera, vecPointToCamera);
float dotProduct2 = dot(vecPlayerToCamera2, vecPlayerToPoint);


// Вычисляем угол между векторами (в радианах)
float angleRad = acos(dotProduct);

float Distance = a * sin(angleRad);
float depth = a * cos(angleRad);
if (depth < length(Player - Camera))
{
    if (dotProduct2 < sqrt(2) / 2)
    {
        Mask = 1;
    }
    else if (Distance < Radius)
    {
        Mask = 0;
    }

    else if (Distance >= Radius && Distance < Radius + Rim)
    {
        Mask = (Distance - Radius) / Rim;
    }

    else
    {
        Mask = 1;
    }
}

// angleRad2 > pi/4
else if (dotProduct2 < sqrt(2) / 2)
{
    Mask = 1;
}

else
{
    Mask = 0;
}








//Player.y = Player.y + Height;

//float a = length(Point - Camera);
//float3 vecPlayerToCamera = Player - Camera;
//float3 vecPointToCamera = Point - Camera;
//float3 vecPlayerToPoint = Point - Player;

//vecPlayerToCamera = normalize(vecPlayerToCamera);
//float3 vecPlayerToCamera2 = -vecPlayerToCamera;
//vecPlayerToCamera2.y = 0;
//vecPlayerToPoint.y = 0;
//// Нормализуем векторы
//vecPlayerToCamera2 = normalize(vecPlayerToCamera2);
//vecPointToCamera = normalize(vecPointToCamera);
//vecPlayerToPoint = normalize(vecPlayerToPoint);

//// Вычисляем дот-произведение векторов
//float dotProduct = dot(vecPlayerToCamera, vecPointToCamera);
//float dotProduct2 = dot(vecPlayerToCamera2, vecPlayerToPoint);


//// Вычисляем угол между векторами (в радианах)
//float angleRad = acos(dotProduct);

//float Distance = a * sin(angleRad);

//vecPlayerToCamera.y = 0;
//vecPlayerToCamera = normalize(vecPlayerToCamera);
//vecPointToCamera.y = 0;
//float3 vecPointToCamera3 = Camera - Point;
//vecPointToCamera3.y = 0;
//vecPointToCamera = normalize(vecPlayerToCamera);
//float dotProduct3 = dot(vecPlayerToCamera, vecPointToCamera);

//float3 vecPlayerToCamera4 = Player - Camera;
//vecPlayerToCamera4.y = 0;

//float depth = length(vecPointToCamera3) * dotProduct3;
//if (depth < length(vecPlayerToCamera4))
//{
//    if (dotProduct2 < sqrt(2) / 2)
//    {
//        Mask = 1;
//    }
//    else if (Distance < Radius)
//    {
//        Mask = 0;
//    }

//    else if (Distance >= Radius && Distance < Radius + Rim)
//    {
//        Mask = (Distance - Radius) / Rim;
//    }

//    else
//    {
//        Mask = 1;
//    }
//}

//// angleRad2 > pi/4
//else if (dotProduct2 < sqrt(2) / 2)
//{
//    Mask = 1;
//}

//else
//{
//    Mask = 0;
//}

