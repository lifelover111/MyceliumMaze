using OldProject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    [SerializeField] Room[] roomsPrison;
    Dictionary<LevelGenerator.LevelType, Room[]> roomsPrefabDict = new Dictionary<LevelGenerator.LevelType, Room[]>();
    private void Awake()
    {
        instance = this;
        roomsPrefabDict.Add(LevelGenerator.LevelType.prison, roomsPrison);
        LevelGenerator.Init();
        LevelGenerator.GenerateLevel(LevelGenerator.LevelType.prison);
    }
    public GameObject GetRoom(LevelGenerator.LevelType leveltype, LevelGenerator.RoomNode.RoomType roomType, int entersNum, int exitNum)
    {
        Room[] suitableRooms = roomsPrefabDict[leveltype].Where(
        room => (roomType == LevelGenerator.RoomNode.RoomType.any ?
        (room.GetRoomType() != LevelGenerator.RoomNode.RoomType.start && room.GetRoomType() != LevelGenerator.RoomNode.RoomType.end && room.GetRoomType() != LevelGenerator.RoomNode.RoomType.requiredRoom) : 
            room.GetRoomType() == roomType)
            && (room.GetEntersNum() >= entersNum)
            && (room.GetExitsNum() >= exitNum)
            ).ToArray();
        if (suitableRooms.Length == 0)
        { 
            Debug.LogError("Cannot find some room: roomType " + roomType + ", enters " + entersNum + ", exits " + exitNum);
            return null;
        }
        return suitableRooms.Where(room => room.GetEntersNum() == suitableRooms.Min(x => x.GetEntersNum()) || room.GetExitsNum() == suitableRooms.Min(x => x.GetExitsNum())).OrderBy(o => Random.value > 0.5f).FirstOrDefault().gameObject;
    }    
}
