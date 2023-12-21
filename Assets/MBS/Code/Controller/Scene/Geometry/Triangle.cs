#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using UnityEngine;

namespace MBS.Controller.Scene
{
    [Serializable]
    public  class Triangle : GeometryShape
    {
        [SerializeReference] private List<Triangle> _trianglesInside;

        public  Triangle( Vector3 a, Vector3 b, Vector3 c )
        {
            essentialPoints = new Vector3[ 3 ];
            essentialPoints[ 0 ] = a;
            essentialPoints[ 1 ] = b;
            essentialPoints[ 2 ] = c;
            RecalculateArea( );
        }

        public  float CalcTriangleArea( Vector3 p1, Vector3 p2, Vector3 p3 )
        {
            return Mathf.Abs( p1.x * p2.z - p1.z * p2.x +
                              ( p2.x * p3.z - p2.z * p3.x ) +
                              ( p3.x * p1.z - p3.z * p1.x ) );
        }

        public  (bool isInside, bool isEdge) IsPointInside( Vector3 point )
        {
            if ( essentialPoints == null || essentialPoints.Length != 3 )
            {
                Debug.Log( Texts.Building.Geometry.TRIANGLES_HAS_LESS_3_POINTS );
                return ( false, false );
            }

            float a1, a2, a3;

            a1 = CalcTriangleArea( point, essentialPoints[ 0 ], essentialPoints[ 1 ] );
            a2 = CalcTriangleArea( point, essentialPoints[ 1 ], essentialPoints[ 2 ] );
            a3 = CalcTriangleArea( point, essentialPoints[ 2 ], essentialPoints[ 0 ] );

            var sum = ( a1 + a2 + a3 ) / 2;


            var isInside = sum.ApxEquals( area );
            var isEdge = a1.ApxEquals( 0 ) || a2.ApxEquals( 0 ) || a3.ApxEquals( 0 );

            return ( isInside, isEdge );
        }

        public  bool IsTriangleInside( Triangle triangle )
        {
            if ( _trianglesInside == null || _trianglesInside.Count == 0 ) return false;

            for ( var i = 0; i < _trianglesInside.Count; i++ )
                if ( triangle == _trianglesInside[ i ] )
                    return true;
            return false;
        }

        public  void AddIntersectedTriangle( Triangle triangle )
        {
            if ( triangle == null || triangle.essentialPoints.Length != 3 )
                return;

            if ( !_trianglesInside.Contains( triangle ) )
                _trianglesInside.Add( triangle );
        }
    }
}

#endif