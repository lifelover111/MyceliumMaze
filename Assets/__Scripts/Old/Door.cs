using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace OldProject
{
    public class Door : MonoBehaviour
    {
        public static List<InRoom> enemiesInCurrentRoom = new List<InRoom>();
        bool isAnimStarted = false;
        InRoom inrm;
        Fog fog;
        [SerializeField] public string rotation;

        public delegate void RemoveInRoomDelegate(InRoom inrm);
        public static RemoveInRoomDelegate removeInRoom = (InRoom inrm) => { enemiesInCurrentRoom.Remove(inrm); };
        void Awake()
        {
            Transform leftArc = transform.Find("LeftArc");
            Transform rightArc = transform.Find("RightArc");
            Transform leftFrontRow = leftArc.Find("FrontRow");
            Transform leftBackRow = leftArc.Find("BackRow");
            Transform rightFrontRow = rightArc.Find("FrontRow");
            Transform rightBackRow = rightArc.Find("BackRow");
            fog = GetComponentInChildren<Fog>();

            GameObject[] frontWalls = LevelComponentKeeper.instance.frontWallPrefabs;
            GameObject[] backWalls = LevelComponentKeeper.instance.backWallPrefabs;


            GameObject go = Instantiate(frontWalls[Random.Range(0, frontWalls.Length)]);
            go.transform.SetParent(leftFrontRow, false);
            go = Instantiate(frontWalls[Random.Range(0, frontWalls.Length)]);
            go.transform.SetParent(rightFrontRow, false);
            go = Instantiate(backWalls[Random.Range(0, backWalls.Length)]);
            go.transform.SetParent(leftBackRow, false);
            go = Instantiate(backWalls[Random.Range(0, backWalls.Length)]);
            go.transform.SetParent(rightBackRow, false);


            inrm = gameObject.GetComponent<InRoom>();
        }

        private void Update()
        {
            /*
            if (TileCamera.heroInrm.roomNum == inrm.roomNum)
            {
                if (enemiesInCurrentRoom.Count > 0)
                {
                    if (!isAnimStarted)
                    {
                        isAnimStarted = true;
                        TileCamera.heroInrm.keepInRoom = true;
                        fog.Invoke("CreateFog", HeroKeeper.instance.heroList[0].transitionDelay);
                    }

                }
                else
                {
                    fog.Invoke("ClearFog", HeroKeeper.instance.heroList[0].transitionDelay);
                    TileCamera.heroInrm.keepInRoom = false;
                }
            }
            
        }

    }
}
*/