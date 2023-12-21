#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class Mouse
    {
        private static Vector3 _prevFreeConstrPos;
        private static Vector3 _prevSnappedConstrPos;
        private static Vector3 _prevFreeWorldPos;
        private static Vector3 _prevSnappedWorldPos;
        
        public  static Vector3 FreeDirection_Constr { get; private set; }
        public  static Vector3 SnappedDirection_Constr { get; private set; }
        public  static Vector3 FreeDirection_World { get; private set; }
        public  static Vector3 SnappedDirection_World { get; private set; }
        
        public  static bool IsSnappedToEnd;
        public  static bool IsMouseInScene { get; set; }

        public  static Ray WorldRay { get; private set; }
        public  static Vector3 FreeWorldPos { get; private set; }
        public  static Vector3 SnappedWorldPos { get; private set; }
        public  static Vector3 FreeConstrPos { get; private set; }
        public  static Vector3 SnappedConstrPos { get; private set; }

        public  static Func<Vector3> CustomSnappingFunction { get; set; }
        public  static bool IsPointerInSceneView { get; set; }


         
         
         
         
         
         
        public  static void CalcFreePosition( Vector3 rawMousePosition )
        {
            if ( !IsMouseInScene ) return;
            FreeWorldPos = GetPointOnPlane( rawMousePosition, MbsGrid.Transform.up, MbsGrid.Position_World );
            FreeConstrPos = MbsGrid.Transform.InverseTransformPoint( FreeWorldPos );

            FreeDirection_World = FreeWorldPos - _prevFreeWorldPos;
            FreeDirection_Constr = FreeConstrPos - _prevFreeConstrPos;

            _prevFreeWorldPos = FreeWorldPos;
            _prevFreeConstrPos = FreeConstrPos;
        }

         
         
         
         
         
         
         
         
         
         
         
        public  static void CalcSnappedPosition( )
        {
            if ( !IsMouseInScene )
                return;

            if ( CustomSnappingFunction == null )
            {
                SnappedConstrPos = FreeConstrPos;
                SnappedWorldPos = FreeWorldPos;
              
            }
            else
            {
                SnappedConstrPos = CustomSnappingFunction.Invoke( );
                SnappedWorldPos = MbsGrid.Transform.TransformPoint( SnappedConstrPos );

                CustomSnappingFunction = null;
            }

            SnappedDirection_World = SnappedWorldPos - _prevSnappedWorldPos;
            SnappedDirection_Constr = SnappedConstrPos - _prevSnappedConstrPos;

            _prevSnappedWorldPos = SnappedWorldPos;
            _prevSnappedConstrPos = SnappedConstrPos;
        }

         
         
         
         
         
         
        public  static Vector3 GetPointOnPlane( Vector3 rawMousePosition, Vector3 planeNormal, Vector3 planePosition )
        {
            var mousePosition = rawMousePosition;
            WorldRay = HandleUtility.GUIPointToWorldRay( mousePosition );
            var plane = new Plane( planeNormal, planePosition );

            if ( plane.Raycast( WorldRay, out var distance ) )
                mousePosition = WorldRay.GetPoint( distance );
            else
                mousePosition = Vector3.zero;

            return mousePosition;
        }
    }
}

#endif