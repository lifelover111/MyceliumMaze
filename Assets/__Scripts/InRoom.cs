using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRoom : MonoBehaviour
{
    static public float ROOM_W = LevelGenerator.roomWidth;
    static public float ROOM_H = LevelGenerator.roomHeight;
    static public float WALL_T = 2;

    static public int MAX_RM_X;
    static public int MAX_RM_Y;
    static public Vector2[] DOORS = new Vector2[] {new Vector2(20, 7), new Vector2(10.5f, 13), new Vector2(1, 7), new Vector2(10.5f, 1)};
    static public Vector2[] ENTERS = new Vector2[] { new Vector2(19f, 7), new Vector2(10.5f, 12f), new Vector2(2f, 7), new Vector2(10.5f, 2f) };

    [Header("Set in Inspector")]
    public bool keepInRoom = true;
    public float gridMult = 1;



    private void Start()
    {
        MAX_RM_X = LevelGenerator.numRoomsX;
        MAX_RM_Y = LevelGenerator.numRoomsY;
    }

    void LateUpdate() {
        if (keepInRoom) {
            Vector2 rPos = roomPos;
            rPos.x = Mathf.Clamp( rPos.x, WALL_T, ROOM_W+0.5f-WALL_T );
            rPos.y = Mathf.Clamp( rPos.y, WALL_T, ROOM_H-0.5f-WALL_T );
            roomPos = rPos;
        }
    }

    public Vector2 roomPos {
        get {
            Vector2 tPos = transform.position;
            tPos.x %= ROOM_W;
            tPos.y %= ROOM_H;
            return tPos;
        }
        set {
            Vector2 rm = roomNum;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            rm += value;
            transform.position = rm;
        }
    }

    public Vector2 roomNum {
        get {
            Vector2 tPos = transform.position;
            tPos.x = Mathf.Floor( tPos.x / ROOM_W );
            tPos.y = Mathf.Floor( tPos.y / ROOM_H );
            return tPos;
        }
        set {
            Vector2 rPos = roomPos;
            Vector2 rm = value;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            transform.position = rm + rPos;
        }
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1) {
        if (mult == -1) {
            mult = gridMult;
        }
        Vector2 rPos = roomPos;
        rPos /= mult;
        rPos.x = Mathf.Round( rPos.x );
        rPos.y = Mathf.Round( rPos.y );
        rPos *= mult;
        return rPos;
    }

}
