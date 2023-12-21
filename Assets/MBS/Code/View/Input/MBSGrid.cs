#if UNITY_EDITOR


using System.Collections.Generic;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MBS.View.Input
{
    public static class MbsGrid
    {
        private struct GridLine
        {
            public float width;
            public Vector3[ ] points;
            public Color color;
        }

        private static Color _col1 = new Color( .3f, .3f, .3f, 1f );
        private static Color _col2 = new Color( .6f, .6f, .6f, 0.2f );

        private static List<GridLine> _gridLines;
        private static GridLine[ ] _centerLines;
        private static Vector3 _offsetPosition;
        private static Transform _transform;

        private static float _prevSize;
        private static float _prevCellSize;
        private static Vector3 _prevPositionLocal;


        public static float Size { get; set; } = 50;
        public static float CellSize { get; set; } = 1;


        public static Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }

        public static Vector3 Position_Local
        {
            get => _offsetPosition + HeightVector;
            set => _offsetPosition = new Vector3( value.x, 0, value.z );
        }

        public static Vector3 Position_World
        {
            get
            {
                if ( Transform != null )
                    return Transform.TransformPoint( Position_Local );

                return Vector3.zero;
            }
        }


        private static Vector3 HeightVector => Vector3.up * ( LevelHeight * LevelNumber );
        public static float LevelHeight { get; set; }
        public static int LevelNumber { get; set; }


        public static void Start( )
        {
            RecalculateGrid( );
        }

        private static void RecalculateGrid( )
        {
            CaclLinesNumber( );
            CalcLinesPoints( );
            CalcCenterLines( );

            if ( EditorWindow.HasOpenInstances<SceneView>( ) )
            {
                var view = EditorWindow.GetWindow<SceneView>( );
                view.Repaint( );
            }

            _prevPositionLocal = Position_Local;
            _prevCellSize = CellSize;
            _prevSize = Size;
        }

        private static void CaclLinesNumber( )
        {
            var lineNumber = Mathf.FloorToInt( Size / CellSize );
            lineNumber = Mathf.Clamp( lineNumber, 1, int.MaxValue );

            if ( lineNumber % 2 == 0 )
                lineNumber = lineNumber + 1;

            _gridLines = new List<GridLine>( lineNumber );
        }

        private static void CalcLinesPoints( )
        {
            var halfSize = Size / 2;

            var halfCapacity = Mathf.CeilToInt( _gridLines.Capacity / 2f );

            var lineColor = Color.white;
            float lineWidth = 1;

            var primeColor = new Color( .3f, .3f, .3f, 1f );
            var secondColor = new Color( .6f, .6f, .6f, 0.5f );
            var primeWidth = 3;
            var secondWidth = 2;

            var config = Config.Sgt;
            if ( config != null )
            {
                primeColor = config.GridPrimeLineColor;
                secondColor = config.GridSecondLineColor;
                primeWidth = config.GridPrimeLineWidth;
                secondWidth = config.GridSecondLineWidth;
            }


            for ( var i = 0; i < halfCapacity - 1; i++ )
            {
                var xPoint = i * CellSize;
                var cos = xPoint / halfSize;
                var angle = Mathf.Acos( cos );

                if ( i == 0 || i % 5 == 0 )
                {
                    lineWidth = primeWidth;
                    lineColor = primeColor;
                }
                else
                {
                    lineWidth = secondWidth;
                    lineColor = secondColor;
                }

                var linePoints = new Vector3[ 2 ];

                linePoints[ 0 ] = new Vector3( i * CellSize, 0, Mathf.Sin( angle ) * -halfSize );
                linePoints[ 1 ] = new Vector3( i * CellSize, 0, Mathf.Sin( angle ) * halfSize );

                linePoints[ 0 ] += Position_Local;
                linePoints[ 1 ] += Position_Local;

                _gridLines.Add( new GridLine
                {
                    width = lineWidth,
                    points = linePoints,
                    color = lineColor
                } );

                if ( i != 0 )
                {
                    var linePoints2 = new Vector3[ 2 ];

                    linePoints2[ 0 ] = new Vector3( -i * CellSize, 0, Mathf.Sin( angle ) * -halfSize );
                    linePoints2[ 1 ] = new Vector3( -i * CellSize, 0, Mathf.Sin( angle ) * halfSize );

                    linePoints2[ 0 ] += Position_Local;
                    linePoints2[ 1 ] += Position_Local;

                    _gridLines.Add( new GridLine
                    {
                        width = lineWidth,
                        points = linePoints2,
                        color = lineColor
                    } );
                }
            }


            for ( var i = 0; i < halfCapacity - 1; i++ )
            {
                var yPoint = i * CellSize;
                var cos = yPoint / halfSize;
                var angle = Mathf.Acos( cos );

                if ( i == 0 || i % 5 == 0 )
                {
                    lineWidth = primeWidth;
                    lineColor = primeColor;
                }
                else
                {
                    lineWidth = secondWidth;
                    lineColor = secondColor;
                }

                var linePoints = new Vector3[ 2 ];

                linePoints[ 0 ] = new Vector3( Mathf.Sin( angle ) * -halfSize, 0, i * CellSize );
                linePoints[ 1 ] = new Vector3( Mathf.Sin( angle ) * halfSize, 0, i * CellSize );

                linePoints[ 0 ] += Position_Local;
                linePoints[ 1 ] += Position_Local;

                GridLine gridLine = new GridLine
                {
                    width = lineWidth,
                    points = linePoints,
                    color = lineColor
                };

                _gridLines.Add( gridLine );

                if ( i != 0 )
                {
                    var linePoints2 = new Vector3[ 2 ];

                    linePoints2[ 0 ] = new Vector3( Mathf.Sin( angle ) * -halfSize, 0, -i * CellSize );
                    linePoints2[ 1 ] = new Vector3( Mathf.Sin( angle ) * halfSize, 0, -i * CellSize );

                    linePoints2[ 0 ] += Position_Local;
                    linePoints2[ 1 ] += Position_Local;

                    _gridLines.Add( new GridLine
                    {
                        width = lineWidth,
                        points = linePoints2,
                        color = lineColor
                    } );
                }
            }
        }

        private static void CalcCenterLines( )
        {
            _centerLines = new GridLine[ 3 ];

            var offset = Position_Local;

            _centerLines[ 0 ] = new GridLine
            {
                width = 6,
                points = new[ ]
                {
                    offset,
                    offset + ( Vector3.right )
                },
                color = Color.red
            };

            _centerLines[ 1 ] = new GridLine
            {
                width = 6,
                points = new[ ]
                {
                    offset,
                    offset + ( Vector3.forward )
                },
                color = Color.blue
            };

            _centerLines[ 2 ] = new GridLine
            {
                width = 6,
                points = new[ ]
                {
                    offset,
                    offset + Vector3.zero + ( Vector3.up )
                },
                color = Color.green
            };
        }


        public static void Draw( )
        {
            if ( _prevSize.ApxEquals( Size ) == false ||
                 _prevCellSize.ApxEquals( CellSize ) == false ||
                 _prevPositionLocal.ApxEquals( Position_Local ) == false )
            {
                RecalculateGrid( );
                return;
            }

            var origMatrix = Handles.matrix;

            Handles.matrix = Transform.localToWorldMatrix;

             
            DrawGridCenter( );

            Handles.zTest = CompareFunction.LessEqual;
            DrawLines( );

            Handles.matrix = origMatrix;
        }

        private static void DrawLines( )
        {
            for ( var i = 0; i < _gridLines.Count; i++ )
            {
                Handles.color = _gridLines[ i ].color;
                Handles.DrawAAPolyLine( _gridLines[i].width, _gridLines[ i ].points );
            }
        }

        private static void DrawGridCenter( )
        {
            Handles.color = Color.white;
            Handles.SphereHandleCap( 0, Position_Local, Quaternion.identity, 0.1f, EventType.Repaint );
            for ( var i = 0; i < _centerLines.Length; i++ )
            {
                Handles.color = _centerLines[ i ].color;
                Handles.DrawAAPolyLine( 4f, _centerLines[ i ].points );
            }
        }


        public static Vector3 Snap_ToBeginning( Vector3 pos_Constr )
        {
            return new Vector3(
                Mathf.Round( ( pos_Constr.x - _offsetPosition.x ) / CellSize ) * CellSize + _offsetPosition.x,
                LevelHeight * LevelNumber,
                Mathf.Round( ( pos_Constr.z - _offsetPosition.z ) / CellSize ) * CellSize + _offsetPosition.z
            );
        }

        public static Vector3 Snap_ToCenter( Vector3 pos_Constr )
        {
            var halfCell = ( CellSize / 2 );
            return new Vector3(
                Mathf.Floor( ( pos_Constr.x - _offsetPosition.x ) / CellSize ) * CellSize + halfCell +
                _offsetPosition.x,
                LevelHeight * LevelNumber,
                Mathf.Floor( ( pos_Constr.z - _offsetPosition.z ) / CellSize ) * CellSize + halfCell + _offsetPosition.z
            );
        }


        public static void Clear( )
        {
            _gridLines = null;
            _centerLines = null;
            _offsetPosition = default;
            _transform = null;

            _prevSize = 50;
            _prevCellSize = 1;
            _prevPositionLocal = default;

            Size = 50;
            CellSize = 1;

            Transform = null;
            Position_Local = default;

            LevelHeight = default;
            LevelNumber = default;
        }
    }
}

#endif