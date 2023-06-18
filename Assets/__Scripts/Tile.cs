using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Set Dynamically")]
    public int x;
    public int y;
    public int tileNum;
    private BoxCollider bColl;

    void Awake() {
        bColl = GetComponent<BoxCollider>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1) 
    {
        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3")+"x"+y.ToString("D3");
        if (eTileNum == -1) 
        {
            eTileNum = TileCamera.GET_MAP(x,y);
        }
        else {
            TileCamera.SET_MAP(x, y, eTileNum); 
        }
        tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum]; 
    }
}
