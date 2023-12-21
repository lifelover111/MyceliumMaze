#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Builder;
using MBS.Controller.Scene.Mono;
using MBS.MBS.Code.Utilities;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using MBS.View.Input;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public static class BuildingProcedures_Floors
    {
        private struct FloorPlacementData
        {
            public GameObject prefab;
            public FloorTileType tileType;
            public FloorCorner corner;
        }

        private static GameObject _drawingGroupRoot;
        private static List<GameObject> _drawingFloorLines;
        private static List<List<(GameObject g1, GameObject g2)>> _drawingFloorLinesItems;
        private static List<List<(FloorPlacementData d1, FloorPlacementData d2)>> _itemsData;
        private static List<List<Vector3>> _tileLocalPositions;
        private static RoomArea _drawingInArea;

        private static ModularPack _curPack;
        private static FloorGroup _curGroup;
        private static FloorModule _curModule;
        private static Vector3 _startPos;
        private static Vector3 _curModuleSize;

        private static Mesh _squareMesh;
        private static Mesh _triangleMesh;

        public static bool Start( )
        {
            var constr = MBSConstruction.Current;


            _curModule = (FloorModule)BuilderDataController.SelectedModule;
            _curGroup = BuilderDataController.SelectedGroup as FloorGroup;
            _curPack = ModularPack_Manager.Singleton.ModularPacks.ToList( )
                                          .Find( i => i.FloorGroups.FirstOrDefault(
                                                          j => j.Guid == BuilderDataController.SelectedGroup
                                                                   .Guid ) != null );

            if ( _curModule == null || _curModule.Prefabs.FirstOrDefault( ) == null )
            {
                Debug.LogError( Texts.Building.CANNOT_START_DRAWING_MODULE );
                return false;
            }

            var neighborNames = constr.transform.GetComponentsInChildren<Transform>( ).Select( i => i.gameObject.name )
                                      .ToArray( );
            var uniqueName = ObjectNames.GetUniqueName( neighborNames, "Floor Tiles Group" );

            _drawingGroupRoot = new GameObject( uniqueName );
            _drawingGroupRoot.transform.SetParent( constr.transform, false );
            _drawingGroupRoot.transform.localPosition = Vector3.zero;

            VisibilityStateInternals.SetPickingNoUndo( _drawingGroupRoot, false, false );


            _drawingFloorLines = new List<GameObject>( );
            _drawingFloorLinesItems = new List<List<(GameObject g1, GameObject g2)>>( );
            _itemsData = new List<List<(FloorPlacementData d1, FloorPlacementData d2)>>( );
            _tileLocalPositions = new List<List<Vector3>>( );

            _drawingInArea = constr.GetAreaAtPoint( Mouse.FreeConstrPos );

            _curModuleSize = _curModule.GetSize( );

            _startPos = Mouse.SnappedConstrPos;

            _squareMesh = _curModule.GetPrefabCombinedMeshAt( 0 );
            if ( _curModule.TriangularPrefab != null )
                _triangleMesh = _curModule.GetPrefabCombinedMeshAt( 1 );

            BeginDrawFloorLine( 0 );

            return true;
        }

        public static void Draw( )
        {
            var deltaPos = Mouse.SnappedConstrPos - _startPos;
            var xLength = Mathf.Abs( Mathf.RoundToInt( deltaPos.x / MbsGrid.CellSize ) ) + 1;
            var zLength = Mathf.Abs( Mathf.RoundToInt( deltaPos.z / MbsGrid.CellSize ) ) + 1;

            var lineNumber = _drawingFloorLinesItems.Count;
            var itemNumber = _drawingFloorLinesItems[ 0 ].Count;

            if ( xLength > lineNumber || zLength > itemNumber )
                for ( var x = 0; x < xLength; x++ )
                {
                    var localLinePos = _startPos + Vector3.right * x * MbsGrid.CellSize * Mathf.Sign( deltaPos.x );

                    if ( x >= lineNumber )
                        BeginDrawFloorLine( x + 1 );

                    itemNumber = _drawingFloorLinesItems[ x ].Count;

                    for ( var z = itemNumber; z < zLength; z++ )
                    {
                        var localItemOffsetPos =
                            localLinePos + Vector3.forward * MbsGrid.CellSize * z * Mathf.Sign( deltaPos.z );
                        var g = CreateFloorItemInPosition( x, z, localItemOffsetPos );
                        _drawingFloorLinesItems[ x ].Add( g.Item1 );
                        _itemsData[ x ].Add( g.Item2 );
                    }
                }

            lineNumber = _drawingFloorLinesItems.Count;

            if ( xLength < lineNumber )
                for ( var x = lineNumber - 1; x >= xLength; x-- )
                    RemoveFloorLine( x );

            lineNumber = _drawingFloorLinesItems.Count;
            itemNumber = _drawingFloorLinesItems[ 0 ].Count;

            if ( zLength < itemNumber )
                for ( var x = lineNumber - 1; x >= 0; x-- )
                {
                    itemNumber = _drawingFloorLinesItems[ x ].Count;
                    for ( var z = itemNumber - 1; z >= zLength; z-- )
                        RemoveFloorItemFromLines( x, z );
                }
        }

        public static void End( )
        {
            _drawingFloorLines.ForEach( i =>
            {
                if ( i.transform.childCount is 0 )
                    Object.DestroyImmediate( i.gameObject );
            } );
            _drawingFloorLines.RemoveAll( i => i == null );

            if ( _drawingFloorLines.Count > 0 )
            {
                Undo.IncrementCurrentGroup( );
                Undo.SetCurrentGroupName( "MBS: Floor placed" );

                _drawingGroupRoot.RecordCreatedUndo( "Floor group" );

                for ( var i = 0; i < _drawingFloorLines.Count; i++ )
                    if ( _drawingFloorLines[ i ] != null )
                    {
                        _drawingFloorLines[ i ].RecordCreatedUndo( "floor tile line" );
                    }

                for ( var i = 0; i < _drawingFloorLinesItems.Count; i++ )
                {
                    var line = _drawingFloorLinesItems[ i ];
                    var lineData = _itemsData[ i ];

                    if ( line == null )
                        continue;

                    var curConstr = MBSConstruction.Current;

                    for ( var i1 = 0; i1 < line.Count; i1++ )
                    {
                        var gameobjects = line.ElementAtOrDefault( i1 );
                        var data = lineData.ElementAtOrDefault( i1 );

                        var g1 = gameobjects.g1;
                        var f1 = g1?.AddComponent<MBSFloorModule>( );
                        var d1 = data.d1;
                        if ( f1 != null )
                            f1.WhenPlaced( _curPack, _curGroup, _curModule, d1.tileType, d1.corner );

                        var g2 = gameobjects.g2;
                        var f2 = g2?.AddComponent<MBSFloorModule>( );
                        var d2 = data.d2;
                        if ( f2 != null )
                            f2.WhenPlaced( _curPack, _curGroup, _curModule, d2.tileType, d2.corner );


                        if ( g1 != null ) g1.RecordCreatedUndo( );
                        if ( g2 != null ) g2.RecordCreatedUndo( );
                    }
                }
            }

            _drawingGroupRoot = null;
            _drawingFloorLines = null;
            _drawingFloorLinesItems = null;
            _tileLocalPositions = null;
            _drawingInArea = null;
        }

        public static void EndWithoutSaving( )
        {
            DestroyAllFloors( );

            _drawingGroupRoot = null;
            _drawingFloorLines = null;
            _drawingFloorLinesItems = null;
            _tileLocalPositions = null;
            _drawingInArea = null;
        }


        public static void Autofill( )
        {
            if ( Start( ) == false )
                return;

            if ( _drawingInArea == null )
            {
                EndWithoutSaving( );
                return;
            }

            var lineNameIterator = 0;
            var rX = 0;

            var constr = MBSConstruction.Current;

            var longSide = Mathf.CeilToInt( Mathf.Max( _drawingInArea.areaBounds.x, _drawingInArea.areaBounds.y ) );
            _drawingGroupRoot.transform.right = constr.transform.right;

            for ( var x = -longSide; x < longSide; x++ )
            {
                BeginDrawFloorLine( lineNameIterator );

                var itemNameIterator = 0;
                var rZ = 0;

                var localLinePos = Vector3.right * x * MbsGrid.CellSize + _startPos;

                for ( var z = -longSide; z < longSide; z++ )
                {
                    var localTilePos = Vector3.forward * MbsGrid.CellSize * z + localLinePos;
                    _drawingFloorLinesItems[ rX ]
                        .Add( CreateFloorItemInPosition( rX, itemNameIterator, localTilePos ).Item1 );

                    if ( _drawingFloorLinesItems[ rX ][ rZ ].g1 != null )
                        itemNameIterator++;

                    rZ++;
                }

                if ( _drawingFloorLines[ rX ].transform.childCount == 0 )
                {
                    Object.DestroyImmediate( _drawingFloorLines[ rX ] );
                    _drawingFloorLines[ rX ] = null;
                    _drawingFloorLines.RemoveAt( rX );
                    rX--;
                }
                else
                {
                    lineNameIterator++;
                }

                rX++;
            }

            End( );
        }


        private static void BeginDrawFloorLine( int lineNumber )
        {
            var neighborNames = _drawingGroupRoot.transform.GetComponentsInChildren<Transform>( )
                                                 .Select( i => i.gameObject.name ).ToArray( );
            var uniqueName = ObjectNames.GetUniqueName( neighborNames, "Floor Tiles Line" );

            var newLine = new GameObject( uniqueName );
            newLine.transform.SetParent( _drawingGroupRoot.transform, false );
            newLine.transform.localPosition = _startPos;

            VisibilityStateInternals.SetPickingNoUndo( newLine, false, false );
            
            _drawingFloorLines.Add( newLine );
            _tileLocalPositions.Add( new List<Vector3>( ) );
            _drawingFloorLinesItems.Add( new List<(GameObject g1, GameObject g2)>( ) );
            _itemsData.Add( new List<(FloorPlacementData d1, FloorPlacementData d2)>( ) );
        }


        private static void DestroyAllFloors( )
        {
            if ( _drawingGroupRoot != null )
                _drawingGroupRoot.DestroyImmediate( );

            if ( _drawingFloorLines == null || _drawingFloorLines.Count == 0 )
                return;


            for ( var i = 0; i < _drawingFloorLines.Count; i++ )
                if ( _drawingFloorLines[ i ] != null )
                    Object.DestroyImmediate( _drawingFloorLines[ i ] );
        }

        private static void RemoveFloorLine( int lineIndex )
        {
            Object.DestroyImmediate( _drawingFloorLines[ lineIndex ] );
            _drawingFloorLines.RemoveAt( lineIndex );
            _drawingFloorLinesItems.RemoveAt( lineIndex );
            _itemsData.RemoveAt( lineIndex );
        }

        private static void RemoveFloorItemFromLines( int lineIndex, int itemIndex )
        {
            Object.DestroyImmediate( _drawingFloorLinesItems[ lineIndex ][ itemIndex ].g1 );
            Object.DestroyImmediate( _drawingFloorLinesItems[ lineIndex ][ itemIndex ].g2 );
            _drawingFloorLinesItems[ lineIndex ].RemoveAt( itemIndex );
            _itemsData[ lineIndex ].RemoveAt( itemIndex );
        }

        private static ((GameObject g1, GameObject g2), (FloorPlacementData d1, FloorPlacementData d2))
            CreateFloorItemInPosition( int lineIndex, int itemIndex, Vector3 constrPos )
        {
            GameObject g1 = null;
            GameObject g2 = null;
            var d1 = new FloorPlacementData( );
            var d2 = new FloorPlacementData( );

            var squarePrefab = _curModule.SquarePrefab;
            var trianglePrefab = _curModule.TriangularPrefab;

            var constr = MBSConstruction.Current;
            var worldPos = constr.transform.TransformPoint( constrPos );
            var pointerInsideDrawingArea = constr.GetAreaAtPoint( constrPos ) == _drawingInArea;

            if ( !pointerInsideDrawingArea )
            {
                var diagonalCheck = IsDiagonalAtPointInnerArea( constrPos, _curModule.GetSize( ), _drawingInArea );

                if ( diagonalCheck.isThereDiagonal )
                {
                    if ( trianglePrefab != null )
                    {
                        g1 = (GameObject)PrefabUtility.InstantiatePrefab( trianglePrefab );
                        Floor_MeshModifier.ModifyFloorCorner( g1, diagonalCheck.corner );
                        d1.prefab = trianglePrefab;
                        d1.corner = diagonalCheck.corner;
                        d1.tileType = FloorTileType.Triangle;
                    }
                    else
                    {
                        g1 = (GameObject)PrefabUtility.InstantiatePrefab( squarePrefab );
                        d1.prefab = squarePrefab;
                        d1.corner = diagonalCheck.corner;
                        d1.tileType = FloorTileType.Triangle;
                    }
                }
                 
                 
                 
                 
                 
                 
                 
                else
                {
                    return ( ( null, null ), ( d1, d2 ) );
                }
            }
            else
            {
                var diagonalCheck = IsDiagonalAtPointRaycast( constrPos,
                                                              worldPos,
                                                              _curModule.GetSize( ),
                                                              _drawingInArea,
                                                              out var cornerPositions );

                if ( diagonalCheck.isThereDiagonal )
                {
                    if ( trianglePrefab != null )
                    {
                        g1 = (GameObject)PrefabUtility.InstantiatePrefab( trianglePrefab );
                        Floor_MeshModifier.ModifyFloorCorner( g1, diagonalCheck.corner );
                        d1.prefab = trianglePrefab;
                        d1.corner = diagonalCheck.corner;
                        d1.tileType = FloorTileType.Triangle;


                        (bool oppositeInArea, FloorCorner corner) secondCheck = default;

                        if ( diagonalCheck.corner == FloorCorner.TopRight )
                            secondCheck = ( constr.GetAreaAtPoint( cornerPositions[ 2 ] ) == _drawingInArea,
                                            FloorCorner.BotLeft );
                        else if ( diagonalCheck.corner == FloorCorner.BotRight )
                            secondCheck = ( constr.GetAreaAtPoint( cornerPositions[ 3 ] ) == _drawingInArea,
                                            FloorCorner.TopLeft );
                        else if ( diagonalCheck.corner == FloorCorner.BotLeft )
                            secondCheck = ( constr.GetAreaAtPoint( cornerPositions[ 0 ] ) == _drawingInArea,
                                            FloorCorner.TopRight );
                        else if ( diagonalCheck.corner == FloorCorner.TopLeft )
                            secondCheck = ( constr.GetAreaAtPoint( cornerPositions[ 1 ] ) == _drawingInArea,
                                            FloorCorner.BotRight );

                        if ( secondCheck.oppositeInArea )
                        {
                            g2 = (GameObject)PrefabUtility.InstantiatePrefab( trianglePrefab );
                            Floor_MeshModifier.ModifyFloorCorner( g2, secondCheck.corner );

                            d2.prefab = trianglePrefab;
                            d2.corner = secondCheck.corner;
                            d2.tileType = FloorTileType.Triangle;
                        }
                    }
                    else
                    {
                        g1 = (GameObject)PrefabUtility.InstantiatePrefab( squarePrefab );
                        d1.prefab = squarePrefab;
                        d1.corner = diagonalCheck.corner;
                        d1.tileType = FloorTileType.Triangle;
                    }
                }
                else
                {
                    var secondCheck = IsDiagonalAtPointInnerArea( constrPos, _curModule.GetSize( ), _drawingInArea );
                    if ( secondCheck.isThereDiagonal )
                    {
                        if ( trianglePrefab != null )
                        {
                            g1 = (GameObject)PrefabUtility.InstantiatePrefab( trianglePrefab );
                            Floor_MeshModifier.ModifyFloorCorner( g1, secondCheck.corner );
                            d1.prefab = trianglePrefab;
                            d1.corner = secondCheck.corner;
                            d1.tileType = FloorTileType.Triangle;
                        }
                        else
                        {
                            g1 = (GameObject)PrefabUtility.InstantiatePrefab( squarePrefab );
                            d1.prefab = squarePrefab;
                            d1.corner = diagonalCheck.corner;
                            d1.tileType = FloorTileType.Triangle;
                        }
                    }
                    else
                    {
                        g1 = (GameObject)PrefabUtility.InstantiatePrefab( squarePrefab );
                        d1.prefab = squarePrefab;
                        d1.corner = secondCheck.corner;
                        d1.tileType = FloorTileType.Square;
                    }
                }
            }

            g1.name = "FloorTile (" + itemIndex + ")";
            g1.transform.SetParent( _drawingFloorLines[ lineIndex ].transform, false );
            g1.transform.localPosition = _drawingFloorLines[ lineIndex ].transform.InverseTransformPoint( worldPos );

            if ( g2 != null )
            {
                g2.name = "FloorTile (" + itemIndex + "_1)";
                g2.transform.SetParent( _drawingFloorLines[ lineIndex ].transform, false );
                g2.transform.localPosition =
                    _drawingFloorLines[ lineIndex ].transform.InverseTransformPoint( worldPos );
            }

            return ( ( g1, g2 ), ( d1, d2 ) );
        }


        public static void SetGizmoMesh( )
        {
            if ( _triangleMesh == null )
                 
                return;

            Vector3 rightTop, rightBot, leftTop, leftBot;
            rightTop = Mouse.SnappedConstrPos + new Vector3( _curModuleSize.x, 0, _curModuleSize.z ) / 2;
            rightBot = Mouse.SnappedConstrPos + new Vector3( _curModuleSize.x, 0, -_curModuleSize.z ) / 2;
            leftTop = Mouse.SnappedConstrPos + new Vector3( -_curModuleSize.x, 0, _curModuleSize.z ) / 2;
            leftBot = Mouse.SnappedConstrPos + new Vector3( -_curModuleSize.x, 0, -_curModuleSize.z ) / 2;

            var d1 = IsThereWallOnLine( rightTop, leftBot );
            var d2 = IsThereWallOnLine( leftTop, rightBot );

            if ( d1 && !d2 )
            {
                var dist1 = Vector3.Distance( Mouse.SnappedConstrPos, rightBot );
                var dist2 = Vector3.Distance( Mouse.FreeConstrPos, leftTop );

                Gizmos_Controller.Mesh = _triangleMesh;

                if ( dist1 < dist2 )
                {
                    Gizmos_Controller.Rotation = Quaternion.Euler( new Vector3( 0, 90, 0 ) );
                    return;
                }

                if ( dist1 > dist2 )
                {
                    Gizmos_Controller.Rotation = Quaternion.Euler( new Vector3( 0, -90, 0 ) );
                }
            }
            else if ( d2 && !d1 )
            {
                var dist1 = Vector3.Distance( Mouse.SnappedConstrPos, rightTop );
                var dist2 = Vector3.Distance( Mouse.FreeConstrPos, leftBot );

                Gizmos_Controller.Mesh = _triangleMesh;

                if ( dist1 < dist2 )
                {
                    Gizmos_Controller.Rotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
                    return;
                }

                if ( dist1 > dist2 ) Gizmos_Controller.Rotation = Quaternion.Euler( new Vector3( 0, 180, 0 ) );
            }
        }


        public static bool FloorGizmoSupervisor( out FloorTileType outFloorType, out Vector3 outRotation )
        {
            outFloorType = FloorTileType.Square;
            outRotation = default;

            var pos = Mouse.SnappedConstrPos;
            var pos_ns = Mouse.FreeConstrPos;

            var size = BuilderDataController.SelectedGroupSize;
            var rightTop = pos + new Vector3( size.x, 0, size.z ) / 2;
            var rightBot = pos + new Vector3( size.x, 0, -size.z ) / 2;
            var leftTop = pos + new Vector3( -size.x, 0, size.z ) / 2;
            var leftBot = pos + new Vector3( -size.x, 0, -size.z ) / 2;

            var d1 = IsThereWallOnLine( rightTop, leftBot );
            var d2 = IsThereWallOnLine( leftTop, rightBot );


            if ( d1 && !d2 )
            {
                var dist1 = Vector3.Distance( pos_ns, rightBot );
                var dist2 = Vector3.Distance( pos_ns, leftTop );

                outFloorType = FloorTileType.Triangle;

                if ( dist1 < dist2 )
                {
                    outRotation = new Vector3( 0, 90, 0 );
                    return true;
                }

                if ( dist1 > dist2 )
                {
                    outRotation = new Vector3( 0, -90, 0 );
                    return true;
                }
            }
            else if ( d2 && !d1 )
            {
                var dist1 = Vector3.Distance( pos_ns, rightTop );
                var dist2 = Vector3.Distance( pos_ns, leftBot );

                outFloorType = FloorTileType.Triangle;

                if ( dist1 < dist2 )
                {
                    outRotation = new Vector3( 0, 0, 0 );
                    return true;
                }

                if ( dist1 > dist2 )
                {
                    outRotation = new Vector3( 0, 180, 0 );
                    return true;
                }
            }

            return false;
        }

        private static bool IsThereWallOnLine( Vector3 start, Vector3 end )
        {
            var pointsToCheck = 3;

            start = MBSConstruction.Current.transform.TransformPoint( start );
            end = MBSConstruction.Current.transform.TransformPoint( end );

            var line = end - start;

            var sideOffset = line.magnitude / 10;
            var shiftStep = ( line.magnitude + sideOffset * 2 ) / pointsToCheck;

            var rayDistance = 1000;
            var distanceUp = Vector3.up * rayDistance;
            var distanceDown = Vector3.down * rayDistance;


            RaycastHit[ ] hits;
            var hitChecks = new bool[ pointsToCheck ];

            GameObject wall = null;

            for ( var i = 0; i < pointsToCheck; i++ )
            {
                var shift = sideOffset + shiftStep * i;
                var pointAlongLine = Vector3.Lerp( start, end, shift );
                var from = pointAlongLine + distanceUp;

                hits = Physics.RaycastAll( from, Vector3.down, rayDistance + 100 );

                wall = null;
                if ( hits != null && hits.Length > 0 )
                    for ( var j = 0; j < hits.Length; j++ )
                    {
                        if ( !hits[ j ].transform.CompareTag( PredefinedTags.EDITING_WALL ) )
                            continue;

                        if ( hits[ j ].transform.position.y == start.y ) wall = hits[ j ].transform.gameObject;
                    }

                if ( wall == null )
                    hitChecks[ i ] = false;
                else
                    hitChecks[ i ] = true;
            }

            var isAll = hitChecks.All( p => p );
            return isAll;
        }

        private static (bool isThereDiagonal, FloorCorner corner) IsDiagonalAtPointRaycast( Vector3 localPos,
            Vector3 worldPos, Vector3 currentAssetSize, RoomArea area, out Vector3[ ] cornersPositions )
        {
            Vector3 topRight, botRight, topLeft, botLeft;

             
            topRight = localPos + new Vector3( currentAssetSize.x, 0, currentAssetSize.z ) / 2;
            botRight = localPos + new Vector3( currentAssetSize.x, 0, -currentAssetSize.z ) / 2;
            botLeft = localPos + new Vector3( -currentAssetSize.x, 0, -currentAssetSize.z ) / 2;
            topLeft = localPos + new Vector3( -currentAssetSize.x, 0, currentAssetSize.z ) / 2;

            var d1 = IsThereWallOnLine( topRight, botLeft );
            var d2 = IsThereWallOnLine( topLeft, botRight );

             
            topRight = localPos + new Vector3( currentAssetSize.x, 0, currentAssetSize.z ) / 4;
            botRight = localPos + new Vector3( currentAssetSize.x, 0, -currentAssetSize.z ) / 4;
            botLeft = localPos + new Vector3( -currentAssetSize.x, 0, -currentAssetSize.z ) / 4;
            topLeft = localPos + new Vector3( -currentAssetSize.x, 0, currentAssetSize.z ) / 4;
            cornersPositions = new[ ] { topRight, botRight, botLeft, topLeft };

            if ( d1 && !d2 )
            {
                var topLeftInArea = MBSConstruction.Current.GetAreaAtPoint( topLeft ) == area;
                var botRightInArea = MBSConstruction.Current.GetAreaAtPoint( botRight ) == area;

                if ( topLeftInArea )
                    return ( true, FloorCorner.TopLeft );
                if ( botRightInArea ) return ( true, FloorCorner.BotRight );
            }
            else if ( !d1 && d2 )
            {
                var topRightInArea = MBSConstruction.Current.GetAreaAtPoint( topRight ) == area;
                var botLeftInArea = MBSConstruction.Current.GetAreaAtPoint( botLeft ) == area;

                if ( topRightInArea )
                    return ( true, FloorCorner.TopRight );
                if ( botLeftInArea ) return ( true, FloorCorner.BotLeft );
            }

             
            return ( false, FloorCorner.All );
        }

        private static (bool isThereDiagonal, FloorCorner corner) IsDiagonalAtPointInnerArea( Vector3 localPos,
            Vector3 assetSize, RoomArea drawingInArea )
        {
            Vector3 topRight, botRight, topLeft, botLeft;

            topRight = localPos + new Vector3( assetSize.x, 0, assetSize.z ) / 3;
            botRight = localPos + new Vector3( assetSize.x, 0, -assetSize.z ) / 3;
            botLeft = localPos + new Vector3( -assetSize.x, 0, -assetSize.z ) / 3;
            topLeft = localPos + new Vector3( -assetSize.x, 0, assetSize.z ) / 3;

            var neededNumber = 0;

            var aTopRight = MBSConstruction.Current.GetAreaAtPoint( topRight );
            var aBotRight = MBSConstruction.Current.GetAreaAtPoint( botRight );
            var aBotLeft = MBSConstruction.Current.GetAreaAtPoint( botLeft );
            var aTopLeft = MBSConstruction.Current.GetAreaAtPoint( topLeft );

            if ( aTopRight == drawingInArea )
                neededNumber++;
            if ( aBotRight == drawingInArea )
                neededNumber++;
            if ( aBotLeft == drawingInArea )
                neededNumber++;
            if ( aTopLeft == drawingInArea )
                neededNumber++;

            if ( neededNumber == 2 || neededNumber == 4 )
                return ( false, FloorCorner.All );

            if ( aTopRight == drawingInArea && aBotLeft != drawingInArea )
                return ( true, FloorCorner.TopRight );
            if ( aBotRight == drawingInArea && aTopLeft != drawingInArea )
                return ( true, FloorCorner.BotRight );
            if ( aBotLeft == drawingInArea && aTopRight != drawingInArea )
                return ( true, FloorCorner.BotLeft );
            if ( aTopLeft == drawingInArea && aBotRight != drawingInArea ) return ( true, FloorCorner.TopLeft );

            return ( false, FloorCorner.All );
        }

        public static void Clear( )
        {
            if ( _drawingFloorLinesItems != null )
                foreach ( var drawingFloorLinesItem in _drawingFloorLinesItems )
                {
                    if ( drawingFloorLinesItem != null )
                        foreach ( var valueTuple in drawingFloorLinesItem )
                        {
                            if ( valueTuple.g1 != null )
                                Object.DestroyImmediate( valueTuple.g1 );

                            if ( valueTuple.g2 != null )
                                Object.DestroyImmediate( valueTuple.g2 );
                        }
                }

            if ( _drawingFloorLines != null )
                foreach ( var drawingFloorLine in _drawingFloorLines )
                {
                    Object.DestroyImmediate( drawingFloorLine );
                }

            _drawingFloorLines = null;

            if ( _drawingGroupRoot != null )
                Object.DestroyImmediate( _drawingGroupRoot );
            _drawingGroupRoot = null;

            _drawingFloorLinesItems = null;

            _itemsData = null;

            _tileLocalPositions = null;
            _drawingInArea = null;

            _curPack = null;
            _curGroup = null;
            _curModule = null;
            _startPos = default;
            _curModuleSize = default;

            _squareMesh = null;
            _triangleMesh = null;
        }
    }
}

#endif