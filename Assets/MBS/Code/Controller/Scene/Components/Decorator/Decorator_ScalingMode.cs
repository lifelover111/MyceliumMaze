#if UNITY_EDITOR

using System;
using MBS.Code.Utilities.Helpers;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  static class Decorator_ScalingMode
    {
        public  enum ScalingAxis { All, XAxis, YAxis, ZAxis }

        public  enum ScalingSnapAccuracy { OneTenth, OneHundredth, OneThousands }

        public  static string[ ] ScalingAxisText_Array = { "All", "X Axis", "Y Axis", "Z Axis" };
        public  static float[ ] ScalingSnapAccuracy_ValuesArray = { 0.1f, 0.01f, 0.001f };
        private static string[ ] _guiValueRound = { "F1", "F2", "F3" };

        public  static ScalingAxis Axis;
        public  static ScalingSnapAccuracy SnapAccuracy;

        public  static Vector3 initialScale;
        public  static Vector3 currentScale;


        public  static void Start( Transform transform )
        {
            Clear( );
            initialScale = transform.localScale;
            currentScale = initialScale;
        }

        public  static void Run( Transform transform )
        {
            var mousePos = MousePlanePosition( transform.position, Mouse.WorldRay );
            var snappingAccuracy = ScalingSnapAccuracy_ValuesArray[ (int)SnapAccuracy ];

            var distance = Vector3.Distance( mousePos, transform.position );
            distance = Mathf.Round( distance / snappingAccuracy ) * snappingAccuracy;

            var guiTextRound = _guiValueRound[ (int)SnapAccuracy ];

            switch ( Axis )
            {
                case ScalingAxis.All:
                    transform.localScale = new Vector3(
                        Mathf.Round( currentScale.x * distance / snappingAccuracy ) * snappingAccuracy,
                        Mathf.Round( currentScale.y * distance / snappingAccuracy ) * snappingAccuracy,
                        Mathf.Round( currentScale.z * distance / snappingAccuracy ) * snappingAccuracy );
                    break;
                case ScalingAxis.XAxis:
                    transform.localScale = new Vector3(
                        Mathf.Round( currentScale.x * distance / snappingAccuracy ) * snappingAccuracy,
                        currentScale.y,
                        currentScale.z );
                    break;
                case ScalingAxis.YAxis:
                    transform.localScale = new Vector3(
                        currentScale.x,
                        Mathf.Round( currentScale.y * distance / snappingAccuracy ) * snappingAccuracy,
                        currentScale.z );
                    break;
                case ScalingAxis.ZAxis:
                    transform.localScale = new Vector3(
                        currentScale.x,
                        currentScale.y,
                        Mathf.Round( currentScale.z * distance / snappingAccuracy ) * snappingAccuracy );
                    break;
            }

            Handles.BeginGUI( );
            {
                var style = new GUIStyle( "button" );
                style.alignment = TextAnchor.MiddleLeft;
                style.richText = true;

                var text = "Scale: " + transform.localScale.ToString( guiTextRound );
                text = "<b>" + text + "</b>";

                text += '\n' + "Snap step: " + snappingAccuracy.ToString( );
                 
                 

                var rectSize = style.CalcSize( new GUIContent( text ) ) * 1.2f;
                var screenPos = HandleUtility.WorldToGUIPoint( transform.position );
                var rect = new Rect( screenPos.x, screenPos.y, rectSize.x, rectSize.y );

                GUI.backgroundColor = new Color( 1.0f, 1.0f, 1.0f, 0.7f );
                GUI.Box( rect, text, style );
                GUI.backgroundColor = Color.white;
            }
            Handles.EndGUI( );
            
            Handles.PositionHandle( transform.position, transform.rotation );
        }

        public  static Vector3 Accept( Transform transform )
        {
            Clear( );
            return  transform.localScale;
        }

        public  static void Cancel( Transform transform )
        {
            Reset( transform );
            Clear( );
        }

        public  static void Clear( )
        {
            Axis = default;
            SnapAccuracy = default;
        }

        public  static void Reset( Transform transform )
        {
            transform.localScale = initialScale;
        }


        private static Vector3 MousePlanePosition( Vector3 position, Ray worldRay )
        {
            var plane = new Plane( Vector3.up, position );

            if ( plane.Raycast( worldRay, out var distance ) )
            {
                var floorPoint = worldRay.GetPoint( distance );

                Handles.color = Color.cyan;
                Handles.SphereHandleCap( 0, floorPoint, Quaternion.identity, 0.3f, EventType.Repaint );

                return floorPoint;
            }

            return Vector3.zero;
        }


        public  static void ChangeAxis_Next( )
        {
            var curIndex = (int)Axis;
            var enumLength = Enum.GetNames( typeof( ScalingAxis ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            Axis = (ScalingAxis)nextTypeIndex;
        }

        public  static void ChangeAccuracy_Next( )
        {
            var curIndex = (int)SnapAccuracy;
            var enumLength = Enum.GetNames( typeof( ScalingSnapAccuracy ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            SnapAccuracy = (ScalingSnapAccuracy)nextTypeIndex;
        }
    }
}
#endif