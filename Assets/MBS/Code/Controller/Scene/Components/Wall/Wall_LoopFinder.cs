#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Scene.Mono;
using MBS.Utilities.Extensions;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  static class Wall_LoopFinder
    {
        public  static List<RoomArea> FindLoops( MBSWallModule startMbsWallModule )
        {
            var areasFound = new List<RoomArea>( );

            var startDirection = startMbsWallModule.FrontEndPointWorldSpace - startMbsWallModule.transform.position;
            
            CheckWall( startMbsWallModule, startDirection, PropagationSide.Left, new List<MBSWallModule>( ),
                       new List<Vector3>( ), areasFound );
            CheckWall( startMbsWallModule, startDirection, PropagationSide.Right, new List<MBSWallModule>( ),
                       new List<Vector3>( ), areasFound );

            return areasFound;
        }

        private static bool CheckWall( MBSWallModule currentMbsWallModule, Vector3 frontDir, PropagationSide sideDir,
                                       List<MBSWallModule> visitedWalls, List<Vector3> visitedPoints,
                                       List<RoomArea> allAreas )
        {
            

            var propagateData = GetPropagateDirData( currentMbsWallModule, frontDir );
            

            var index = visitedPoints.FindIndex( i => i.ApxEquals( propagateData.ConstrFrontEndPoint ) );
            

            if ( index > -1 )
            {            

                 
                if ( index != 0 )
                    return false;
                

                visitedPoints.Add( propagateData.ConstrBackEndPoint );
                visitedWalls.Add( currentMbsWallModule );
                

                var area = new RoomArea( visitedWalls.GetRange( index, visitedWalls.Count - index ),
                                         visitedPoints.GetRange( index, visitedWalls.Count - index ) );
                

                allAreas.Add( area );
                

                return true;
            }
            

            visitedWalls.Add( currentMbsWallModule );
            visitedPoints.Add( propagateData.ConstrBackEndPoint );
            

            if ( propagateData.FrontConnections.Count == 0 ) 
                return false;
            

            if ( propagateData.FrontConnections.Count == 1 )
            {
                var nextWall = propagateData.FrontConnections[ 0 ];
                var nextDirection = nextWall.transform.position - propagateData.WorldFrontEndPoint;
                return CheckWall( nextWall, nextDirection, sideDir, visitedWalls, visitedPoints, allAreas );
            }
            

            var connectedDesiredSide = new List<KeyValuePair<float, MBSWallModule>>( );
            var connectedOppositeSide = new List<KeyValuePair<float, MBSWallModule>>( );
            

            for ( var i = 0; i < propagateData.FrontConnections.Count; i++ )
            {
                var nextWall = propagateData.FrontConnections[ i ];

                var mainDirPoint = currentMbsWallModule.transform.position;
                var a = propagateData.WorldFrontEndPoint - mainDirPoint;
                var b = nextWall.transform.position - mainDirPoint;

                var angle = Vector3.SignedAngle( a, b, Vector3.up ).RoundDecimals( );

                if ( sideDir == PropagationSide.Left )
                {
                    if ( angle < 0 )
                        connectedDesiredSide.Add( new KeyValuePair<float, MBSWallModule>( angle, nextWall ) );
                    else
                        connectedOppositeSide.Add( new KeyValuePair<float, MBSWallModule>( angle, nextWall ) );
                }
                else if ( sideDir == PropagationSide.Right )
                {
                    if ( angle > 0 )
                        connectedDesiredSide.Add( new KeyValuePair<float, MBSWallModule>( angle, nextWall ) );
                    else
                        connectedOppositeSide.Add( new KeyValuePair<float, MBSWallModule>( angle, nextWall ) );
                }
            }

            connectedDesiredSide.Sort( ( a, b ) =>
            {
                if ( Mathf.Abs( a.Key ) > Mathf.Abs( b.Key ) )
                    return -1;
                return +1;
            } );

            connectedOppositeSide.Sort( ( a, b ) =>
            {
                if ( Mathf.Abs( a.Key ) < Mathf.Abs( b.Key ) )
                    return -1;
                return 1;
            } );

            var listOfConnected = connectedDesiredSide.ToList( );
            listOfConnected.AddRange( connectedOppositeSide );

            for ( var i = 0; i < listOfConnected.Count; i++ )
            {
                var nextWall = listOfConnected[ i ].Value;
                var nextDirection = nextWall.transform.position - propagateData.WorldFrontEndPoint;
                var found = CheckWall( nextWall, nextDirection, sideDir,
                                       new List<MBSWallModule>( visitedWalls ), new List<Vector3>( visitedPoints ),
                                       allAreas );

                if ( found )
                    return true;
            }

            return false;
        }


        private static WallNearestSide GetPropagateDirData( MBSWallModule mbsWallModule, Vector3 worldDirection )
        {
        
            var p = mbsWallModule.transform.InverseTransformPoint( mbsWallModule.transform.position + worldDirection );
            var frontDist = ( p - mbsWallModule.frontEndPointLocalSpace ).sqrMagnitude;
            var rearDist = ( p - mbsWallModule.rearEndPointLocalSpace ).sqrMagnitude;
            
            if ( frontDist < rearDist )
                return new WallNearestSide
                {
                    FrontConnections = mbsWallModule.frontConnections,

                    WorldFrontEndPoint = mbsWallModule.FrontEndPointWorldSpace,
                    ConstrFrontEndPoint = mbsWallModule.frontEndPointConstructorSpace,

                    WorldBackEndPoint = mbsWallModule.RearEndPointWorldSpace,
                    ConstrBackEndPoint = mbsWallModule.rearEndPointConstructorSpace
                };
            if ( rearDist < frontDist )
                return new WallNearestSide
                {
                    FrontConnections = mbsWallModule.rearConnections,

                    WorldFrontEndPoint = mbsWallModule.RearEndPointWorldSpace,
                    ConstrFrontEndPoint = mbsWallModule.rearEndPointConstructorSpace,

                    WorldBackEndPoint = mbsWallModule.FrontEndPointWorldSpace,
                    ConstrBackEndPoint = mbsWallModule.frontEndPointConstructorSpace
                };
            
            return default;
        }

        private enum PropagationSide
        {
            Left,
            Right
        }

        private struct WallNearestSide
        {
            public  Vector3 WorldFrontEndPoint;
            public  Vector3 ConstrFrontEndPoint;

            public  Vector3 WorldBackEndPoint;
            public  Vector3 ConstrBackEndPoint;

            public  List<MBSWallModule> FrontConnections;
        }
    }
}
#endif