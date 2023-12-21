#if UNITY_EDITOR

using UnityEngine;

namespace MBS.Controller.Scene
{
    public  enum FloorCorner
    {
        TopLeft,
        TopRight,
        BotRight,
        BotLeft,
        All
    }

    public  static class Floor_MeshModifier
    {
        public  static void ModifyFloorCorner( GameObject floorPrefab, FloorCorner corner )
        {
            var localScale = floorPrefab.transform.localScale;
            switch ( corner )
            {
                case FloorCorner.TopRight:
                    return;
                case FloorCorner.BotRight:
                    localScale = new Vector3(
                        localScale.x,
                        localScale.y,
                        localScale.z * -1 );
                    floorPrefab.transform.localScale = localScale;
                    return;
                case FloorCorner.BotLeft:
                    localScale = new Vector3(
                        localScale.x * -1,
                        localScale.y,
                        localScale.z * -1 );
                    floorPrefab.transform.localScale = localScale;
                    return;
                case FloorCorner.TopLeft:
                    localScale = new Vector3(
                        localScale.x * -1,
                        localScale.y,
                        localScale.z );
                    floorPrefab.transform.localScale = localScale;
                    return;
            }


             
             
        }

         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
    }
}
#endif