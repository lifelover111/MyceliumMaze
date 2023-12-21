#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Controller.Builder;
using MBS.Controller.Scene;
using MBS.Controller.Scene.Mono;
using MBS.MBS.Code.Utilities;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Model.Scene;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using MBS.View.Builder;
using MBS.View.Input;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBS.Builder.Scene
{
    public enum WallAngleSnapStep
    {
        OneTenth,
        One,
        Ten,
        Fifteen,
        Ninety,
    }

    public static class BuildingProcedures_Walls
    {
        #region AssetVars

        private static WallModule _wallModule;

        private static Vector3 _defaultPrefabSize;
        private static bool _isExtendedPrefabExist;
        private static Vector3 _extendedPrefabSize;

        #endregion

        #region ProcessVars

        private static Vector3 _startPosConstr;
        private static Vector3 _startPosWorld;

        private static GameObject _currentPrefab;
        public static Vector3 currentPrefabSize;

        private static GameObject _currentDrawingGroup;
        private static List<List<GameObject>> _currentDrawingItems;


        private static bool _is45DegreeDrawing;
        private static bool _isPrefabScaled;

        private static float _extendScale = 1;
        private static float _fitScale = 1;

        public static WallAngleSnapStep SnapStep;
        public static float[ ] SnapStepValueArray = { 0.1f, 1.0f, 10.0f, 15.0f, 90.0f };

        public static float levelHeight = 1;
        public static int levelsNumber = 1;

        #endregion


        public static void StartDrawing( )
        {
            SceneData.CaptureStartPosition_Snapped( );

            if ( Start( ) )
            {
                SceneData.BuilderMode = BuilderMode.Drawing;
                InputWalls_Tool.Setup_DrawingInputs( );
                return;
            }

            Debug.LogError( Texts.Building.CANNOT_START_DRAWING_MODULE );
        }

        public static void EndDrawing( )
        {
            End( );

            SceneData.IsMouseRecalcNeeded = true;
            SceneData.BuilderMode = BuilderMode.Idle;
            InputWalls_Tool.Setup_IdleInputs( );
        }

        public static void EndDrawingWithoutSaving( )
        {
            SceneData.IsMouseRecalcNeeded = true;
            EndWithoutSaving( );
            InputWalls_Tool.Setup_IdleInputs( );
            SceneData.BuilderMode = BuilderMode.Idle;
        }

        public static bool Start( )
        {
            Mouse.IsSnappedToEnd = false;

            _wallModule = (WallModule)BuilderDataController.SelectedModule;

            if ( _wallModule?.Prefabs == null || _wallModule.Prefabs.FirstOrDefault( ) == null )
                return false;

            _defaultPrefabSize = GameObjectHelper.GetSize( _wallModule.DefaultPrefab );
            if ( _wallModule.ExtendedPrefab != null )
            {
                _isExtendedPrefabExist = true;
                _extendedPrefabSize = GameObjectHelper.GetSize( _wallModule.ExtendedPrefab );
            }
            else
            {
                _isExtendedPrefabExist = false;
            }


            _currentPrefab = _wallModule.DefaultPrefab;
            currentPrefabSize = _defaultPrefabSize;

            var existedNames = MBSConstruction.Current.transform.GetComponentsInChildren<Transform>( )
                                              .Select( i => i.gameObject.name ).ToArray( );
            var uniqueName = ObjectNames.GetUniqueName( existedNames, "Wall Line" );

            _currentDrawingGroup = new GameObject( uniqueName );
            _currentDrawingGroup.transform.SetParent( MBSConstruction.Current.transform, true );
            _currentDrawingGroup.transform.localPosition = Mouse.SnappedConstrPos;

            VisibilityStateInternals.SetPickingNoUndo( _currentDrawingGroup, false, false );

            _startPosConstr = Mouse.SnappedConstrPos;
            _startPosWorld = Mouse.SnappedWorldPos;

            _currentDrawingItems = new List<List<GameObject>>( );
            levelsNumber = Mathf.Clamp( levelsNumber, 1, 999 );

            _extendScale = 1;
            _fitScale = 1;

            return true;
        }

        public static Vector3 Building( Vector3 currentPosConstr )
        {
            SceneData.DoStretchWalls = true;


            if ( SceneData.DoSnapToGrid )
                return Building_GridSnap( currentPosConstr );
            return Building_NoGridSnap( currentPosConstr );
        }

        private static Vector3 Building_GridSnap( Vector3 currentPosConstr )
        {
            var deltaPos = currentPosConstr - _startPosConstr;
            var deltaPosNormalized = deltaPos.normalized;

            float itemsFit = 1;
            var itemsFitRounded = 1;

            _fitScale = 1;
            _extendScale = 1;


            if ( !Mathf.Abs( deltaPos.x ).ApxEquals( Mathf.Abs( deltaPos.z ) ) )
            {
                if ( _is45DegreeDrawing )
                    if ( _isExtendedPrefabExist )
                        DestroyCurrentWalls( );

                _is45DegreeDrawing = false;

                _currentPrefab = _wallModule.DefaultPrefab;
                currentPrefabSize = _defaultPrefabSize;
            }
            else
            {
                if ( !_isExtendedPrefabExist )
                {
                    _currentPrefab = _wallModule.DefaultPrefab;
                    currentPrefabSize = _defaultPrefabSize;

                    _extendScale = 1 / Mathf.Sin( 45 * Mathf.Deg2Rad );
                    _is45DegreeDrawing = true;
                }
                else
                {
                    if ( _is45DegreeDrawing == false )
                    {
                        DestroyCurrentWalls( );
                        _is45DegreeDrawing = true;
                    }

                    _currentPrefab = _wallModule.ExtendedPrefab;
                    currentPrefabSize = _extendedPrefabSize;
                }
            }

            itemsFit = deltaPos.magnitude / ( currentPrefabSize.x * _extendScale );
            itemsFitRounded = Mathf.RoundToInt( Mathf.Abs( itemsFit ) );


            if ( levelsNumber > _currentDrawingItems.Count )
            {
                var difference = levelsNumber - _currentDrawingItems.Count;
                for ( int i = 0; i < difference; i++ )
                {
                    _currentDrawingItems.Add( new List<GameObject>( ) );
                }
            }
            else if ( levelsNumber < _currentDrawingItems.Count )
            {
                for ( int i = _currentDrawingItems.Count - 1; i >= levelsNumber; i-- )
                {
                    foreach ( var go in _currentDrawingItems[ i ] )
                    {
                        Object.DestroyImmediate( go );
                    }

                    _currentDrawingItems.RemoveAt( i );
                }
            }

            for ( int i = 0; i < _currentDrawingItems.Count; i++ )
            {
                if ( itemsFitRounded > _currentDrawingItems[ i ].Count )
                    for ( var j = _currentDrawingItems[ i ].Count; j < itemsFitRounded; j++ )
                    {
                        var existedNames = _currentDrawingGroup.transform.GetComponentsInChildren<Transform>( )
                                                               .Select( k => k.gameObject.name ).ToArray( );
                        var uniqueName = ObjectNames.GetUniqueName( existedNames, _currentPrefab.name );

                        var instantiatedWall = (GameObject)PrefabUtility.InstantiatePrefab( _currentPrefab );
                        instantiatedWall.transform.SetParent( _currentDrawingGroup.transform, false );
                        instantiatedWall.transform.name = uniqueName;
                        _currentDrawingItems[ i ].Add( instantiatedWall );
                    }
                else if ( itemsFitRounded < _currentDrawingItems[ i ].Count )
                    for ( var j = _currentDrawingItems[ i ].Count - 1; j >= itemsFitRounded; j-- )
                    {
                        Object.DestroyImmediate( _currentDrawingItems[ i ][ j ] );
                        _currentDrawingItems[ i ].RemoveAt( j );
                    }
            }


            var scaledSize = currentPrefabSize.x * _extendScale;
            var scaledSizeHalf = scaledSize / 2;

            for ( int i = 0; i < _currentDrawingItems.Count; i++ )
            {
                for ( var j = 0; j < _currentDrawingItems[ i ].Count; j++ )
                {
                    _currentDrawingItems[ i ][ j ].transform.localPosition =
                        Vector3.right * ( scaledSize * j + scaledSizeHalf ) + Vector3.up * levelHeight * i;

                    var localScale = _currentPrefab.transform.localScale;
                    _currentDrawingItems[ i ][ j ].transform.localScale =
                        new Vector3(
                            localScale.x * _extendScale,
                            localScale.y,
                            localScale.z );
                }
            }

            if ( deltaPosNormalized == Vector3.zero )
            {
                _currentDrawingGroup.transform.localRotation = Quaternion.identity;
            }
            else
            {
                _currentDrawingGroup.transform.localRotation =
                    Quaternion.identity *
                    Quaternion.LookRotation( Quaternion.AngleAxis( -90, Vector3.up ) * deltaPosNormalized, Vector3.up );
            }

            return _startPosConstr + deltaPosNormalized * scaledSize * _currentDrawingItems[ 0 ].Count;
        }

        private static Vector3 Building_NoGridSnap( Vector3 currentPosConstr )
        {
            var deltaPos = currentPosConstr - _startPosConstr;
            var deltaPosNormalized = deltaPos.normalized;

            float itemsFit = 1;
            var itemsFitRounded = 1;

            _extendScale = 1;
            _fitScale = 1;

            _is45DegreeDrawing = false;

            if ( SceneData.DoStretchWalls && Mouse.IsSnappedToEnd )
            {
                if ( _currentPrefab == _wallModule.DefaultPrefab )
                {
                    itemsFit = deltaPos.magnitude / _defaultPrefabSize.x;
                    itemsFitRounded = Mathf.RoundToInt( itemsFit );
                    _fitScale = itemsFit / itemsFitRounded;

                    _isPrefabScaled = true;

                    if ( _isExtendedPrefabExist )
                    {
                        var itemsFit2 = deltaPos.magnitude / _extendedPrefabSize.x;
                        var roundedFit2 = Mathf.RoundToInt( itemsFit2 );
                        var fitScale2 = itemsFit2 / roundedFit2;

                        if ( Mathf.Abs( 1 - fitScale2 ) < Mathf.Abs( 1 - _fitScale ) )
                        {
                            _currentPrefab = _wallModule.ExtendedPrefab;
                            currentPrefabSize = _extendedPrefabSize;
                            DestroyCurrentWalls( );
                            return Mouse.FreeConstrPos;
                        }
                    }
                }
                else
                {
                    _isPrefabScaled = true;

                    itemsFit = deltaPos.magnitude / _extendedPrefabSize.x;
                    itemsFitRounded = Mathf.RoundToInt( itemsFit );
                    _fitScale = itemsFit / itemsFitRounded;

                    var itemsFit2 = deltaPos.magnitude / _defaultPrefabSize.x;
                    var roundedFit2 = Mathf.RoundToInt( itemsFit2 );
                    var fitScale2 = itemsFit2 / roundedFit2;

                    if ( Mathf.Abs( 1 - fitScale2 ) < Mathf.Abs( 1 - _fitScale ) )
                    {
                        _currentPrefab = _wallModule.DefaultPrefab;
                        currentPrefabSize = _defaultPrefabSize;
                        DestroyCurrentWalls( );
                        return Mouse.FreeConstrPos;
                    }
                }
            }
            else
            {
                if ( _isPrefabScaled )
                {
                    if ( _currentPrefab != _wallModule.DefaultPrefab )
                    {
                        _currentPrefab = _wallModule.DefaultPrefab;
                        currentPrefabSize = _defaultPrefabSize;
                        DestroyCurrentWalls( );
                    }

                    _isPrefabScaled = false;
                }


                #region AngleSnapping

                var constrTransform = MBSConstruction.Current.transform;
                var angle = Vector3.SignedAngle( Vector3.right, deltaPosNormalized, constrTransform.up );
                angle = ( angle < 0 ) ? 360 + angle : angle;

                var snapStep = SnapStepValueArray[ (int)SnapStep ];
                angle = Mathf.Round( angle / snapStep ) * snapStep;

                var toRot = Quaternion.AngleAxis( angle, Vector3.up ) * Vector3.right * deltaPos.magnitude;

                deltaPos = toRot;
                deltaPosNormalized = deltaPos.normalized;

                Handles.BeginGUI( );
                {
                    var style = new GUIStyle( "button" );
                    style.alignment = TextAnchor.MiddleLeft;
                    style.richText = true;
                    var text = angle + "°";
                    text = "<b>" + text + "</b>";

                    var rectSize = style.CalcSize( new GUIContent( text ) ) * 1.2f;
                    var screenPos = HandleUtility.WorldToGUIPoint( Mouse.FreeWorldPos );
                    var mouseOffset = new Vector2( 50, 0 );
                    var rect = new Rect( screenPos.x + mouseOffset.x, screenPos.y, rectSize.x, rectSize.y );

                    GUI.backgroundColor = new Color( 1.0f, 1.0f, 1.0f, 0.85f );
                    GUI.Box( rect, text, style );
                    GUI.backgroundColor = Color.white;
                }
                Handles.EndGUI( );

                #endregion


                itemsFit = deltaPos.magnitude / _defaultPrefabSize.x;
                itemsFitRounded = Mathf.RoundToInt( itemsFit );
            }


            if ( levelsNumber > _currentDrawingItems.Count )
            {
                var difference = levelsNumber - _currentDrawingItems.Count;
                for ( int i = 0; i < difference; i++ )
                {
                    _currentDrawingItems.Add( new List<GameObject>( ) );
                }
            }
            else if ( levelsNumber < _currentDrawingItems.Count )
            {
                var difference = levelsNumber - _currentDrawingItems.Count;
                for ( int i = _currentDrawingItems.Count - 1; i > levelsNumber; i++ )
                {
                    foreach ( var go in _currentDrawingItems[ i ] )
                    {
                        Object.DestroyImmediate( go );
                    }

                    _currentDrawingItems.RemoveAt( i );
                }
            }

            for ( int i = 0; i < _currentDrawingItems.Count; i++ )
            {
                if ( itemsFitRounded > _currentDrawingItems[ i ].Count )
                    for ( var j = _currentDrawingItems[ i ].Count; j < itemsFitRounded; j++ )
                    {
                        var existedNames = _currentDrawingGroup.transform.GetComponentsInChildren<Transform>( )
                                                               .Select( k => k.gameObject.name ).ToArray( );
                        var uniqueName = ObjectNames.GetUniqueName( existedNames, _currentPrefab.name );

                        var instantiatedWall = (GameObject)PrefabUtility.InstantiatePrefab( _currentPrefab );
                        instantiatedWall.transform.SetParent( _currentDrawingGroup.transform, false );
                        instantiatedWall.transform.name = uniqueName;
                        _currentDrawingItems[ i ].Add( instantiatedWall );
                    }
                else if ( itemsFitRounded < _currentDrawingItems[ i ].Count )
                    for ( var j = _currentDrawingItems[ i ].Count - 1; j >= itemsFitRounded; j-- )
                    {
                        Object.DestroyImmediate( _currentDrawingItems[ i ][ j ] );
                        _currentDrawingItems[ i ].RemoveAt( j );
                    }
            }


            var scaledSize = currentPrefabSize.x * _fitScale;
            var scaledSizeHalf = scaledSize / 2;

            for ( int i = 0; i < _currentDrawingItems.Count; i++ )
            {
                for ( var j = 0; j < _currentDrawingItems[ i ].Count; j++ )
                {
                    _currentDrawingItems[ i ][ j ].transform.localPosition =
                        Vector3.right * ( scaledSize * j + scaledSizeHalf ) + Vector3.up * levelHeight * i;

                    var localScale = _currentPrefab.transform.localScale;
                    _currentDrawingItems[ i ][ j ].transform.localScale =
                        new Vector3(
                            localScale.x * _fitScale,
                            localScale.y,
                            localScale.z );
                }
            }


            if ( deltaPosNormalized == Vector3.zero )
            {
                _currentDrawingGroup.transform.localRotation = Quaternion.identity;
            }
            else
            {
                _currentDrawingGroup.transform.localRotation =
                    Quaternion.LookRotation( Quaternion.AngleAxis( -90, Vector3.up ) * deltaPosNormalized, Vector3.up );
            }

            return _startPosConstr + deltaPosNormalized * scaledSize * _currentDrawingItems[ 0 ].Count;
        }


        public static void End( )
        {
            if ( _currentDrawingItems.Count == 0 )
            {
                Object.DestroyImmediate( _currentDrawingGroup );
            }
            else
            {
                Undo.IncrementCurrentGroup( );
                Undo.SetCurrentGroupName( Texts.Building.Wall.DRAWING_UNDO_NAME );

                var modularPack = ModularPack_Manager.Singleton.ModularPacks.ToList( )
                                                     .Find( i => i.WallGroups.FirstOrDefault(
                                                                     j => j.Guid == BuilderDataController.SelectedGroup
                                                                              .Guid ) != null );


                for ( var i = 0; i < _currentDrawingItems.Count; i++ )
                {
                    for ( var j = 0; j < _currentDrawingItems[ i ].Count; j++ )
                    {
                        var wallItem = _currentDrawingItems[ i ][ j ].AddComponent<MBSWallModule>( );


                        wallItem.WhenPlaced( MBSConstruction.Current,
                                             _extendScale, _fitScale,
                                             modularPack,
                                             (WallGroup)BuilderDataController.SelectedGroup,
                                             (WallModule)BuilderDataController.SelectedModule,
                                             _currentPrefab );

                        MBSConstruction.Current.AddWallAndConnect( wallItem );
                        MBSConstruction.Current.AddWallEndPoints( wallItem );


                        if ( j == 0 || j == _currentDrawingItems[ i ].Count - 1 || j % 2 != 0 )
                            WallConnectionController.RecalculateConnectionNodes( wallItem );

                        MBSConstruction.Current.AddAreasIfFound( wallItem );
                    }
                }

                _currentDrawingGroup.RecordCreatedUndo( );

                MBSConstruction.Current.UpdateEndPoints( );
                MBSConstruction.Current.UpdateAreas( );
            }

            _currentDrawingItems = null;
            _currentDrawingGroup = null;
            _wallModule = null;

            _is45DegreeDrawing = false;
            _extendScale = 1;

            _isPrefabScaled = false;
            _fitScale = 1;

            currentPrefabSize = Vector3.zero;
            _defaultPrefabSize = Vector3.zero;
            _extendedPrefabSize = Vector3.zero;

            _startPosConstr = Vector3.zero;
        }

        private static void EndWithoutSaving( )
        {
            DestroyCurrentWalls( );


            _currentDrawingItems = null;
            _currentDrawingGroup = null;
            _wallModule = null;

            _is45DegreeDrawing = false;
            _extendScale = 1;

            _isPrefabScaled = false;
            _fitScale = 1;

            currentPrefabSize = Vector3.zero;
            _defaultPrefabSize = Vector3.zero;
            _extendedPrefabSize = Vector3.zero;

            _startPosConstr = Vector3.zero;
        }

        private static void DestroyCurrentWalls( )
        {
            if ( _currentDrawingGroup != null )
                _currentDrawingGroup.DestroyImmediateUndo( );

            if ( _currentDrawingItems == null )
                return;

            for ( var i = 0; i < _currentDrawingItems.Count; i++ )
            {
                for ( var j = 0; j < _currentDrawingItems[ i ].Count; j++ )
                    Object.DestroyImmediate( _currentDrawingItems[ i ][ j ] );
            }


            _currentDrawingItems.Clear( );
        }

        public static void Clear( )
        {
            DestroyCurrentWalls( );

            _wallModule = null;

            _defaultPrefabSize = default;
            _isExtendedPrefabExist = false;
            _extendedPrefabSize = default;

            _startPosConstr = default;
            _startPosWorld = default;

            _currentPrefab = null;
            currentPrefabSize = default;

            _is45DegreeDrawing = false;
            _isPrefabScaled = false;

            _extendScale = 1;
            _fitScale = 1;
        }

        public static void ChangeSnapAccuracy_Next( )
        {
            var curIndex = (int)SnapStep;
            var enumLength = Enum.GetNames( typeof( WallAngleSnapStep ) ).Length;
            var nextTypeIndex = Collections_Helper.GetNextLoopIndex( curIndex, enumLength );
            SnapStep = (WallAngleSnapStep)nextTypeIndex;
        }
    }
}
#endif