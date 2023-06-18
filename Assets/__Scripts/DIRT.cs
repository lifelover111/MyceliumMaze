using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Dirt_Handler
{

	private static List<Dirt_Handler> instanceList = new List<Dirt_Handler>();

	private float timer = 1f;
	private int materialStarting;
	private int material;
	private float eulerY;
	private Vector3 velocity = Vector3.zero;
	private float speed;
	private Vector3 pos = Vector3.zero;
	private int index;
	private Generic_Mesh_Script meshScript;
	private static Vector3 baseSize = new Vector3(3f, 3f);

	private static float deltaTime, deltaTime5;

	private static float intervalTimer;
	private static float intervalTimerMax = 0.1f;

	private float materialTimer;
	private float materialTimerMax = 0.06f;

	private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

	private static void InitIfNeeded()
	{
		if (initGameObject == null)
		{
			initGameObject = new GameObject("Dirt_Handler");
			ComponentActions.AddComponent(initGameObject, null, null, null, Update_Static);
		}
	}
	public static void ResetStatic()
	{
		instanceList = new List<Dirt_Handler>();
	}

	public Dirt_Handler()
	{
		materialStarting = UnityEngine.Random.Range(0, 2) * 8;
		material = materialStarting;//Random.Range(0,8);
		materialTimer = materialTimerMax;
	}

	void Update()
	{
		pos += velocity * (speed * deltaTime);
		speed -= speed * deltaTime5;
		//material = 8 - Mathf.FloorToInt(timer * 16f);
		//if (material < 0) material = 0;
		//if (material > 7) material = 7;
		materialTimer -= deltaTime;
		if (materialTimer < 0)
		{
			materialTimer = materialTimerMax;
			material++;
		}
		if (material > materialStarting + 7) material = materialStarting + 7;

		meshScript.updateGeneric(index, pos, eulerY, material, 0f, baseSize, false);

		timer -= deltaTime;
		if (timer < 0)
		{
			instanceList.Remove(this);
		}
	}
	public static void SpawnInterval(Vector3 loc, Vector3 dir)
	{
		if (intervalTimer <= 0f)
		{
			Spawn(1, loc, dir);
			intervalTimer = intervalTimerMax;
		}
	}
	public static void Spawn(Vector3 loc, Vector3 dir)
	{
		Spawn(3, loc, dir);
	}
	public static void Spawn(int amt, Vector3 loc, Vector3 dir)
	{
		InitIfNeeded();
		dir.Normalize();
		Vector3 baseDir = dir;

		loc.z = 0;
		for (int i = 0; i < amt; i++)
		{
			dir = baseDir;
			Dirt_Handler handler = new Dirt_Handler();
			handler.pos = loc;
			handler.index = Generic_Mesh_Script.GetIndex("Mesh_Dirt");
			handler.meshScript = Generic_Mesh_Script.GetMeshScript("Mesh_Dirt");

			dir.x += UnityEngine.Random.Range(-.4f, .4f);
			dir.y += UnityEngine.Random.Range(-.4f, .4f);

			Vector3 velocity = dir.normalized;

			handler.speed = 5f * UnityEngine.Random.Range(3f, 6f);
			handler.velocity = velocity;
			handler.eulerY = UnityEngine.Random.Range(0, 360);
			Generic_Mesh_Script.AddGeneric("Mesh_Dirt", handler.pos, handler.eulerY, handler.material, 0f, baseSize, false);

			instanceList.Add(handler);
		}
	}
	public static void Update_Static()
	{
		deltaTime = Time.deltaTime;
		deltaTime5 = deltaTime * 5;
		intervalTimer -= deltaTime;
		for (int i = 0; i < instanceList.Count; i++)
		{
			instanceList[i].Update();
		}
	}
}











public class ComponentActions : MonoBehaviour
{

	public Action OnDestroyFunc;
	public Action OnEnableFunc;
	public Action OnDisableFunc;
	public Action OnUpdate;

	void OnDestroy()
	{
		if (OnDestroyFunc != null) OnDestroyFunc();
	}
	void OnEnable()
	{
		if (OnEnableFunc != null) OnEnableFunc();
	}
	void OnDisable()
	{
		if (OnDisableFunc != null) OnDisableFunc();
	}
	void Update()
	{
		if (OnUpdate != null) OnUpdate();
	}


	public static void CreateComponent(Action OnDestroyFunc = null, Action OnEnableFunc = null, Action OnDisableFunc = null, Action OnUpdate = null)
	{
		GameObject gameObject = new GameObject("ComponentActions");
		AddComponent(gameObject, OnDestroyFunc, OnEnableFunc, OnDisableFunc, OnUpdate);
	}
	public static void AddComponent(GameObject gameObject, Action OnDestroyFunc = null, Action OnEnableFunc = null, Action OnDisableFunc = null, Action OnUpdate = null)
	{
		ComponentActions componentFuncs = gameObject.AddComponent<ComponentActions>();
		componentFuncs.OnDestroyFunc = OnDestroyFunc;
		componentFuncs.OnEnableFunc = OnEnableFunc;
		componentFuncs.OnDisableFunc = OnDisableFunc;
		componentFuncs.OnUpdate = OnUpdate;
	}
}





[System.Serializable]

public class UVVectors
{
	public Vector2 Vector_1, Vector_2, Vector_3, Vector_4;

	public UVVectors(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
	{
		Vector_1 = v1;
		Vector_2 = v2;
		Vector_3 = v3;
		Vector_4 = v4;
	}
}