#if UNITY_EDITOR

using System;
using MBS.Code.Utilities.Helpers;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBS.Controller.Scene
{
    public  enum DecoratorRotationAxis
    {
        YAxis,
        XAxis,
        ZAxis
    }

    public  enum RotationSnapStep
    {
        Ninety,
        Fifteen,
        One,
        OneTenth
    }

    public  static class Decorator_RotationMode
    {
        public  static string[ ] AxisTextArray = { "Y Axis", "X Axis", "Z Axis" };
        public  static float[ ] SnapStepValueArray = { 90.0f, 15.0f, 1.0f, 0.1f };

        private static float _staticColorLerp = 0.7f;
        private static Color _staticColor = new Color( 0, 0, 0, 0.3f );

        private static Color[ ] _axisColors =
        {
            Handles.yAxisColor,
            Handles.xAxisColor,
            Handles.zAxisColor
        };

        private static readonly Vector3[ ] _axisVector = new Vector3[ 3 ];
        private static readonly float _passiveAxisThickness = 2f;
        private static readonly float _activeAxisThickness = 5f;


        public  static DecoratorRotationAxis Axis;
        public  static RotationSnapStep SnapStep;
        public  static Space RotationSpace;

        public  static bool DoSnapRotation;

        public  static Quaternion initialRotation;
        public  static Vector3 initialExtents;


        public  static void Start( Transform transform, Bounds worldBounds )
        {
            initialRotation = transform.rotation;
            initialExtents = worldBounds.extents;
        }

        public  static void Run( Transform transform, Bounds worldBounds )
        {
            var worldRay = Mouse.WorldRay;
            var snapStep = SnapStepValueArray[ (int)SnapStep ];

            transform.rotation = initialRotation;

            Vector3 objDirection;
            Vector3 objDirectionFULL;
            Vector3 rotateAroundAxis;
            Vector3 rotateAroundAxisFULL;

            switch ( Axis )
            {
                case DecoratorRotationAxis.YAxis:
                    objDirection = Vector3.forward;
                    rotateAroundAxis = Vector3.up;
                    break;

                case DecoratorRotationAxis.XAxis:
                    objDirection = Vector3.up;
                    rotateAroundAxis = Vector3.right;
                    break;

                case DecoratorRotationAxis.ZAxis:
                    objDirection = Vector3.up;
                    rotateAroundAxis = Vector3.forward;
                    break;

                default:
                    Debug.LogError( "MBS. Decorator: Rotation Mode. Unexpected behavior." );
                    return;
            }

            #region RotationLogic

            if ( RotationSpace == Space.Self )
            {
                objDirectionFULL = transform.rotation * objDirection;
                rotateAroundAxisFULL = transform.rotation * rotateAroundAxis;
            }
            else
            {
                objDirectionFULL = transform.rotation * objDirection;
                rotateAroundAxisFULL = rotateAroundAxis;
            }

            Vector3 mousePositionOnPlane;

            var plane = new Plane( rotateAroundAxisFULL, transform.position );

            if ( plane.Raycast( worldRay, out var distance ) )
                mousePositionOnPlane = worldRay.GetPoint( distance );
            else
                mousePositionOnPlane = Vector3.zero;

            var toMouseDirection = mousePositionOnPlane - transform.position;
            var projectedOnPlane =
                Vector3.ProjectOnPlane( objDirectionFULL, rotateAroundAxisFULL );
            var angleBetweenMouseAndForward =
                Vector3.SignedAngle( projectedOnPlane, toMouseDirection, rotateAroundAxisFULL );

            if ( angleBetweenMouseAndForward < 0 )
                angleBetweenMouseAndForward = 360 - ( angleBetweenMouseAndForward * -1 );

            if ( DoSnapRotation )
                angleBetweenMouseAndForward = Mathf.Round( angleBetweenMouseAndForward / snapStep ) * snapStep;

            transform.Rotate( rotateAroundAxis, angleBetweenMouseAndForward, RotationSpace );

            #endregion

            #region Handles

            if ( RotationSpace == Space.Self )
            {
                _axisVector[ 0 ] = transform.up;
                _axisVector[ 1 ] = transform.right;
                _axisVector[ 2 ] = transform.forward;
            }
            else
            {
                _axisVector[ 0 ] = Vector3.up;
                _axisVector[ 1 ] = Vector3.right;
                _axisVector[ 2 ] = Vector3.forward;
            }

            var cameraVector = Handles.inverseMatrix.MultiplyVector(
                (Object)Camera.current != (Object)null ? Camera.current.transform.forward : Vector3.forward );

            var size = HandleUtility.GetHandleSize( transform.position ) * 2 * initialExtents.magnitude;

            for ( int i = 0; i < 3; i++ )
            {
                Handles.color = GetAxisColor( i, Axis );

                if ( i == (int)Axis )
                {
                    var zTest = Handles.zTest;
                     
                    var p1 = transform.position - ( _axisVector[ i ] * size * 2 );
                    var p2 = transform.position + ( _axisVector[ i ] * size * 2 );
                    Handles.DrawDottedLine( p1, p2, 5 );
                    Handles.zTest = zTest;
                }

                var crossNormalized = Vector3.Cross( _axisVector[ i ], cameraVector ).normalized;
                var thickness = GetAxisThickness( i, Axis );
                Handles.DrawWireArc( transform.position, _axisVector[ i ], crossNormalized,
                                     180f, size, thickness );
            }

            Handles.BeginGUI( );
            {
                var style = new GUIStyle( "button" );
                style.alignment = TextAnchor.MiddleLeft;
                style.richText = true;
                var text = angleBetweenMouseAndForward + "°";
                text = "<b>" + text + "</b>";

                if ( DoSnapRotation )
                    text += '\n' + "Snap step: " + SnapStepValueArray[ (int)SnapStep ] + "°";

                var rectSize = style.CalcSize( new GUIContent( text ) ) * 1.2f;
                var screenPos = HandleUtility.WorldToGUIPoint( transform.position + Vector3.right * size );
                var mouseOffset = new Vector2( 30, 0 );
                var rect = new Rect( screenPos.x + mouseOffset.x, screenPos.y, rectSize.x, rectSize.y );

                GUI.backgroundColor = new Color( 1.0f, 1.0f, 1.0f, 0.85f );
                GUI.Box( rect, text, style );
                GUI.backgroundColor = Color.white;
            }
            Handles.EndGUI( );
            Handles.PositionHandle( transform.position, transform.rotation );

            #endregion
        }


        public  static void Cancel( )
        {
            Clear( );
        }

        public  static Quaternion Accept( Transform transform )
        {
            var temp_value = transform.rotation;
            Clear( );
            return temp_value;
        }

        public  static void ChangeSpace_Next( )
        {
            var curIndex = (int)RotationSpace;
            var enumLength = Enum.GetNames( typeof( Space ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            RotationSpace = (Space)nextTypeIndex;
        }

        public  static void ChangeAxis_Next( )
        {
            var curIndex = (int)Axis;
            var enumLength = Enum.GetNames( typeof( DecoratorRotationAxis ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            Axis = (DecoratorRotationAxis)nextTypeIndex;
        }

        public  static void ChangeSnapAccuracy_Next( )
        {
            var curIndex = (int)SnapStep;
            var enumLength = Enum.GetNames( typeof( RotationSnapStep ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            SnapStep = (RotationSnapStep)nextTypeIndex;
        }

        private static Color GetAxisColor( int index, DecoratorRotationAxis curAxis )
        {
            var curAxisIndex = (int)curAxis;
            var multiplier = ( index == curAxisIndex ) ? 1 : _staticColorLerp;
            var color = Color.Lerp( _staticColor, _axisColors[ index ], multiplier );
            return color;
        }

        private static float GetAxisThickness( int index, DecoratorRotationAxis curAxis )
        {
            var curAxisIndex = (int)curAxis;
            var thickness = ( index == curAxisIndex ) ? _activeAxisThickness : _passiveAxisThickness;
            return thickness;
        }

        public  static void Clear( )
        {
            Axis = default;
            SnapStep = default;
            RotationSpace = default;

            DoSnapRotation = default;

            initialRotation = default;
            initialExtents = default;
        }
    }
}
#endif