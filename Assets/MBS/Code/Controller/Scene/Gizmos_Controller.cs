#if UNITY_EDITOR

using System;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MBS.Controller.Scene
{
    public  static class Gizmos_Controller
    {
        private const float LINE_OFFSET = 0.2f;
        private const float LINE_LENGTH = 0.5f;

        private const int LINE_WIDTH = 1;
        private const int LINE_ACCENT_WIDTH = 4;

        private static readonly Color DEFAULT_COLOR = Color.green;
        private static readonly Color ACCENT_COLOR = new Color( 1, .4f, 0, 1 );

        private static Color _prevColorGizmos;
        private static Color _prevColorHandles;

        private static Color _currentColor;
        private static Action _currentDrawAction;

        private static float _xAxisWidth = LINE_WIDTH;
        private static float _yAxisWidth = LINE_WIDTH;
        private static float _zAxisWidth = LINE_WIDTH;

        private static Matrix4x4 _initialGizmoMatrix;
        private static Matrix4x4 _initialHandlesMatrix;

        public  static Mesh Mesh;
        public  static Matrix4x4 Matrix;

        public  static Vector3 Position;
        public  static Quaternion Rotation;
        public  static bool DefaultOrAccent;

        public  static Vector3 ObjBoundsSize;
        public  static LeadAxis LearAxis;


        public  static void Draw( )
        {
            if ( Matrix == default ) return;

            SetColor( );

            _currentDrawAction?.Invoke( );

            ResetColor( );

            _xAxisWidth = LINE_WIDTH;
            _yAxisWidth = LINE_WIDTH;
            _zAxisWidth = LINE_WIDTH;
        }


        public  static void Clear( )
        {
            Mesh = null;
            _currentColor = Color.white;
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        public  static void SetupMesh( Mesh mesh )
        {
            Mesh = mesh;
        }


        public  static void SetupMeshDrawFunction( )
        {
            _currentDrawAction = DrawMesh;
        }

        public  static void SetupPickerDrawFunction( )
        {
            _currentDrawAction = DrawPickerModeGizmo;

            Matrix = default;
            _initialGizmoMatrix = Matrix4x4.identity;
            _initialHandlesMatrix = Matrix4x4.identity;
        }

        public  static void SetupPointerDrawFunction( )
        {
            _currentDrawAction = DrawPointerModeGizmo;

            Matrix = default;
            _initialGizmoMatrix = Matrix4x4.identity;
            _initialHandlesMatrix = Matrix4x4.identity;
        }


        public  static void SetWallData( float height )
        {
            SetupMesh( InternalDataManager.Singleton.WallGizmoMesh );

            if ( ReferenceEquals( Mesh, null ) )
            {
                Debug.LogError( Texts.Building.Gizmos.WALL_BUILDING_GIZMO_MISSING );
                return;
            }

            var vertices = Mesh.vertices;
            var colors = Mesh.colors;
            var extendVertColor = new Color( 1, 1, 0 );

            height += .2f;
            height = Mathf.Max( height, 1 );


            for ( var i = 0; i < vertices.Length; i++ )
                if ( colors[ i ] == extendVertColor )
                    vertices[ i ] = new Vector3( vertices[ i ].x, height, vertices[ i ].z );

            Mesh.SetVertices( vertices );
        }


        private static void DrawMesh( )
        {
            if ( ReferenceEquals( Mesh, null ) )
                return;

            SetMatrix( );
            Gizmos.DrawMesh( Mesh, 0, Position, Rotation );
            ResetMatrix( );
        }
        

        private static void DrawPickerModeGizmo( )
        {
            SetMatrix( );

            Handles.zTest = CompareFunction.Always;

            var xAxis0 = Vector3.zero;
            var xAxis1 = Vector3.right * LINE_LENGTH;

            var yAxis0 = Vector3.zero;
            var yAxis1 = Vector3.up * LINE_LENGTH;

            var zAxis0 = Vector3.zero;
            var zAxis1 = Vector3.forward * LINE_LENGTH;

            switch ( LearAxis )
            {
                case LeadAxis.XAxis:
                    xAxis0 = -Vector3.right * LINE_LENGTH;
                    _xAxisWidth = LINE_ACCENT_WIDTH;
                    break;
                case LeadAxis.YAxis:
                    yAxis0 = -Vector3.up * LINE_LENGTH;
                    _yAxisWidth = LINE_ACCENT_WIDTH;
                    break;
                case LeadAxis.ZAxis:
                    zAxis0 = -Vector3.forward * LINE_LENGTH;
                    _zAxisWidth = LINE_ACCENT_WIDTH;
                    break;
            }

            Handles.SphereHandleCap( 0, Vector3.zero, Quaternion.identity, 0.25f, EventType.Repaint );

            Handles.color = Color.red;
            Handles.DrawLine( xAxis0, xAxis1, _xAxisWidth );

            Handles.color = Color.green;
            Handles.DrawLine( yAxis0, yAxis1, _yAxisWidth );

            Handles.color = Color.blue;
            Handles.DrawLine( zAxis0, zAxis1, _zAxisWidth );

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube( Vector3.zero, ObjBoundsSize );

            ResetMatrix( );
        }

        private static void DrawPointerModeGizmo( )
        {
            if ( Matrix == default ) return;

            SetMatrix( );

            Handles.zTest = CompareFunction.Always;

            var f = .3f;
            var xAxis0 = -Vector3.right * f;
            var xAxis1 = Vector3.right * f;

            var yAxis0 = -Vector3.up * f;
            var yAxis1 = Vector3.up * f;

            var zAxis0 = -Vector3.forward * f;
            var zAxis1 = Vector3.forward * f;


            Handles.color = Color.white;
            Handles.DrawLine( xAxis0, xAxis1, 2 );
            Handles.DrawLine( yAxis0, yAxis1, 2 );
            Handles.DrawLine( zAxis0, zAxis1, 2 );

            Handles.color = _currentColor;
            Handles.SphereHandleCap( 0, Vector3.zero, Quaternion.identity, 0.2f, EventType.Repaint );

            ResetMatrix( );
        }


        private static void SetMatrix( )
        {
            _initialGizmoMatrix = Gizmos.matrix;
            _initialHandlesMatrix = Handles.matrix;

            Gizmos.matrix = Matrix;
            Handles.matrix = Matrix;
        }

        private static void ResetMatrix( )
        {
            Gizmos.matrix = _initialGizmoMatrix;
            Handles.matrix = _initialHandlesMatrix;
            _initialGizmoMatrix = default;
            _initialHandlesMatrix = default;
            Matrix = default;
        }


        private static void SetColor( )
        {
            _prevColorGizmos = Gizmos.color;
            _prevColorHandles = Gizmos.color;

            if ( DefaultOrAccent == false )
                _currentColor = DEFAULT_COLOR;
            else
                _currentColor = ACCENT_COLOR;

            Gizmos.color = _currentColor;
            Handles.color = _currentColor;
        }

        private static void ResetColor( )
        {
            Gizmos.color = _prevColorGizmos;
            Handles.color = _prevColorHandles;

            _currentColor = default;
        }
    }
}

#endif