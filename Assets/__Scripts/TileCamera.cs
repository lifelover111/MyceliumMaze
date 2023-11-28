using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileCamera : MonoBehaviour 
{
    static private int W, H;
    static private int[,] MAP;
    static public Sprite[] SPRITES;
    static public Transform TILE_ANCHOR;
    static public Transform DOOR_ANCHOR;
    static public Transform WALL_ANCHOR;
    static public Transform ENEMY_ANCHOR;
    static public Transform ENVIRONMENT_ANCHOR;
    static public Transform BONFIRE_ANCHOR;
    static public Tile[,] TILES;
    static public OldProject.Room[,] level;
    static public List<InRoom> enemyInrms;

    static public InRoom heroInrm;

    [Header("Set in Inspector")]
    public Texture2D grassTiles;
    public GameObject tilePrefab;

    void Awake() 
    {
        enemyInrms = new List<InRoom>();
        LevelGenerator.Init(GameManager.seed);
    }
    
    private void Start()
    {
        heroInrm = HeroKeeper.instance.heroList[0].GetComponent<InRoom>();
        LoadMap();
    }

    public void LoadMap()
    {
        GameObject go = new GameObject("TILE_ANCHOR");
        TILE_ANCHOR = go.transform;
        go = new GameObject("DOOR_ANCHOR");
        DOOR_ANCHOR = go.transform;
        go = new GameObject("WALL_ANCHOR");
        WALL_ANCHOR = go.transform;
        go = new GameObject("ENEMY_ANCHOR");
        ENEMY_ANCHOR = go.transform;
        go = new GameObject("ENVIRONMENT_ANCHOR");
        ENVIRONMENT_ANCHOR = go.transform;
        go = new GameObject("BONFIRE_ANCHOR");
        BONFIRE_ANCHOR = go.transform;
        SPRITES = Resources.LoadAll<Sprite>(grassTiles.name);
        W = LevelGenerator.W;
        H = LevelGenerator.H;
        MAP = new int[W, H];
        level = LevelGenerator.GenerateLevel();

        foreach (OldProject.Room room in level)
        {
            int[,] tileNums = room.GetIntPresentation();
            int roomH = tileNums.GetLength(0);
            int roomW = tileNums.GetLength(1);
            
            for (int j = 0; j < roomH; j++)
            {
                for (int i = 0; i < roomW; i++)
                {
                    MAP[i + room.x * LevelGenerator.roomWidth, j + room.y * LevelGenerator.roomHeight] = tileNums[j, i];
                }
            }

            room.SpawnDoors();
            room.SpawnWalls(1.8f, 2);
            room.SpawnEnemies();
            room.SpawnEnvironment();
            room.SpawnBonfires();
        }


        ShowMap();

        foreach (Hero hero in HeroKeeper.instance.heroList)
            hero.gameObject.transform.position = new Vector2((LevelGenerator.startRoom.x + 0.5f) * LevelGenerator.roomWidth, (LevelGenerator.startRoom.y + 0.3f) * LevelGenerator.roomHeight);
        gameObject.transform.position += new Vector3(LevelGenerator.startRoom.x * LevelGenerator.roomWidth, LevelGenerator.startRoom.y * LevelGenerator.roomHeight, 0);
    }

    void ShowMap()
    {
        TILES = new Tile[W,H];
        for (int j = 0; j < H; j++)
            for (int i = 0; i < W; i++)
            {
                GameObject ti = Instantiate<GameObject>(tilePrefab);
                ti.transform.SetParent(TILE_ANCHOR);
                Tile tmpTile = ti.GetComponent<Tile>();
                tmpTile.SetTile(i, j, MAP[i, j]);
                TILES[i, j] = tmpTile;
            }

        StaticBatchingUtility.Combine(TILE_ANCHOR.gameObject);
        StaticBatchingUtility.Combine(WALL_ANCHOR.gameObject);
        StaticBatchingUtility.Combine(DOOR_ANCHOR.gameObject);
        StaticBatchingUtility.Combine(ENVIRONMENT_ANCHOR.gameObject);
    }

    private void Update()
    {
        List<InRoom> currentInrms = new List<InRoom>();
        foreach (InRoom inrm in enemyInrms)
        {
            if (inrm.roomNum != heroInrm.roomNum)
                inrm.gameObject.SetActive(false);
            else
            {
                inrm.gameObject.SetActive(true);
                currentInrms.Add(inrm);
            }
        }
        foreach(InRoom inrm in currentInrms)
        {
            OldProject.Door.enemiesInCurrentRoom.Add(inrm);
            enemyInrms.Remove(inrm);
        }
    }

    static public int GET_MAP( int x, int y ) {
        if ( x<0 || x>=W || y<0 || y>=H ) {
            return -1;
        }
        return MAP[x,y];
    }

    static public int GET_MAP( float x, float y ) {
        int tX = Mathf.RoundToInt(x);
        int tY = Mathf.RoundToInt(y - 0.25f);
        return GET_MAP(tX,tY);
    }


    static public void SET_MAP( int x, int y, int tNum ) {
        if ( x<0 || x>=W || y<0 || y>=H ) {
            return;
        }
        MAP[x,y] = tNum;
    }
}