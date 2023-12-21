#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Controller.Scene.Mono;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    [Serializable]
    public  class RoomArea : GeometryShape
    {
        [SerializeField] public  List<Vector3> wallEndPoints;
        [SerializeField] public  List<MBSWallModule> walls;
        [SerializeField] public  Vector2 areaBounds;
        [SerializeField] private float _height;
        [SerializeReference] public  List<Triangle> triangles;


        public  RoomArea( List<MBSWallModule> walls, List<Vector3> pathPointsConstuctorSpace,
                         bool doFaceElements = true )
        {
            if ( walls == null || walls.Count < 3 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_WALL_LESS_THAN_3 );
                return;
            }

            this.walls = walls;
            wallEndPoints = pathPointsConstuctorSpace;
            _height = pathPointsConstuctorSpace[ 0 ].y;

            RecalculateEssentialPoints( );
            RecalcTriangles( );
            RecalcBounds( );
            RecalculateArea( );

             
             
        }


        public  List<MBSWallModule> Walls
        {
            get
            {
                if ( walls == null )
                    walls = new List<MBSWallModule>( );
                else
                    walls = walls.Where( i => i != null ).ToList( );

                return walls;
            }

            set
            {
                if ( value == null )
                    return;
                walls = value;
            }
        }


        public  (float area, WindingOrder windingOrder) FindAreaAndWindingOrder( List<Vector3> areaExtremePoints )
        {
            var area = 0f;
            WindingOrder order;

            for ( var i = 0; i < areaExtremePoints.Count; i++ )
            {
                var va = areaExtremePoints[ i ];
                var vb = areaExtremePoints[ ( i + 1 ) % areaExtremePoints.Count ];

                var width = vb.x - va.x;
                var height = ( vb.z + va.z ) / 2;

                area += width * height;
            }

            if ( area > 0 )
                order = WindingOrder.Clockwise;
            else if ( area < 0 )
                order = WindingOrder.CounterClockise;
            else
                order = WindingOrder.Invalid;


            return ( Mathf.Abs( area ), order );
        }


        public  List<Vector3> GetItemEndPoints( )
        {
            var retval = new List<Vector3>( );
            for ( var i = 0; i < Walls.Count; i++ ) retval.Add( Walls[ i ].rearEndPointConstructorSpace );
            return wallEndPoints;
        }

        public  List<int> GetEssentialPointsIndexes( )
        {
            if ( wallEndPoints == null || wallEndPoints.Count < 3 )
                return null;

            int prev, next;
            var retval = new List<int>( );

            for ( var i = 0; i < wallEndPoints.Count; i++ )
            {
                prev = Collections_Helper.GetPrevLoopIndex( i, wallEndPoints.Count );
                next = Collections_Helper.GetNextLoopIndex( i, wallEndPoints.Count );

                var a = wallEndPoints[ prev ] - wallEndPoints[ i ];
                var b = wallEndPoints[ next ] - wallEndPoints[ i ];

                var angle = Mathf.Abs( Vector3.SignedAngle( b, a, Vector3.up ) );

                if ( angle != 0 && angle % 180 != 0 ) retval.Add( i );
            }

            return retval;
        }

        public  static Vector3[ ] GetExtremePoints( List<Vector3> pathPoints )
        {
            if ( pathPoints == null || pathPoints.Count <= 2 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_POINTS  );
                return null;
            }

            var retval = new List<Vector3>( );
            int prev, next;
            for ( var i = 0; i < pathPoints.Count; i++ )
            {
                prev = i - 1 < 0 ? pathPoints.Count - 1 : i - 1;
                next = ( i + 1 ) % pathPoints.Count;
                var a = pathPoints[ i ] - pathPoints[ prev ];
                var b = pathPoints[ i ] - pathPoints[ next ];
                var angle = Vector3.Angle( b, a );
                if ( angle != 0 && angle % 180 != 0 ) retval.Add( pathPoints[ i ] );
            }

            return retval.ToArray( );
        }

        public  Vector3[ ] RecalculateEssentialPoints( )
        {
            var essentialPointsIndexes = GetEssentialPointsIndexes( );

            if ( essentialPointsIndexes == null || essentialPointsIndexes.Count <= 2 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_EXTREME_POINTS );
                return null;
            }

            if ( wallEndPoints == null || wallEndPoints.Count <= 2 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_POINTS  );
                return null;
            }

            var list = new List<Vector3>( );

            for ( var i = 0; i < essentialPointsIndexes.Count; i++ )
                list.Add( wallEndPoints.ElementAt( essentialPointsIndexes[ i ] ) );

            var retval = list.ToArray( );

            essentialPoints = retval;

            return retval;
        }


        public  void SelectAreaItems( )
        {
            Selection.objects = Walls.Select( i => i.gameObject ).ToArray( );
        }

        public  void PingAreaItems( )
        {
            var gameObjects = Walls.Select( i => i.gameObject ).ToArray( );
            for ( var i = 0; i < gameObjects.Length; i++ )
            {
                EditorGUIUtility.PingObject( gameObjects[ i ].gameObject );
            }
        }

        public  bool CheckContinuity( )
        {
            var continuity = true;
            for ( var i = 0; i < Walls.Count; i++ )
                if ( Walls[ i ] == null )
                    continuity = false;
                else if ( Walls[ i ].frontConnections.Count == 0 )
                    continuity = false;
                else if ( Walls[ i ].rearConnections.Count == 0 )
                    continuity = false;
            return continuity;
        }


        private List<Triangle> RecalcTriangles( )
        {
            if ( essentialPoints == null || essentialPoints.Length < 3 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_EXTREME_POINTS  );
                return null;
            }

            var retval = new List<Triangle>( );
            int n1, n2;

            for ( var n = 0; n < essentialPoints.Length - 2; n++ )
            {
                n1 = Collections_Helper.GetNextLoopIndex( n, essentialPoints.Length );
                n2 = Collections_Helper.GetNextLoopIndex( n1, essentialPoints.Length );

                var t = new Triangle( essentialPoints[ 0 ],
                                      essentialPoints[ n1 ],
                                      essentialPoints[ n2 ] );

                if ( t.area > 0 )
                    retval.Add( t );
            }

            for ( var i = 0; i < retval.Count; i++ )
                for ( var j = 0; j < retval.Count; j++ )
                {
                    if ( i == j )
                        continue;

                    var isIntersect = false;

                    var intersectedPoints = retval[ i ].essentialPoints.ToList( )
                                                       .IntersectVectors( retval[ j ].essentialPoints.ToList( ) )
                                                       .ToArray( );

                    if ( intersectedPoints.Length > 0 && intersectedPoints.Length < 3 )
                    {
                        var remainedPointsI = retval[ i ].essentialPoints
                                                         .Where( e => !intersectedPoints.Any( d => d.ApxEquals( e ) ) )
                                                         .ToArray( );

                        var remainedPointsJ = retval[ j ].essentialPoints
                                                         .Where( e => !intersectedPoints.Any( d => d.ApxEquals( e ) ) )
                                                         .ToArray( );

                        for ( var r = 0; r < remainedPointsJ.Length; r++ )
                        {
                            var isInsinde = retval[ i ].IsPointInside( remainedPointsJ[ r ] );
                            isIntersect |= isInsinde.isInside && isInsinde.isEdge == false;
                        }

                        if ( isIntersect )
                        {
                            retval[ i ].AddIntersectedTriangle( retval[ j ] );
                            retval[ j ].AddIntersectedTriangle( retval[ i ] );
                        }
                        else if ( intersectedPoints.Length == 2 && remainedPointsI.Length == 1 &&
                                  remainedPointsJ.Length == 1 )
                        {
                             
                            var a = intersectedPoints[ 0 ];
                            var b = intersectedPoints[ 1 ];

                            var c = remainedPointsI[ 0 ];
                            var d = remainedPointsJ[ 0 ];
                            var ab = b - a;
                            var ac = c - a;
                            var ad = d - a;

                            var angleС = Vector3.SignedAngle( ab, ac, Vector3.up ).RoundDecimals( );
                            var angleВ = Vector3.SignedAngle( ab, ad, Vector3.up ).RoundDecimals( );

                            var sideC =
                                ( ab.magnitude * ac.magnitude * Mathf.Sin( angleС * Mathf.Deg2Rad ) ).RoundDecimals( );
                            var sideD =
                                ( ab.magnitude * ad.magnitude * Mathf.Sin( angleВ * Mathf.Deg2Rad ) ).RoundDecimals( );

                            if ( ( sideC >= 0 && sideD >= 0 )
                              || ( sideC <= 0 && sideD <= 0 ) )
                            {
                                retval[ i ].AddIntersectedTriangle( retval[ j ] );
                                retval[ j ].AddIntersectedTriangle( retval[ i ] );
                            }
                        }
                    }
                }

            triangles = retval;

            return retval;
        }

        private bool IsPointOnPathPoints( Vector3 point )
        {
            if ( wallEndPoints == null || wallEndPoints.Count == 0 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_POINTS  );
                return false;
            }

            for ( var i = 0; i < wallEndPoints.Count; i++ )
                if ( wallEndPoints[ i ].ApxEquals( point ) )
                    return true;

            return false;
        }

        public  bool IsPointInsideArea( Vector3 pointer )
        {
            if ( triangles == null || triangles.Count == 0 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_TRIANGLES  );
                return false;
            }

            if ( !pointer.y.ApxEquals( _height ) ) return false;

             
            if ( IsPointOnPathPoints( pointer ) ) return true;

            var edgeNumber = 0;
            var insideNumber = 0;
            var insideTriangles = new List<Triangle>( );
            var trisOnEdge = new List<Triangle>( );

             
            for ( var i = 0; i < triangles.Count; i++ )
            {
                var checker = triangles[ i ].IsPointInside( pointer );

                if ( checker.isInside )
                {
                     
                    insideNumber++;

                     
                    if ( checker.isEdge )
                    {
                        edgeNumber++;
                        trisOnEdge.Add( triangles[ i ] );
                    }
                }
            }

             
             
             
             
             
            if ( insideNumber > 1 && edgeNumber > 1 )
            {
                var trisIntersection = 0;

                trisOnEdge.Distinct( );

                for ( var j = 0; j < trisOnEdge.Count; j++ )
                    for ( var k = 0; k < trisOnEdge.Count; k++ )
                    {
                        if ( j == k )
                            continue;

                        if ( trisOnEdge[ j ].IsTriangleInside( trisOnEdge[ k ] ) ) trisIntersection++;
                    }

                if ( trisIntersection == 0 )
                    insideNumber--;
            }

            var isInside = insideNumber % 2 != 0;

            if ( !isInside )
                if ( edgeNumber == 1 )
                    isInside = true;

            return isInside;
        }

        public  bool IsPointInsideAreaNotOnEdge( Vector3 pointer )
        {
            if ( triangles == null || triangles.Count == 0 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_TRIANGLES  );
                return false;
            }

            if ( pointer.y != _height )
            {
                return false;
            }

             
            if ( IsPointOnPathPoints( pointer ) ) return false;

            var edgeNumber = 0;
            var insideNumber = 0;
            var trisOnEdge = new List<Triangle>( );

             
            for ( var i = 0; i < triangles.Count; i++ )
            {
                var checker = triangles[ i ].IsPointInside( pointer );

                if ( checker.isInside )
                {
                     
                    insideNumber++;

                     
                    if ( checker.isEdge )
                    {
                        edgeNumber++;
                        trisOnEdge.Add( triangles[ i ] );
                    }
                }
            }

             
             
             
             
             
            if ( insideNumber > 1 && edgeNumber > 1 )
            {
                var trisIntersection = 0;

                trisOnEdge.Distinct( );

                for ( var j = 0; j < trisOnEdge.Count; j++ )
                    for ( var k = 0; k < trisOnEdge.Count; k++ )
                    {
                        if ( j == k )
                            continue;

                        if ( trisOnEdge[ j ].IsTriangleInside( trisOnEdge[ k ] ) )
                            trisIntersection++;
                    }

                if ( trisIntersection == 0 )
                    insideNumber--;
            }

            var isInside = insideNumber % 2 != 0;

            if ( !isInside )
                if ( edgeNumber == 1 )
                    isInside = true;
            return isInside;
        }
        
        public  (bool inside, bool edge) IsPointInsideAreaWithEdge( Vector3 pointer )
        {
            if ( triangles == null || triangles.Count == 0 )
            {
                Debug.LogError( Texts.Building.Geometry.AREA_HAS_NO_PATH_TRIANGLES );
                return ( false, false );
            }

            if ( !pointer.y.ApxEquals( _height ) ) return ( false, false );

             
            if ( IsPointOnPathPoints( pointer ) ) return ( true, true );

            var edgeNumber = 0;
            var insideNumber = 0;
            var trisOnEdge = new List<Triangle>( );

             
            for ( var i = 0; i < triangles.Count; i++ )
            {
                var checker = triangles[ i ].IsPointInside( pointer );

                if ( checker.isInside )
                {
                     
                    insideNumber++;
                     
                    if ( checker.isEdge )
                    {
                        edgeNumber++;
                        trisOnEdge.Add( triangles[ i ] );
                    }
                }
            }

             
             
             
             
             
            if ( insideNumber > 1 && edgeNumber > 1 )
            {
                var trisIntersection = 0;

                trisOnEdge.Distinct( );

                for ( var j = 0; j < trisOnEdge.Count; j++ )
                    for ( var k = 0; k < trisOnEdge.Count; k++ )
                    {
                        if ( j == k )
                            continue;

                        if ( trisOnEdge[ j ].IsTriangleInside( trisOnEdge[ k ] ) )
                            trisIntersection++;
                    }

                if ( trisIntersection == 0 )
                    insideNumber--;
            }

            var isInside = insideNumber % 2 != 0;

            if ( !isInside )
                if ( edgeNumber == 1 )
                    isInside = true;
            return ( isInside, edgeNumber == 1 );
        }


        public  Vector2 RecalcBounds( )
        {
            var retval = default( Vector2 );

            var xMax = float.MinValue;
            var yMax = float.MinValue;

            if ( essentialPoints != null && essentialPoints.Length > 2 )
            {
                for ( var i = 0; i < essentialPoints.Length; i++ )
                    for ( var j = 0; j < essentialPoints.Length; j++ )
                    {
                        if ( i == j )
                            continue;
                        var diff = essentialPoints[ j ] - essentialPoints[ i ];
                        var xDiff = Mathf.Abs( diff.x );
                        var yDiff = Mathf.Abs( diff.z );
                        if ( xDiff > xMax )
                            xMax = xDiff;
                        if ( yDiff > yMax )
                            yMax = yDiff;
                    }

                retval = new Vector2( xMax, yMax );
            }

            areaBounds = retval;
            return retval;
        }

        internal void TurnWalls( MBSConstruction mbsConstruction, bool turnInside )
        {
            if ( Walls == null || Walls.Count == 0 )
                return;
            
            
            for ( var i = 0; i < Walls.Count; i++ )
                Walls[ i ].TurnWallInsideArea( this, turnInside );

            for ( var i = 0; i < Walls.Count; i++ )
            {
                mbsConstruction.RemoveWallAndDisconnect( Walls[ i ] );
                Walls[ i ].ResetSideModifications( );
                Walls[ i ].ClearConnections( );
            }

            for ( var i = 0; i < Walls.Count; i++ )
                mbsConstruction.AddWallAndConnect( Walls[ i ] );

            for ( var i = 0; i < Walls.Count; i++ )
                if ( i == 0 || i == Walls.Count - 1 || i % 2 != 0 )
                    WallConnectionController.RecalculateConnectionNodes( Walls[ i ] );

            mbsConstruction.UpdateEndPoints( );
        }


        public  bool Equals( RoomArea compareObject )
        {
            if ( !area.ApxEquals( compareObject.area ) )
                return false;

            if ( Walls.Count != compareObject.Walls.Count )
            {
                return false;
            }

            var intersectPoints = 0;

            var list1 = RecalculateEssentialPoints( );
            var list2 = compareObject.RecalculateEssentialPoints( );

            if ( list1.Length != list2.Length )
                return false;

            for ( var i = 0; i < list1.Length; i++ )
            {
                var item = list1[ i ];

                for ( var j = 0; j < list2.Length; j++ )
                    if ( item.ApxEquals( list2[ j ] ) )
                    {
                        intersectPoints++;
                        break;
                    }
            }


            if ( intersectPoints == list1.Length && intersectPoints == list2.Length )
                return true;

            return false;
        }
    }
}
#endif