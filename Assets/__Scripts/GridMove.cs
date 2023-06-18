using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    private IFacingMover mover;

    void Awake() 
    {
        mover = GetComponent<IFacingMover>();
    }   

    void FixedUpdate() 
    {
        if ( !mover.moving ) 
            return;
        int facing = mover.GetFacing();
        Vector2 rPos = mover.roomPos;
        Vector2 rPosGrid = mover.GetRoomPosOnGrid();
        Vector2 delta;
        
        delta = rPosGrid - rPos;
        
        if (delta.magnitude == 0) return;
    
        float move = mover.GetSpeed() * Time.fixedDeltaTime;
        move = Mathf.Min( move, delta.magnitude );
        
        rPos += move*delta;
        mover.roomPos = rPos;
    }
}