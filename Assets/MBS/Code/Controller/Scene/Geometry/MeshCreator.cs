#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public static class MeshCreator
    {
        public static GameObject CreatePlaneMesh( Vector3[ ] areaPoints, WindingOrder pointsWindingOrder )
        {
            if ( areaPoints == null || areaPoints.Length < 3 )
                return null;

            var center = GetCenterPoint( areaPoints );
            var points = ChangeOffset_SubtractPoint( areaPoints, center );

            if ( pointsWindingOrder == WindingOrder.CounterClockise )
            {
                var temp = points.ToList( );
                temp.Reverse( );

                points = ReverseVectors( points );
            }

            var trianglesList = new List<int>( );
            var indexList = GetIndexList( points.Length );

            var whileIter = 0;

            while ( indexList.Count > 3 )
            {
                if ( whileIter > points.Length * 5 )
                {
                    Debug.LogError( Texts.Building.LoopFinder.INFINITE_LOOP );
                    break;
                }

                for ( var i = 0; i < indexList.Count; i++ )
                {
                    var aIndex = indexList[ i ];
                    var bIndex = indexList.ElementAt( Collections_Helper.GetPrevLoopIndex( i, indexList.Count ) );
                    var cIndex = indexList.ElementAt( Collections_Helper.GetNextLoopIndex( i, indexList.Count ) );

                    var a = points[ aIndex ];
                    var b = points[ bIndex ];
                    var c = points[ cIndex ];

                    var ab = b - a;
                    var ac = c - a;

                    if ( !IsAngleConvex( ab, ac ) )
                        continue;

                    var t = new Triangle( b, a, c );

                    var isEar = true;

                    for ( var j = 0; j < points.Length; j++ )
                    {
                        if ( j == aIndex || j == bIndex || j == cIndex )
                            continue;

                        var p = points[ j ];

                        if ( t.IsPointInside( p ).isInside )
                        {
                            isEar = false;
                            break;
                        }
                    }

                    if ( isEar )
                    {
                        indexList.Remove( aIndex );

                        trianglesList.Add( bIndex );
                        trianglesList.Add( aIndex );
                        trianglesList.Add( cIndex );

                        break;
                    }
                }

                whileIter++;
            }

            trianglesList.Add( indexList.ElementAt( 0 ) );
            trianglesList.Add( indexList.ElementAt( 1 ) );
            trianglesList.Add( indexList.ElementAt( 2 ) );

            var planeMesh = new Mesh( );
            planeMesh.SetVertices( points );
            planeMesh.SetTriangles( trianglesList.ToArray( ), 0 );
            planeMesh.RecalculateBounds( );
            planeMesh.RecalculateNormals( );
            planeMesh.RecalculateTangents( );
            planeMesh.SetUVs( 0, RecalculateUV( planeMesh.vertices, planeMesh.bounds ) );
            planeMesh.RecalculateUVDistributionMetrics( );
            planeMesh.name = "MBSGeneratedAreaMesh";

            var gameObject = new GameObject( );
            gameObject.transform.position = center;

            var mf = gameObject.AddComponent<MeshFilter>( );
            mf.mesh = Object.Instantiate( planeMesh );

            var mr = gameObject.AddComponent<MeshRenderer>( );
            mr.sharedMaterial = Object.Instantiate( InternalDataManager.Singleton.CheckboardMaterial );
            
            return gameObject;
        }

        private static Vector3[ ] ReverseVectors( Vector3[ ] array )
        {
            var temp = new List<Vector3>( array.Length );

            for ( var i = array.Length - 1; i >= 0; i-- ) temp.Add( array[ i ] );

            return temp.ToArray( );
        }

        private static Vector3 GetCenterPoint( Vector3[ ] rawPoints )
        {
            var center = Vector3.zero;

            for ( var i = 0; i < rawPoints.Length; i++ )
                center += rawPoints[ i ];

            center /= rawPoints.Length;

            return center;
        }

        private static Vector3[ ] ChangeOffset_SubtractPoint( Vector3[ ] rawPoints, Vector3 point )
        {
            var recalcPoints = new Vector3[ rawPoints.Length ];

            for ( var i = 0; i < recalcPoints.Length; i++ )
                recalcPoints[ i ] = rawPoints[ i ] - point;

            return recalcPoints;
        }

        private static Vector3[ ] ChangeOffset_AddPoint( Vector3[ ] rawPoints, Vector3 point )
        {
            var recalcPoints = new Vector3[ rawPoints.Length ];

            for ( var i = 0; i < recalcPoints.Length; i++ )
                recalcPoints[ i ] = rawPoints[ i ] + point;

            return recalcPoints;
        }

        private static Vector2[ ] RecalculateUV( Vector3[ ] vertices, Bounds bounds )
        {
            var pivotPoint = new Vector3( bounds.extents.x, 0, -bounds.extents.z );

            var points = ChangeOffset_AddPoint( vertices, pivotPoint );

            var uvs = new Vector2[ points.Length ];

            var xSize = bounds.size.x;
            var zSize = bounds.size.z;

            for ( var i = 0; i < uvs.Length; i++ )
            {
                var uv = new Vector2( points[ i ].x / xSize, points[ i ].z / zSize );
                uvs[ i ] = uv;
            }

            return uvs;
        }


        private static bool IsAngleConvex( Vector3 ab, Vector3 ac )
        {
            var cross = Vector3.Cross( ab, ac ).y;
            return cross < 0;
        }

        public static List<int> GetIndexList( int listLength )
        {
            var retval = new List<int>( );
            for ( var i = 0; i < listLength; i++ )
                retval.Add( i );
            return retval;
        }

        public static float GetAngleBetweenPoints( Vector3 prevPoint, Vector3 midPoint, Vector3 nextPoint )
        {
            var mp = prevPoint - midPoint;
            var mn = nextPoint - midPoint;

            return Vector3.Angle( mn, mp ).RoundDecimals( );
        }
    }
}

#endif