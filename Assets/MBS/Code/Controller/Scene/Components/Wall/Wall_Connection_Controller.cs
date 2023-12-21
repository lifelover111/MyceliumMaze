#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Scene.Mono;
using MBS.Utilities.Extensions;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  static class WallConnectionController
    {
        public  static void RecalculateConnectionNodes( MBSWallModule mbsWallModule, bool doUpdateMesh = true )
        {
            List<MBSWallModule> frontSideConnectionNode = new List<MBSWallModule>( );
            List<MBSWallModule> rearSideConnectionNode = new List<MBSWallModule>( );

            var frontList = mbsWallModule.frontConnections;
            var rearList = mbsWallModule.rearConnections;

            if ( mbsWallModule.connectedToFront != null )
            {
                frontSideConnectionNode.Add( mbsWallModule );
                frontSideConnectionNode.AddRange( frontList );
                RecalculateConnectionsNode( mbsWallModule.FrontEndPointWorldSpace,
                                            mbsWallModule.frontEndPointConstructorSpace,
                                            frontSideConnectionNode,
                                            mbsWallModule.connectedToFront, doUpdateMesh );
            }
            else
            {
                var frontLocked = CheckLockedConnections( frontList, mbsWallModule );

                if ( frontLocked.first != null && frontLocked.second != null )
                {
                    RecalculateConnectionNodes( frontLocked.first );
                }
                else
                {
                    frontSideConnectionNode.Add( mbsWallModule );
                    frontSideConnectionNode.AddRange( frontList );
                    RecalculateConnectionsNode( mbsWallModule.FrontEndPointWorldSpace,
                                                mbsWallModule.frontEndPointConstructorSpace,
                                                frontSideConnectionNode,
                                                mbsWallModule.connectedToFront, doUpdateMesh );
                }
            }


            if ( mbsWallModule.connectedToRear != null )
            {
                rearSideConnectionNode.Add( mbsWallModule );
                rearSideConnectionNode.AddRange( rearList );
                RecalculateConnectionsNode( mbsWallModule.RearEndPointWorldSpace,
                                            mbsWallModule.rearEndPointConstructorSpace,
                                            rearSideConnectionNode,
                                            mbsWallModule.connectedToRear, doUpdateMesh );
            }
            else
            {
                var rearLocked = CheckLockedConnections( rearList, mbsWallModule );

                if ( rearLocked.first != null && rearLocked.second != null )
                {
                    RecalculateConnectionNodes( rearLocked.first );
                }
                else
                {
                    rearSideConnectionNode.Add( mbsWallModule );
                    rearSideConnectionNode.AddRange( rearList );
                    RecalculateConnectionsNode( mbsWallModule.RearEndPointWorldSpace,
                                                mbsWallModule.rearEndPointConstructorSpace,
                                                rearSideConnectionNode,
                                                mbsWallModule.connectedToRear, doUpdateMesh );
                }
            }
        }

        private static (MBSWallModule first, MBSWallModule second) CheckLockedConnections(
            List<MBSWallModule> list, MBSWallModule mbsWallModule )
        {
             
             

            for ( var i = 0; i < list.Count; i++ )
            {
                var item = list[ i ];
                
                if ( item.frontConnections.Contains( mbsWallModule ) )
                {
                    if ( item.connectedToFront != null )
                        if ( item.connectedToFront.connectedToFront == item ||
                             item.connectedToFront.connectedToRear == item )
                            return ( item, item.connectedToFront );
                }
                else if ( item.rearConnections.Contains( mbsWallModule ) )
                {
                    if ( item.connectedToRear != null )
                        if ( item.connectedToRear.connectedToFront == item ||
                             item.connectedToRear.connectedToRear == item )
                            return ( item, item.connectedToFront );
                }
            }

            return ( null, null );
        }

        private static void RecalculateConnectionsNode( Vector3 connectionPointWorld, Vector3 connectionPointConstr,
                                                        List<MBSWallModule> connectedWalls,
                                                        MBSWallModule lockedMbsWallModule, bool doUpdateMesh )
        {
             
             
             
             
            float startSide = 0;

             
            var restWallsBelongToOneSide = false;


            var firstWallIndex = -1;
            MBSWallModule firstMbsWallModule = null;

            var secondWallIndex = -1;
            MBSWallModule secondMbsWallModule = null;

            var maxAngle = float.MinValue;

             
             
             
             
            if ( lockedMbsWallModule != null )
            {
                firstWallIndex = 0;
                firstMbsWallModule = connectedWalls.ElementAt( 0 );

                secondWallIndex = connectedWalls.IndexOf( lockedMbsWallModule );
                secondMbsWallModule = lockedMbsWallModule;
                restWallsBelongToOneSide = true;
            }
            else
                 
                 
            {
                for ( var i = 0; i < connectedWalls.Count; i++ )
                {
                    var nextIndex = ( i + 1 ) % connectedWalls.Count;

                    if ( nextIndex == i || connectedWalls.Count < nextIndex )
                        continue;

                    var currentWall = connectedWalls[ i ];
                    var nextWall = connectedWalls[ nextIndex ];

                    var crossProduct = GetCrossProductRegardToLine( connectionPointWorld,
                                                                    currentWall.transform.position,
                                                                    nextWall.transform.position ).RoundDecimals( );
                    startSide = crossProduct;

                     
                    restWallsBelongToOneSide = true;
                    for ( var j = 0; j < connectedWalls.Count; j++ )
                    {
                        if ( i == j || i == nextIndex )
                            continue;

                        crossProduct = GetCrossProductRegardToLine( connectionPointWorld,
                                                                    currentWall.transform.position,
                                                                    connectedWalls[ j ].transform.position )
                            .RoundDecimals( );

                        restWallsBelongToOneSide &= ( crossProduct >= 0 && startSide >= 0 ) ||
                                                    ( crossProduct <= 0 && startSide <= 0 );
                    }

                     
                    if ( restWallsBelongToOneSide )
                    {
                        firstWallIndex = i;
                        firstMbsWallModule = connectedWalls[ firstWallIndex ];
                        break;
                    }
                }
            }

             
            if ( restWallsBelongToOneSide )
            {
                 
                 
                if ( secondMbsWallModule == null )
                     
                    for ( var i = 0; i < connectedWalls.Count; i++ )
                    {
                        if ( firstWallIndex == i || connectedWalls[ i ] == firstMbsWallModule )
                            continue;

                        var sideA = connectedWalls[ firstWallIndex ].transform.position - connectionPointWorld;
                        var sideB = connectedWalls[ i ].transform.position - connectionPointWorld;
                        var currentAngle = Vector3.Angle( sideA, sideB ).RoundDecimals( );

                         
                         
                        if ( currentAngle > maxAngle )
                        {
                            maxAngle = currentAngle;
                            secondMbsWallModule = connectedWalls[ i ];
                            secondWallIndex = i;
                        }
                    }

                 
                 
                if ( secondMbsWallModule != null )
                {
                    if ( maxAngle != 180 && maxAngle != 0 )
                    {
                        for ( var i = 0; i < connectedWalls.Count; i++ )
                            if ( i != secondWallIndex && i != firstWallIndex )
                            {
                                if ( connectedWalls[ i ].IsThereLockedConnectionAt( connectionPointConstr ) )
                                    continue;

                                connectedWalls[ i ].ResetSideModification( connectionPointConstr );

                                if ( doUpdateMesh )
                                    UpdateMesh( connectedWalls[ i ] );
                            }

                        UpdateSides( firstMbsWallModule, secondMbsWallModule, connectionPointConstr, maxAngle );
                        UpdateSides( secondMbsWallModule, firstMbsWallModule, connectionPointConstr, maxAngle );

                        if ( doUpdateMesh )
                        {
                            UpdateMesh( firstMbsWallModule );
                            UpdateMesh( secondMbsWallModule );
                        }
                    }
                    else
                    {
                         
                        for ( var i = 0; i < connectedWalls.Count; i++ )
                        {
                            connectedWalls[ i ].ResetSideModification( connectionPointConstr );

                            if ( doUpdateMesh )
                                UpdateMesh( connectedWalls[ i ] );
                        }
                    }
                }
            }
            else
            {
                 
                for ( var i = 0; i < connectedWalls.Count; i++ )
                {
                    connectedWalls[ i ].ResetSideModification( connectionPointConstr );

                    if ( doUpdateMesh )
                        UpdateMesh( connectedWalls[ i ] );
                }
            }
        }


        private static void UpdateSides( MBSWallModule currentMbsWallModule, MBSWallModule connectedMbsWallModule,
                                         Vector3 connectionPointConstr,
                                         float angle )
        {
             
             
            var modification = currentMbsWallModule.GetModificationAt( connectionPointConstr );

            if ( modification == null )
                return;

            var position = currentMbsWallModule.transform.position;
            var direction = connectedMbsWallModule.transform.position - position;
            direction /= 2;


            var midPoint = position + direction;


            var dist = ( position - currentMbsWallModule.transform.forward / 2 - midPoint ).sqrMagnitude;
            var dist1 = ( position + currentMbsWallModule.transform.forward / 2 - midPoint ).sqrMagnitude;

            if ( dist1 < dist )
            {
                modification.positiveSide = -1;
                modification.negativeSide = +1;
            }
            else if ( dist1 > dist )
            {
                modification.positiveSide = +1;
                modification.negativeSide = -1;
            }

            modification.angle = angle;


            var pointO = currentMbsWallModule.MbsConstruction.transform.TransformPoint( connectionPointConstr );
            var vectorOa = currentMbsWallModule.transform.position - pointO;
            var vectorOb = connectedMbsWallModule.transform.position - pointO;
            var bPointWorldEqualized = pointO + vectorOb.normalized * vectorOa.magnitude;
            var aPointWorld = pointO + vectorOa;
            var vectorBa = bPointWorldEqualized - aPointWorld;

            modification.abVector = vectorBa.normalized;
        }

        public  static void UpdateMesh( MBSWallModule mbsWallModule )
        {
            for ( var i = 0; i < mbsWallModule.meshModifiers.Count; i++ )
            {
                var modifier = mbsWallModule.meshModifiers[ i ];

                if ( !modifier.doModify )
                {
                    modifier.SetupMesh( );
                }
                else
                {
                    var modifiedMesh = Wall_MeshModifier.ModifyMesh( mbsWallModule, modifier );
                    modifier.SetupMesh( modifiedMesh );
                }

                mbsWallModule.transform.localScale = Vector3.one;
            }
        }

        private static float GetCrossProductRegardToLine( Vector3 startLine, Vector3 endLine, Vector3 point )
        {
            var retval = Vector3.Cross( startLine - point, endLine - point ).y;
            return retval;
        }
    }
}
#endif