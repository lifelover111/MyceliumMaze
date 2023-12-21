#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MBS.Utilities.Extensions
{
    public  static class MathExtensions
    {
        public  static bool ApxEquals( this float lhs, float rhs )
        {
            return Mathf.Abs( lhs - rhs ) < 0.0001f;
        }

        public  static bool ApxEquals( this float lhs, float rhs, float treshold = 0 )
        {
            if ( treshold == 0 )
                treshold = 0.00010f;
            return lhs - rhs < treshold;
        }

        public  static bool ApxEquals( this Vector3 lhs, Vector3 rhs )
        {
            return ( lhs - rhs ).sqrMagnitude < 0.0005f;
        }


        public  static float RoundDecimals( this float var, int decimalPlaces = 3 )
        {
            return (float)Math.Round( var, decimalPlaces );
        }

        public  static Vector3 RScale( this Vector3 v, float x, float y, float z )
        {
            v.Scale( new Vector3( x, y, z ) );
            return v;
        }

        public  static List<Vector3> DistinctVectors( this List<Vector3> list )
        {
            var temp = new List<Vector3>( list );

            for ( var i = 0; i < list.Count; i++ )
            {
                var currentVector = list[ i ];

                for ( var j = i; j < list.Count; j++ )
                {
                    if ( i == j )
                        continue;

                    var checkVector = list[ j ];

                    if ( currentVector.ApxEquals( checkVector ) ) temp.Remove( checkVector );
                }
            }

            list = temp;

            return list;
        }

        public  static List<Vector3> IntersectVectors( this List<Vector3> list, List<Vector3> checkList )
        {
            var intersectedVectors = new List<Vector3>( list.Count );

            for ( var i = 0; i < list.Count; i++ )
            {
                var currentVector = list[ i ];

                for ( var j = i; j < checkList.Count; j++ )
                {
                    var checkVector = list[ j ];

                    if ( currentVector.ApxEquals( checkVector ) ) intersectedVectors.Add( checkVector );
                }
            }

            return intersectedVectors.DistinctVectors( );
        }


        public  static Vector3 Abs( this Vector3 vector )
        {
            return new Vector3( Mathf.Abs( vector.x ),
                                Mathf.Abs( vector.y ),
                                Mathf.Abs( vector.z ) );
        }

        public  static Vector3 MultiplyByVector3_XXYYZZ( this Vector3 v, Vector3 multVector )
        {
            return new Vector3( v.x * multVector.x,
                                v.y * multVector.y,
                                v.z * multVector.z );
        }
    }
}

#endif