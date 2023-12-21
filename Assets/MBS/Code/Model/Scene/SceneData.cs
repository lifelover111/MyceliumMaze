#if UNITY_EDITOR

using MBS.View.Builder;
using MBS.View.Input.Physical;
using UnityEngine;

namespace MBS.Model.Scene
{
    public  static class SceneData
    {
        public  static bool DoStretchWalls { get; set; }

        public  static bool IsCollisionOn { get; set; }


        public  static SceneMode SceneMode { get; set; }
        public  static BuilderMode BuilderMode { get; set; }
        public  static bool DoSnapToGrid { get; set; }
        public  static bool DoSnapToEnds { get; set; }
        public  static Vector3 StartPositionConstr { get; set; }
        public  static bool IsMouseRecalcNeeded { get; set; }


        public  static void CaptureStartPosition_Snapped( )
        {
            StartPositionConstr = Mouse.SnappedConstrPos;
        }

        public  static void CaptureStartPosition_Free( )
        {
            StartPositionConstr = Mouse.FreeConstrPos;
        }
        
        public  static void Clear( )
        {
            DoStretchWalls = false;
            IsCollisionOn = false;
            SceneMode = default;
            BuilderMode = default;
            DoSnapToGrid = true;
            DoSnapToEnds = true;
            StartPositionConstr = Vector3.zero;
            IsMouseRecalcNeeded = false;
        }
    }
}

#endif