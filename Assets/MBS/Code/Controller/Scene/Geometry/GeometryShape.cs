#if UNITY_EDITOR

using System;
using MBS.Code.Utilities.Helpers;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  enum WindingOrder
    {
        Clockwise,
        CounterClockise,
        Invalid
    }

    [Serializable]
    public  class GeometryShape
    {
        [SerializeField] public  float area;
        [SerializeField] public  Vector3[ ] essentialPoints;
        [SerializeField] public  WindingOrder pointsWindingOrder;

        public  float RecalculateArea( )
        {
            if ( essentialPoints == null || essentialPoints.Length == 0 )
                return 0;

            var tempArea = CalculateArea( essentialPoints );

            area = Mathf.Abs( tempArea );

            pointsWindingOrder = GetWindingOrderByArea( tempArea );

            return area;
        }

         
         
         
         
         
         
        public  static float CalculateArea( Vector3[ ] areaEssentialPoints )
        {
            var area = 0f;

            for ( var i = 0; i < areaEssentialPoints.Length; i++ )
            {
                var va = areaEssentialPoints[ i ];
                var vb = areaEssentialPoints[ Collections_Helper.GetNextLoopIndex( i, areaEssentialPoints.Length ) ];

                var width = vb.x - va.x;
                var height = ( vb.z + va.z ) / 2;

                area += width * height;
            }

            return area;
        }

         
         
         
         
         
         
        public  static WindingOrder GetWindingOrderByArea( float area )
        {
            WindingOrder order;

            if ( area > 0 )
                order = WindingOrder.Clockwise;
            else if ( area < 0 )
                order = WindingOrder.CounterClockise;
            else
                order = WindingOrder.Invalid;

            return order;
        }
    }
}

#endif