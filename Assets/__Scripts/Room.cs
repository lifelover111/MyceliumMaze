using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public enum eDoorLoc
    {
        right,
        up,
        left,
        down
    }

    public enum eRoomType
    {
        fight,
        chest,
        rest
    }

    public eRoomType? type = null;
    public int x;
    public int y;
    public bool visited = false;
    public List<eDoorLoc> doorLocs = new List<eDoorLoc>();

    public List<Room> ConnectedRooms { get; set; }
    public int depth;
    
    private int[,] tiles;

    public Room(int x, int y)
    {
        this.x = x;
        this.y = y;

        tiles = new int[LevelGenerator.roomHeight, LevelGenerator.roomWidth];
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = Random.Range(0, 4);
            }
        }
    }


    public void Connect(Room another)
    {
        if (this.x == another.x)
        {
            if (this.y > another.y)
            {
                this.doorLocs.Add(eDoorLoc.down);
                another.doorLocs.Add(eDoorLoc.up);
            }
            else
            {
                this.doorLocs.Add(eDoorLoc.up);
                another.doorLocs.Add(eDoorLoc.down);
            }
        }

        else if (this.y == another.y)
        {
            if (this.x > another.x)
            {
                this.doorLocs.Add(eDoorLoc.left);
                another.doorLocs.Add(eDoorLoc.right);
            }
            else
            {
                this.doorLocs.Add(eDoorLoc.right);
                another.doorLocs.Add(eDoorLoc.left);
            }
        }
        else
        {
            return;
        }
    }

    public int[,] GetIntPresentation()
    {
        return tiles;
    }

    public void SetType(eRoomType type)
    {
        this.type = type;
    }



    public void SpawnDoors()
    {
        foreach(eDoorLoc door in doorLocs)
        {
            GameObject doorPrefab;
            Vector2 relativeDelta = new Vector2(x * LevelGenerator.roomWidth, y * LevelGenerator.roomHeight);
            switch (door)
            {
                case eDoorLoc.right:
                    doorPrefab = Object.Instantiate(LevelComponentKeeper.instance.doorRightPrefab);
                    break;
                case eDoorLoc.up:
                    doorPrefab = Object.Instantiate(LevelComponentKeeper.instance.doorTopPrefab);
                    break;
                case eDoorLoc.left:
                    doorPrefab = Object.Instantiate(LevelComponentKeeper.instance.doorLeftPrefab);
                    break;
                case eDoorLoc.down:
                    doorPrefab = Object.Instantiate(LevelComponentKeeper.instance.doorBottomPrefab);
                    break;
                default:
                    doorPrefab = null;
                    break;
            }
            doorPrefab.transform.position = InRoom.DOORS[(int)door] + relativeDelta;
            doorPrefab.transform.SetParent(TileCamera.DOOR_ANCHOR);
        }
    }
    
    public void SpawnWalls(float step, float wallThickness)
    {
        Vector2 relativeDelta = new Vector2(x * LevelGenerator.roomWidth, y * LevelGenerator.roomHeight);
        float i = 0;
        while (i <= LevelGenerator.roomWidth - wallThickness)
        {
            if (!doorLocs.Contains(eDoorLoc.up) || Mathf.Abs(InRoom.DOORS[1].x - i) >= 3.9f)
            {
                GameObject wall = Object.Instantiate(LevelComponentKeeper.instance.frontWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.frontWallPrefabs.Length)]);
                wall.transform.position = new Vector2(i, LevelGenerator.roomHeight - 3.5f) + relativeDelta + new Vector2(0, Random.value/5 - 0.1f);
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);

                wall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
                wall.transform.position = new Vector2(i + 0.7f, LevelGenerator.roomHeight - 3f) + relativeDelta + new Vector2(0, Random.value / 5 - 0.1f);
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);
            }

            if (!doorLocs.Contains(eDoorLoc.down) || Mathf.Abs(InRoom.DOORS[3].x - i) >= 3.9f)
            {
                GameObject wall = Object.Instantiate(LevelComponentKeeper.instance.frontWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.frontWallPrefabs.Length)]);
                wall.transform.position = new Vector2(i, 0) + relativeDelta + new Vector2(0, Random.value / 5 - 0.1f);
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);

                wall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
                wall.transform.position = new Vector2(i + 0.7f, -0.5f) + relativeDelta + new Vector2(0, Random.value / 5 - 0.1f);
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);
            }

            i += step;

        }

        GameObject additionalWall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
        additionalWall.transform.position = new Vector2(0, -0.5f) + relativeDelta + new Vector2(0, Random.value / 5 - 0.1f);
        additionalWall.transform.SetParent(TileCamera.WALL_ANCHOR);



        i = 0;
        float verticalStep = 5*step / 7;
        while (i <= LevelGenerator.roomHeight - wallThickness)
        {

            if (!doorLocs.Contains(eDoorLoc.left) || i >= 8 + verticalStep || i <= 3.5 - verticalStep)
            {
                GameObject wall = Object.Instantiate(LevelComponentKeeper.instance.frontWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.frontWallPrefabs.Length)]);
                wall.transform.position = new Vector2(0.7f, i) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0);
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);

                wall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
                wall.transform.position = new Vector2(0, i + 0.5f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);
            }


            if (!doorLocs.Contains(eDoorLoc.right) || i >= 8 + verticalStep || i <= 3.5 - verticalStep)
            {
                GameObject wall = Object.Instantiate(LevelComponentKeeper.instance.frontWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.frontWallPrefabs.Length)]);
                wall.transform.position = new Vector2(LevelGenerator.roomWidth - 0.7f, i) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);

                wall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
                wall.transform.position = new Vector2(LevelGenerator.roomWidth, i + 0.5f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
                wall.transform.SetParent(TileCamera.WALL_ANCHOR);
            }

            i += verticalStep;
        }

        additionalWall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
        additionalWall.transform.position = new Vector2(20.7f, 9f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
        additionalWall.transform.SetParent(TileCamera.WALL_ANCHOR);
        additionalWall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
        additionalWall.transform.position = new Vector2(20.7f, 2.6f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
        additionalWall.transform.SetParent(TileCamera.WALL_ANCHOR);
        additionalWall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
        additionalWall.transform.position = new Vector2(0, 9f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
        additionalWall.transform.SetParent(TileCamera.WALL_ANCHOR);
        additionalWall = Object.Instantiate(LevelComponentKeeper.instance.backWallPrefabs[Random.Range(0, LevelComponentKeeper.instance.backWallPrefabs.Length)]);
        additionalWall.transform.position = new Vector2(0f, 2.6f) + relativeDelta + new Vector2(Random.value / 5 - 0.1f, 0); ;
        additionalWall.transform.SetParent(TileCamera.WALL_ANCHOR);

    }



    public void SpawnEnemies()
    {
        if (type != eRoomType.fight)
            return;
        
        int numEnemies = Random.Range(Mathf.RoundToInt(Random.value) + Mathf.RoundToInt(depth / Mathf.Sqrt(LevelGenerator.numRoomsX * LevelGenerator.numRoomsY)), 3 + Mathf.RoundToInt(Mathf.Sqrt(depth)));
        
        for(int i = 0; i < numEnemies; i++)
        {
            GameObject go = Object.Instantiate(LevelComponentKeeper.instance.enemyPrefabs[Random.Range(0, LevelComponentKeeper.instance.enemyPrefabs.Length)].enemy);

            int relativeLevel = depth / 10;
            relativeLevel = LevelGenerator.GetRandomChance(2 * depth / (LevelGenerator.numRoomsX + LevelGenerator.numRoomsY)) ? relativeLevel + Mathf.RoundToInt(2 * depth / (LevelGenerator.numRoomsX + LevelGenerator.numRoomsY)) : relativeLevel;
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.SetStats(new Stats(enemy.stats.Vitality+relativeLevel, enemy.stats.Endurance + relativeLevel, enemy.stats.Strength + relativeLevel, enemy.stats.Dexterity + relativeLevel));

            go.transform.position = new Vector2(LevelGenerator.roomWidth * Random.Range(x + InRoom.WALL_T/InRoom.ROOM_W + 0.1f, x + 1 - InRoom.WALL_T / InRoom.ROOM_W - 0.1f), LevelGenerator.roomHeight * Random.Range(y + InRoom.WALL_T/InRoom.ROOM_H + 0.15f, y + 1 - InRoom.WALL_T / InRoom.ROOM_H - 0.15f));
            go.transform.SetParent(TileCamera.ENEMY_ANCHOR);
            TileCamera.enemyInrms.Add(go.GetComponent<InRoom>());
        }
    }

    public void SpawnEnvironment()
    {
        int locsLength = type == eRoomType.rest ? LevelGenerator.environmentsCenters.Length - 1 : LevelGenerator.environmentsCenters.Length;
        int numObj = Random.Range(0, 8);
        for(int i = 0; i < numObj; i++)
        {
            GameObject go = Object.Instantiate(LevelComponentKeeper.instance.envPrefabs[Random.Range(0, LevelComponentKeeper.instance.envPrefabs.Length)]);
            go.transform.position = new Vector2(LevelGenerator.roomWidth*x, LevelGenerator.roomHeight*y) + 1.4f*Random.insideUnitCircle + LevelGenerator.environmentsCenters[Random.Range(0, locsLength)];
            go.transform.SetParent(TileCamera.ENVIRONMENT_ANCHOR);
            int numSmallObj = Random.Range(0, 12);
            for(int j = 0; j < numSmallObj; j++)
            {
                GameObject sgo = Object.Instantiate(LevelComponentKeeper.instance.addEnvPrefabs[Random.Range(0, LevelComponentKeeper.instance.addEnvPrefabs.Length)]);
                sgo.transform.position = go.transform.position + 1.5f * new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0);
                sgo.transform.SetParent(TileCamera.ENVIRONMENT_ANCHOR);
            }
            
        }
    }

    public void SpawnBonfires()
    {
        if (type != eRoomType.rest)
            return;
        GameObject go = Object.Instantiate(LevelComponentKeeper.instance.bonfire);
        go.transform.position = new Vector2(LevelGenerator.roomWidth * x, LevelGenerator.roomHeight * y) + LevelGenerator.environmentsCenters[LevelGenerator.environmentsCenters.Length - 1];
        go.transform.SetParent(TileCamera.BONFIRE_ANCHOR);
    }

}