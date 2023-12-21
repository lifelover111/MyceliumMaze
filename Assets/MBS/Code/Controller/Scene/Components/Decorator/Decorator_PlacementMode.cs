#if UNITY_EDITOR

using System;
using MBS.Code.Utilities.Helpers;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using MBS.View.Input;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public enum PlacementType
    {
        AllSurfaces,
        UpwardPlus,
        UpwardOnly,
    }

    public enum PlacementFacing
    {
        None,
        XAxis,
        YAxis,
        ZAxis,
        NegXAxis,
        NegYAxis,
        NegZAxis
    }


    public class Decorator_PlacementMode
    {
        public static readonly string[ ] PLACEMENT_TYPE_TEXT = { "All Surfaces", "Upward Plus", "Upward Only" };

        public static readonly string[ ] PLACEMENT_FACING_TEXT =
            { "None", "X Axis", "Y Axis", "Z Axis", "-X Axis", "-Y Axis", "-Z Axis" };


        public static PlacementType placementType;
        public static PlacementFacing placementFacing;
        public static bool DoSnapToGrid;
        public static bool IsCollisionOn;
        public static bool RandomModuleOnPlace;
        public static bool LimitHeightByMBSLevel;
        public static float DefaultRaycastDistance;
        public static int RaycastOffsetCoefficient;


        private static Collider[ ] _boxOverlapNonAlloc;
        private static RaycastHit[ ] _castBoxHitNonAlloc;
        private static RaycastHit[ ] _raycastHitNonAlloc;


        private static Vector3 _pivotOffset;


        private static Vector3 _prevPosition_SidePlacement;
        private static bool _isSideSurface;


        public static bool DoAdjustHeight;
        public static float AdjustHeight;
        public static Space AdjustHeight_SpaceMode;

        public static void Start( )
        {
            _boxOverlapNonAlloc = new Collider[ 1 ];
            _raycastHitNonAlloc = new RaycastHit[ 30 ];
            _castBoxHitNonAlloc = new RaycastHit[ 30 ];

            LimitHeightByMBSLevel = Config.Sgt.LimitPlacementHeightByCurrentLevel;
            RaycastOffsetCoefficient = Config.Sgt.DecoratorRaycastOffsetCoefficient;
        }

        public static void Run( Transform transform, GameObject originalPrefab, Quaternion currentRotation,
                                Bounds localBounds, Bounds worldBounds, Ray worldRay )
        {
            _pivotOffset = transform.TransformPoint( localBounds.center ) - transform.position;

            Vector3 newPrefabPosition;
            RaycastHit rayHit = default;
            switch ( placementType )
            {
                case PlacementType.AllSurfaces:
                {
                    if ( !RaycastNonAlloc( worldRay.origin, worldRay.direction, 200,
                                           _raycastHitNonAlloc, out rayHit ) )
                    {
                        rayHit = GroundHit( worldRay );
                    }
                    else if ( LimitHeightByMBSLevel )
                    {
                        if ( rayHit.point.y < MbsGrid.Position_World.y )
                        {
                            rayHit = GroundHit( worldRay );
                        }
                    }

                    newPrefabPosition = Placement_AllSurfaces( rayHit, transform.position, transform.rotation,
                                                               localBounds, worldBounds, IsCollisionOn );
                    break;
                }
                case PlacementType.UpwardPlus:
                {
                    if ( !RaycastNonAlloc( worldRay.origin, worldRay.direction, 200,
                                           _raycastHitNonAlloc, out rayHit ) )
                    {
                        rayHit = GroundHit( worldRay );
                    }
                    else if ( LimitHeightByMBSLevel )
                    {
                        if ( rayHit.point.y < MbsGrid.Position_World.y )
                        {
                            rayHit = GroundHit( worldRay );
                        }
                    }

                    newPrefabPosition =
                        Placement_UpwardPlus( rayHit, transform, localBounds, worldBounds, IsCollisionOn );
                    break;
                }
                case PlacementType.UpwardOnly:
                {
                    if ( !RaycastNonAllocUpwards( worldRay.origin, worldRay.direction, 200,
                                                  _raycastHitNonAlloc, out rayHit ) )
                    {
                        rayHit = GroundHit( worldRay );
                    }
                    else if ( LimitHeightByMBSLevel )
                    {
                        if ( rayHit.point.y < MbsGrid.Position_World.y )
                        {
                            rayHit = GroundHit( worldRay );
                        }
                    }

                    newPrefabPosition =
                        Placement_UpwardOnly( rayHit, transform.position, currentRotation,
                                              localBounds, worldBounds, IsCollisionOn );
                    break;
                }
                default:
                    Debug.LogError( "MBS. Decorator: PlacementMode. Unexpected behavior." );
                    return;
            }

            Handles.matrix = Matrix4x4.TRS( transform.position + _pivotOffset, transform.rotation, Vector3.one );
            Handles.color = Color.cyan;
            Handles.DrawWireCube( Vector3.zero, localBounds.size * 1.04f );
            Handles.matrix = Matrix4x4.identity;

            PlacementFacing( originalPrefab, transform, currentRotation, placementFacing, rayHit.normal );

            if ( DoSnapToGrid )
            {
                var constrPosition = MBSConstruction.Current.transform.InverseTransformPoint( newPrefabPosition );
                newPrefabPosition =
                    MBSConstruction.Current.transform.TransformPoint( MbsGrid.Snap_ToCenter( constrPosition ) );
            }

            if ( DoAdjustHeight )
            {
                switch ( AdjustHeight_SpaceMode )
                {
                    case Space.World:
                        newPrefabPosition.y = AdjustHeight;
                        break;
                    case Space.Self:
                        newPrefabPosition.y = MbsGrid.Position_World.y + AdjustHeight;
                        break;
                }
            }

            transform.position = newPrefabPosition;
        }

        private static void PlacementFacing( GameObject originalPrefab, Transform transform, Quaternion initialRotation,
                                             PlacementFacing facing,
                                             Vector3 hitNormal )
        {
            switch ( facing )
            {
                case Scene.PlacementFacing.None:
                    transform.rotation = initialRotation;
                    break;
                case Scene.PlacementFacing.XAxis:
                    transform.right = hitNormal;
                    break;
                case Scene.PlacementFacing.YAxis:
                    transform.up = hitNormal;
                    break;
                case Scene.PlacementFacing.ZAxis:
                    transform.forward = hitNormal;
                    break;
                case Scene.PlacementFacing.NegXAxis:
                    transform.right = -hitNormal;
                    break;
                case Scene.PlacementFacing.NegYAxis:
                    transform.up = -hitNormal;
                    break;
                case Scene.PlacementFacing.NegZAxis:
                    transform.forward = -hitNormal;
                    break;
            }
        }

        public static void Clear( )
        {
            _boxOverlapNonAlloc = null;
            _castBoxHitNonAlloc = null;
            _raycastHitNonAlloc = null;
            _pivotOffset = default;
            _isSideSurface = false;
            _prevPosition_SidePlacement = default;
        }


        public static void ChangePlacementType( PlacementType placementType )
        {
            Decorator_PlacementMode.placementType = placementType;
        }

        public static void ChangePlacementType_Next( )
        {
            var curIndex = (int)placementType;
            var enumLength = Enum.GetNames( typeof( PlacementType ) ).Length;

            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            placementType = (PlacementType)nextTypeIndex;
        }


        public static void ChangePlacementFacing( PlacementFacing placementFacing )
        {
            Decorator_PlacementMode.placementFacing = placementFacing;
        }

        public static void ChangePlacementFacing_Next( )
        {
            var curIndex = (int)placementFacing;
            var enumLength = Enum.GetNames( typeof( PlacementFacing ) ).Length;

            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            placementFacing = (PlacementFacing)nextTypeIndex;
        }


        // private static Vector3 GroundPlacement( Ray worldRay, Bounds worldBounds )
        // {
        //     var height = MbsGrid.Position_World.y;
        //     var plane = new Plane( Vector3.up, Vector3.up * height );
        //
        //     if ( plane.Raycast( worldRay, out var distance ) )
        //     {
        //         var floorPoint = worldRay.GetPoint( distance );
        //         var centerFromFloor = floorPoint + Vector3.up * worldBounds.extents.y;
        //
        //         Handles.color = Color.cyan;
        //         Handles.DrawWireCube( centerFromFloor, worldBounds.size );
        //
        //         return centerFromFloor - _pivotOffset;
        //     }
        //
        //     return Vector3.zero;
        // }

        private static RaycastHit GroundHit( Ray worldRay )
        {
            var height = MbsGrid.Position_World.y;
            var plane = new Plane( Vector3.up, Vector3.up * height );

            if ( plane.Raycast( worldRay, out var distance ) )
            {
                _raycastHitNonAlloc[ 0 ] = new RaycastHit( );
                _raycastHitNonAlloc[ 0 ].point = worldRay.GetPoint( distance );
                _raycastHitNonAlloc[ 0 ].normal = Vector3.up;
                _raycastHitNonAlloc[ 0 ].distance = distance;
                return _raycastHitNonAlloc[ 0 ];
            }

            return default;
        }

        #region All_Surfaces

        private static Vector3 Placement_AllSurfaces( RaycastHit rayHit, Vector3 currentObjectPos, Quaternion rotation,
                                                      Bounds localBounds, Bounds worldBounds, bool isCollisionOn )
        {
            if ( isCollisionOn )
            {
                return AllSurfaces_Collision( rayHit, currentObjectPos, rotation, localBounds, worldBounds );
            }

            return AllSurfaces_NoCollision( rayHit, rotation, localBounds, worldBounds );
        }

        private static Vector3 AllSurfaces_Collision( RaycastHit rayHit, Vector3 currentObjectPos, Quaternion rotation,
                                                      Bounds localBounds, Bounds worldBounds )
        {
            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;

            var prevCenterPos = currentObjectPos + _pivotOffset + physicsOffset;
            var newCenterPos = rayHit.point
                             + rayHit.normal * LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation )
                             + physicsOffset;


            var overlapNumber =
                Physics.OverlapBoxNonAlloc( newCenterPos, localBounds.extents, _boxOverlapNonAlloc, rotation );
            if ( overlapNumber == 0 )
            {
                return newCenterPos - _pivotOffset - physicsOffset;
            }

            var velocity = newCenterPos - prevCenterPos;
            var velocityNormal = velocity.normalized;
            var velocityMagnitude = velocity.magnitude;


            if ( BoxCastNonAlloc( prevCenterPos, localBounds.extents, velocityNormal, rotation, velocityMagnitude,
                                  _castBoxHitNonAlloc, out var boxHit ) )
            {
                if ( boxHit.distance > Physics.defaultContactOffset * 3 )
                {
                    newCenterPos = prevCenterPos + velocityNormal * ( boxHit.distance - Physics.defaultContactOffset );
                    velocity = newCenterPos - prevCenterPos;
                }

                var hitNormal = boxHit.normal;
                var j = ( -2 * Vector3.Dot( velocity, hitNormal ) ) / 2;
                var v2 = velocity + j * hitNormal;
                newCenterPos = prevCenterPos + v2;


                overlapNumber = Physics.OverlapBoxNonAlloc( newCenterPos, localBounds.extents, _boxOverlapNonAlloc, rotation );
                if ( overlapNumber == 0 )
                {
                    return newCenterPos - _pivotOffset - physicsOffset + ( velocityNormal * Physics.defaultContactOffset );
                }
            }

            return currentObjectPos - velocityNormal * Physics.defaultContactOffset;
        }

        private static Vector3 AllSurfaces_NoCollision( RaycastHit rayHit, Quaternion rotation, Bounds localBounds,
                                                        Bounds worldBounds )
        {
            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;
            var newCenterPos = rayHit.point
                             + rayHit.normal * LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation )
                             + physicsOffset;
            return newCenterPos - _pivotOffset - physicsOffset;
        }

        #endregion

        #region Upward_Plus

        private static Vector3 Placement_UpwardPlus( RaycastHit rayHit, Transform transform,
                                                     Bounds localBounds, Bounds worldBounds, bool isCollisionOn )
        {
            if ( rayHit.normal.y > 0.707f || rayHit.normal.y < -0.707f )
            {
                if ( isCollisionOn )
                    return UpwardPlus_TopSurface_Collision( rayHit, transform.position, transform.rotation, localBounds,
                                                            worldBounds );
                else
                    return UpwardPlus_TopSurface_NoCollision( rayHit, transform.position, transform.rotation,
                                                              localBounds, worldBounds );
            }
            else
            {
                if ( isCollisionOn )
                    return UpwardPlus_SideSurface_Collision( rayHit, transform.position, transform.rotation,
                                                             localBounds, worldBounds,
                                                             ref _prevPosition_SidePlacement );
                else
                    return UpwardPlus_SideSurface_NoCollision( rayHit, transform.position, transform.rotation,
                                                               localBounds, worldBounds );
            }
        }

        private static Vector3 UpwardPlus_TopSurface_Collision( RaycastHit rayHit, Vector3 currentObjectPos,
                                                                Quaternion rotation,
                                                                Bounds localBounds, Bounds worldBounds )
        {
            _isSideSurface = false;

            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;
            var prevCenterPosition = currentObjectPos + _pivotOffset + physicsOffset;
            var newCenterPosition = rayHit.point + rayHit.normal
                                                 * LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation )
                                                 + physicsOffset;

            var velocity = newCenterPosition - prevCenterPosition;
            var velocityNormal = velocity.normalized;
            var velocityMagnitude = velocity.magnitude;

            if ( velocityMagnitude < Physics.defaultContactOffset )
                return currentObjectPos;

            var overlapNumber =
                Physics.OverlapBoxNonAlloc( newCenterPosition,
                                            localBounds.extents,
                                            _boxOverlapNonAlloc, rotation );
            if ( overlapNumber == 0 )
            {
                return newCenterPosition - _pivotOffset - physicsOffset;
            }


            if ( BoxCastNonAlloc( prevCenterPosition, localBounds.extents, velocityNormal, rotation, velocityMagnitude,
                                  _castBoxHitNonAlloc, out var boxHit ) )
            {
                if ( boxHit.distance > Physics.defaultContactOffset * 2 )
                {
                    newCenterPosition = prevCenterPosition + velocityNormal * boxHit.distance;
                    return newCenterPosition - _pivotOffset - physicsOffset;
                }

                var hitNormal = boxHit.normal;
                var j = ( ( -( 1 + 1 ) * Vector3.Dot( velocity, hitNormal ) ) / 2 );
                var v2 = velocity + j * hitNormal;
                var afterCollisionPos = prevCenterPosition + v2;

                overlapNumber =
                    Physics.OverlapBoxNonAlloc( afterCollisionPos, localBounds.extents, _boxOverlapNonAlloc, rotation );
                if ( overlapNumber == 0 )
                {
                    return afterCollisionPos - _pivotOffset - physicsOffset;
                }
            }


            return currentObjectPos - ( velocityNormal * Physics.defaultContactOffset );
        }

        private static Vector3 UpwardPlus_TopSurface_NoCollision( RaycastHit rayHit, Vector3 currentObjectPos,
                                                                  Quaternion rotation,
                                                                  Bounds localBounds, Bounds worldBounds )
        {
            _isSideSurface = false;

            var wExtents = worldBounds.extents;
            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;
            var newCenterPosition = rayHit.point + rayHit.normal
                                  * LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation ) +
                                    physicsOffset;
            return newCenterPosition - _pivotOffset - physicsOffset;
        }

        private static float LocalExtentNormalDot( Vector3 normal, Vector3 extent, Quaternion rotation )
        {
            var e1 = extent;
            var e2 = new Vector3( -e1.x, e1.y, e1.z );
            var e3 = new Vector3( -e1.x, -e1.y, e1.z );
            var e4 = new Vector3( -e1.x, -e1.y, -e1.z );

            var e5 = new Vector3( e1.x, -e1.y, e1.z );
            var e6 = new Vector3( e1.x, -e1.y, -e1.z );

            var e7 = new Vector3( e1.x, e1.y, -e1.z );
            var e8 = new Vector3( -e1.x, e1.y, -e1.z );

            var dot1 = Vector3.Dot( rotation * e1, normal );
            var dot2 = Vector3.Dot( rotation * e2, normal );
            var dot3 = Vector3.Dot( rotation * e3, normal );
            var dot4 = Vector3.Dot( rotation * e4, normal );
            var dot5 = Vector3.Dot( rotation * e5, normal );
            var dot6 = Vector3.Dot( rotation * e6, normal );
            var dot7 = Vector3.Dot( rotation * e7, normal );
            var dot8 = Vector3.Dot( rotation * e8, normal );


            return Mathf.Max( dot1, dot2, dot2, dot3, dot4, dot5, dot6, dot7, dot8 );
        }

        private static Vector3 UpwardPlus_SideSurface_Collision( RaycastHit rayHit, Vector3 currentObjectPos,
                                                                 Quaternion rotation,
                                                                 Bounds localBounds, Bounds worldBounds,
                                                                 ref Vector3 prevPosition )
        {
            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;
            var prevTopPosition = prevPosition;

            var newTopPosition = rayHit.point + rayHit.normal *
                                 LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation ) +
                                 physicsOffset;

            if ( _isSideSurface == false )
            {
                _isSideSurface = true;
                prevPosition = newTopPosition;
                return currentObjectPos;
            }

            var velocity = ( newTopPosition - prevTopPosition );
            var velocityNormal = velocity.normalized;
            var velocityMagnitude = velocity.magnitude;

            if ( velocityMagnitude < Physics.defaultContactOffset )
                return currentObjectPos;

            var overlapNumber =
                Physics.OverlapBoxNonAlloc( newTopPosition, localBounds.extents, _boxOverlapNonAlloc, rotation );
            if ( overlapNumber > 0 )
            {
                if ( BoxCastNonAlloc( prevTopPosition, localBounds.extents, velocityNormal, rotation,
                                      velocityMagnitude,
                                      _castBoxHitNonAlloc, out var boxHit1 ) )
                {
                    if ( boxHit1.distance > Physics.defaultContactOffset * 2 )
                    {
                        newTopPosition = prevTopPosition +
                                         velocityNormal * ( boxHit1.distance - Physics.defaultContactOffset );
                    }
                    else
                    {
                        var hitNormal = boxHit1.normal;
                        var j = ( ( -( 1 + 1 ) * Vector3.Dot( velocity, hitNormal ) ) / 2 );
                        var v2 = velocity + j * hitNormal;
                        var afterCollisionPos = prevTopPosition + v2;

                        overlapNumber =
                            Physics.OverlapBoxNonAlloc( afterCollisionPos, localBounds.extents, _boxOverlapNonAlloc,
                                                        rotation );
                        if ( overlapNumber == 0 )
                            newTopPosition = afterCollisionPos;
                        else
                            return currentObjectPos;
                    }
                }

                var da = Vector3.Dot( worldBounds.extents, Vector3.up );
                var fa = new Vector3( newTopPosition.x, MbsGrid.Position_World.y + da, newTopPosition.z );
                return fa - _pivotOffset;
            }


            prevPosition = newTopPosition;

            var toGroundDirection = Vector3.down;

            float groundCastDistance;
            if ( LimitHeightByMBSLevel )
                groundCastDistance = Mathf.Abs( rayHit.point.y - MbsGrid.Position_World.y );
            else
                groundCastDistance = DefaultRaycastDistance;

            if ( BoxCastNonAlloc( newTopPosition, localBounds.extents, toGroundDirection, rotation, groundCastDistance,
                                  _castBoxHitNonAlloc, out var groundHit ) )
            {
                var groundPosition = newTopPosition +
                                     toGroundDirection * ( groundHit.distance - Physics.defaultContactOffset );

                var toWallDirection = -rayHit.normal;

                if ( BoxCastNonAlloc( groundPosition, localBounds.extents, toWallDirection, rotation, 1,
                                      _castBoxHitNonAlloc, out var wallHit ) )
                {
                    var retval = groundPosition + ( toWallDirection * wallHit.distance );
                    return retval - _pivotOffset - ( -toGroundDirection * Physics.defaultContactOffset );
                }
                else
                {
                    var retval = groundPosition;
                    return retval - _pivotOffset - ( -toGroundDirection * Physics.defaultContactOffset );
                }
            }

            var d = Vector3.Dot( worldBounds.extents, Vector3.up );
            var failedGroundCastPos = new Vector3( newTopPosition.x, MbsGrid.Position_World.y + d, newTopPosition.z );
            return failedGroundCastPos - _pivotOffset;
        }

        private static Vector3 UpwardPlus_SideSurface_NoCollision( RaycastHit rayHit, Vector3 currentObjectPos,
                                                                   Quaternion rotation,
                                                                   Bounds localBounds, Bounds worldBounds )
        {
            var retval_FinalObjectPosition = currentObjectPos + _pivotOffset;

            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;

            var newTopPosition = rayHit.point + rayHit.normal
                                              * LocalExtentNormalDot( rayHit.normal, localBounds.extents, rotation )
                                              + physicsOffset;

            var toGroundDirection = Vector3.down;

            if ( BoxCastNonAlloc( newTopPosition, localBounds.extents, toGroundDirection, rotation, 100,
                                  _castBoxHitNonAlloc, out var groundHit ) )
            {
                if ( LimitHeightByMBSLevel )
                    if ( groundHit.point.y < MbsGrid.Position_World.y )
                    {
                        var groundCollisionBelowLevelPos = newTopPosition;
                        groundCollisionBelowLevelPos.y = MbsGrid.Position_World.y;
                        return groundCollisionBelowLevelPos;
                    }


                var groundPosition =
                    newTopPosition + toGroundDirection * ( groundHit.distance - Physics.defaultContactOffset );

                var toWallDirection = ( ( groundPosition - rayHit.normal ) - groundPosition ).normalized;

                if ( BoxCastNonAlloc( groundPosition, localBounds.extents, toWallDirection, rotation, 1,
                                      _castBoxHitNonAlloc, out var wallHit ) )
                {
                    retval_FinalObjectPosition = groundPosition + toWallDirection * wallHit.distance;
                    return retval_FinalObjectPosition - _pivotOffset -
                           ( -toGroundDirection * Physics.defaultContactOffset );
                }

                return retval_FinalObjectPosition - _pivotOffset;
            }

            var d = Vector3.Dot( worldBounds.extents, Vector3.up );
            var failedGroundCastPos = new Vector3( newTopPosition.x, MbsGrid.Position_World.y + d, newTopPosition.z );
            return failedGroundCastPos - _pivotOffset;
        }

        #endregion

        #region Upward_Only

        private static Vector3 Placement_UpwardOnly( RaycastHit rayHit, Vector3 currentObjectPos,
                                                     Quaternion rotation,
                                                     Bounds scaledLocalBounds, Bounds worldBounds,
                                                     bool isCollisionEnabled )
        {
            if ( isCollisionEnabled == false )
            {
                var newPos = rayHit.point + ( rayHit.normal * scaledLocalBounds.extents.y );
                return newPos - _pivotOffset;
            }

            var physicsOffset = rayHit.normal * Physics.defaultContactOffset;
            var prevCenterPos = currentObjectPos + _pivotOffset + physicsOffset;
            var newCenterPos =
                rayHit.point + ( rayHit.normal * worldBounds.extents.y ) + physicsOffset;

            var overlapNumber =
                Physics.OverlapBoxNonAlloc( newCenterPos, scaledLocalBounds.extents, _boxOverlapNonAlloc, rotation );
            if ( overlapNumber == 0 )
            {
                return newCenterPos - _pivotOffset - physicsOffset;
            }

            var velocity = ( newCenterPos - prevCenterPos ).RScale( 1, 0, 1 );
            var velocity_normal = velocity.normalized;
            var velocity_magnitude = velocity.magnitude;

            if ( BoxCastNonAlloc( prevCenterPos, scaledLocalBounds.extents, velocity_normal, rotation,
                                  velocity_magnitude + Physics.defaultContactOffset,
                                  _castBoxHitNonAlloc, out var boxHit1 ) )
            {
                if ( boxHit1.distance > Physics.defaultContactOffset * 2 )
                {
                    newCenterPos = prevCenterPos +
                                   velocity_normal * ( boxHit1.distance - Physics.defaultContactOffset );
                    return newCenterPos - _pivotOffset;
                }

                var hitNormal = boxHit1.normal;
                var j = ( ( -( 1 + 1 ) * Vector3.Dot( velocity, hitNormal ) ) / 2 );
                var v2 = velocity + j * hitNormal;
                var afterCollisionPos = prevCenterPos + v2;


                overlapNumber =
                    Physics.OverlapBoxNonAlloc( afterCollisionPos, scaledLocalBounds.extents, _boxOverlapNonAlloc,
                                                rotation );
                if ( overlapNumber == 0 )
                {
                    return afterCollisionPos - _pivotOffset - physicsOffset;
                }

                return currentObjectPos;
            }

            return currentObjectPos;
        }

        #endregion
        


        #region Raycast_Utils

        private static bool RaycastNonAlloc( Vector3 position, Vector3 direction, float distance,
                                             RaycastHit[ ] nonAllocArray, out RaycastHit hit )
        {
            var hitNumber = Physics.RaycastNonAlloc( position, direction, nonAllocArray, distance );

            if ( hitNumber > 0 )
            {
                var minDistance = float.MaxValue;
                var minIndex = int.MaxValue;

                for ( var i = 0; i < hitNumber; i++ )
                {
                    if ( nonAllocArray[ i ].normal == default || nonAllocArray[ i ].point == default )
                        continue;

                    if ( nonAllocArray[ i ].distance < minDistance )
                    {
                        minDistance = nonAllocArray[ i ].distance;
                        minIndex = i;
                    }
                }

                if ( minIndex == int.MaxValue )
                {
                    hit = default;
                    return false;
                }

                hit = nonAllocArray[ minIndex ];
                return true;
            }

            hit = default;
            return false;
        }


        private static bool RaycastNonAllocUpwards( Vector3 position, Vector3 direction, float distance,
                                                    RaycastHit[ ] nonAllocArray, out RaycastHit hit )
        {
            var hitNumber = Physics.RaycastNonAlloc( position, direction, nonAllocArray, distance );

            if ( hitNumber > 0 )
            {
                var minDistance = float.MaxValue;
                var minIndex = int.MaxValue;

                for ( var i = 0; i < hitNumber; i++ )
                {
                    if ( nonAllocArray[ i ].normal == default || nonAllocArray[ i ].point == default )
                        continue;

                    if ( Mathf.Abs( nonAllocArray[ i ].normal.y ) < 0.707f )
                        continue;

                    if ( nonAllocArray[ i ].distance < minDistance )
                    {
                        minDistance = nonAllocArray[ i ].distance;
                        minIndex = i;
                    }
                }

                if ( minIndex == int.MaxValue )
                {
                    hit = default;
                    return false;
                }

                hit = nonAllocArray[ minIndex ];
                return true;
            }

            hit = default;
            return false;
        }

        private static bool BoxCastNonAlloc( Vector3 position, Vector3 extents, Vector3 direction, Quaternion rotation,
                                             float distance,
                                             RaycastHit[ ] nonAllocArray, out RaycastHit outHit )
        {
            var hitNumber = Physics.BoxCastNonAlloc( position, extents, direction,
                                                     nonAllocArray, rotation, distance );
            if ( hitNumber > 0 )
            {
                var minDistance = float.MaxValue;
                var minIndex = int.MaxValue;

                for ( var i = 0; i < hitNumber; i++ )
                {
                    var hit = nonAllocArray[ i ];

                    if ( hit.normal == default || hit.point == default )
                        continue;

                    if ( nonAllocArray[ i ].distance < minDistance )
                    {
                        minDistance = nonAllocArray[ i ].distance;
                        minIndex = i;
                    }
                }

                if ( minIndex == int.MaxValue )
                {
                    outHit = default;
                    return false;
                }

                outHit = nonAllocArray[ minIndex ];
                return true;
            }

            outHit = default;
            return false;
        }

        #endregion
    }
}
#endif