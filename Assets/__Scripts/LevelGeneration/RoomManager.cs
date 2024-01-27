using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;
    [SerializeField] Room[] roomsCastle;
    Dictionary<LevelGenerator.LevelType, Room[]> roomsPrefabDict = new Dictionary<LevelGenerator.LevelType, Room[]>();
    public Dictionary<LevelGenerator.RoomNode, GameObject> nodesRoomsDictionary { get; private set; }
    private void Awake()
    {
        instance = this;
        roomsPrefabDict.Add(LevelGenerator.LevelType.castle, roomsCastle);
    }
    private void Start()
    {
        nodesRoomsDictionary = LevelGenerator.GenerateLevel(LevelGenerator.LevelType.castle);
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
            Debug.LogWarning("Cannot find some room: roomType " + roomType + ", enters " + entersNum + ", exits " + exitNum);
            suitableRooms = roomsPrefabDict[leveltype].Where(room =>
                    room.GetRoomType() != LevelGenerator.RoomNode.RoomType.start && room.GetRoomType() != LevelGenerator.RoomNode.RoomType.end && room.GetRoomType() != LevelGenerator.RoomNode.RoomType.requiredRoom
                    && (room.GetEntersNum() >= entersNum)
                    && (room.GetExitsNum() >= exitNum)
                    ).ToArray();
            return suitableRooms.Where(room => room.GetEntersNum() == suitableRooms.Min(x => x.GetEntersNum()) || room.GetExitsNum() == suitableRooms.Min(x => x.GetExitsNum())).OrderBy(o => Random.value > 0.5f).FirstOrDefault().gameObject;
        }
        return suitableRooms.Where(room => room.GetEntersNum() == suitableRooms.Min(x => x.GetEntersNum()) || room.GetExitsNum() == suitableRooms.Min(x => x.GetExitsNum())).OrderBy(o => Random.value > 0.5f).FirstOrDefault().gameObject;
    }    
}
