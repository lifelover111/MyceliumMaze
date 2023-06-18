using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class LevelGenerator
{
    static Room[,] _level;
    static public Room[,] level { get { return _level; } private set { _level = value; } }
    static Room _startRoom;
    static public Room startRoom { get { return _startRoom; } private set { _startRoom = value; } }

    static int intSeed;
    static public int numRoomsX;
    static public int numRoomsY;
    static public int roomWidth = 21;
    static public int roomHeight = 15;

    static int minWidthInRooms = 5;
    static int minHeightInRooms = 5;
    static int maxWidthInRooms = 10;
    static int maxHeightInRooms = 10;

    static public int H;
    static public int W;

    static public Vector2[] environmentsCenters = { new Vector2(5f, 4.5f), new Vector2(5f, 9.5f), new Vector2(16f, 4.5f), new Vector2(16f, 9.5f), new Vector2(10.5f, 7f) };

    public static void Init(string seed = null)
    {
        if (seed != null)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(seed);
            if (System.BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(bytes);
            }
            intSeed = System.BitConverter.ToInt32(bytes, 0);
            Random.InitState(intSeed);
        }


        numRoomsX = Random.Range(minWidthInRooms, maxWidthInRooms);
        numRoomsY = Random.Range(minHeightInRooms, maxHeightInRooms);


        W = numRoomsX*roomWidth;
        H = numRoomsY*roomHeight;
    }

    public static Room[,] GenerateLevel()
    {
        level = new Room[numRoomsX, numRoomsY];
        for (int x = 0; x < numRoomsX; x++)
            for (int y = 0; y < numRoomsY; y++)
                level[x, y] = new Room(x, y);
        Stack<Room> stack = new Stack<Room>();
        Room current = level[Random.Range(0, numRoomsX), Random.Range(0, numRoomsY)];
        current.visited = true;
        while (true)
        {
            List<Room> neighbors = new List<Room>();
            for (int i = -1; i < 2; i += 2)
            {
                if (current.x + i >= 0 && current.x + i < numRoomsX)
                {
                    if (!level[current.x + i, current.y].visited)
                        neighbors.Add(level[current.x + i, current.y]);
                }
                if (current.y + i >= 0 && current.y + i < numRoomsY)
                {
                    if (!level[current.x, current.y + i].visited)
                        neighbors.Add(level[current.x, current.y + i]);
                }
            }
            if(neighbors.Count > 0)
            {
                Room next = neighbors[Random.Range(0, neighbors.Count)];
                current.Connect(next);
                stack.Push(current);
                current = next;
                current.visited = true;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
            else
            {
                break;
            }
        }

        startRoom = level[Random.Range(0, numRoomsX), Random.Range(0, numRoomsY)];
        startRoom.SetType(Room.eRoomType.rest);
        foreach (Room room in level)
            room.visited = false;


        SetRoomsDepth(startRoom, 0);

        foreach (Room room in level)
            room.visited = false;
        
        SetRoomsRest(startRoom, 0);

        return level;
    }

    static void SetRoomsDepth(Room room, int depth)
    {
        room.depth = depth;
        room.visited = true;
        foreach(Room.eDoorLoc doorLoc in room.doorLocs)
        {
            Vector2 bypassDir = new Vector2(Mathf.Cos(Mathf.PI/2*(int)doorLoc), Mathf.Sin(Mathf.PI/2 * (int)doorLoc));
            if (!level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)].visited)
                SetRoomsDepth(level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)], depth + 1);
        }
    }

    static void SetRoomsRest(Room room, float chance)
    {
        room.visited = true;
        float nxtChance = chance + 0.025f;
        if (room.type == null)
        {
            if (GetRandomChance(nxtChance))
            {
                room.type = Room.eRoomType.rest;
                nxtChance = 0;
            }
            else
                room.type = Room.eRoomType.fight;
        }
        foreach (Room.eDoorLoc doorLoc in room.doorLocs)
        {
            Vector2 bypassDir = new Vector2(Mathf.Cos(Mathf.PI / 2 * (int)doorLoc), Mathf.Sin(Mathf.PI / 2 * (int)doorLoc));
            if (!level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)].visited)
                SetRoomsRest(level[room.x + Mathf.RoundToInt(bypassDir.x), room.y + Mathf.RoundToInt(bypassDir.y)], nxtChance);
        }
    }


    public static bool GetRandomChance(float chance)
    {
        return Random.value <= chance;
    }

}
