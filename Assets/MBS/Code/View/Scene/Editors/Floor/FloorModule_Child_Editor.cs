#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Scene.Mono;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Scene
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( MBSFloorModule_Child ) )]
    public  class FloorModule_Child_Editor : Editor
    {
        private GUIStyle _boldFoldoutStyle;

        private int _currentModuleIndex;


        private FloorGroup _originalGroup;
        private FloorModule _originalModule;

        private MBSFloorModule _single;
        private bool _isOriginalGroupOk;
        private bool _isOriginalGroupEmpty;
        private bool _isOriginalModuleOk;
        private bool _isSingleTriangle;
        private bool _isSingleHasTrianglePrefab;

        private MBSFloorModule[ ] _multiple;
        private bool _isAllGroupsOk;
        private bool _isAllGroupsSame;
        private bool _isAllModulesOk;
        private bool _isAllSquare;
        private bool _isAllTriangles;
        private bool _isAllHaveTrianglePrefab;

        private bool _isMultiple;

        private bool _bothComponentsAttached;

        private Vector2 _scrollPos;

        private float _windowWidth;
        private bool _isParentOk;


        private void OnEnable( )
        {
            _currentModuleIndex = -1;


            if ( targets.Length == 1 )
                _isMultiple = false;
            else
                _isMultiple = true;


            if ( _isMultiple == false )
            {
                var targetObject = (MBSFloorModule_Child)target;
                 
                 
                 

                var targetParent = targetObject.gameObject.GetComponentInParent<MBSFloorModule>( true );

                if ( targetParent != null )
                    _isParentOk = true;
                else
                    return;

                _single = targetParent;

                _originalGroup = _single.OrigGroup;
                _isOriginalGroupOk = _originalGroup != null;
                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );

                if ( !_isOriginalGroupOk ) return;
                if ( _isOriginalGroupEmpty ) return;

                _originalModule = _single.OrigModule;
                _isOriginalModuleOk = _originalModule != null;

                if ( _isOriginalModuleOk )
                {
                    _currentModuleIndex = _originalGroup.Modules
                                                        .ToList( )
                                                        .FindIndex( i => i.Guid == _originalModule.Guid );
                    _isSingleHasTrianglePrefab = _originalModule.TriangularPrefab != null;
                }

                if ( _currentModuleIndex < 0 )
                    _currentModuleIndex = 0;

                if ( _single.Data.tileType == FloorTileType.Triangle )
                    _isSingleTriangle = true;
            }
            else
            {
                var targetsCasted = targets.Cast<MBSFloorModule_Child>( ).ToArray( );
                 
                 
                 

                var targetsRoots = targetsCasted.Select( i => i.GetComponentInParent<MBSFloorModule>( true ) );
                _multiple = targetsRoots.Distinct( ).ToArray( );

                if ( _multiple != null && _multiple.Length > 0 )
                    _isParentOk = true;
                else
                    _isParentOk = false;

                _isAllGroupsOk = _multiple.All( i => i.OrigGroup != null );
                if ( !_isAllGroupsOk ) return;

                _originalGroup = _multiple[ 0 ].OrigGroup;

                _isAllGroupsSame = _multiple.All( i => i.OrigGroup == _originalGroup );
                if ( !_isAllGroupsSame ) return;

                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );
                if ( _isOriginalGroupEmpty ) return;

                _isAllModulesOk = _multiple.All( i => i.OrigModule != null );
                if ( !_isAllModulesOk ) return;

                _isAllSquare = _multiple.All( i => i.Data.tileType == FloorTileType.Square );
                _isAllTriangles = _multiple.All( i => i.Data.tileType == FloorTileType.Triangle );

                _originalGroup = _multiple[ 0 ].OrigGroup;

                _originalModule = _multiple.Select( i => i.OrigModule ).FirstOrDefault( );

                _currentModuleIndex = _originalGroup.Modules.ToList( ).IndexOf( _originalModule );
                if ( _currentModuleIndex < 0 )
                    _currentModuleIndex = 0;
            }
        }

        public  override void OnInspectorGUI( )
        {
             
             

            _windowWidth = EditorGUIUtility.currentViewWidth - 25;

            _boldFoldoutStyle = new GUIStyle( EditorStyles.foldout );
            _boldFoldoutStyle.font = EditorStyles.boldFont;

            if ( !_isMultiple )
                SingleInspector( );
            else
                MultipleInspector( );
        }

        private void SingleInspector( )
        {
            if ( _isParentOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.CANNOT_GET_PARENT_MODULE_COMPONENT, MessageType.Error );
                return;
            }

            if ( _isOriginalGroupOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.SINGLE_GROUP_MISSING, MessageType.Error );
                return;
            }

            if ( _isOriginalGroupEmpty )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.GROUP_EMPTY, MessageType.Error );
                return;
            }

            if ( _isOriginalModuleOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.MODULE_MISSING, MessageType.Error );
                return;
            }

            ShowSingleSelectionGrid( );

            ChangeTileTypeButton( false );


            if ( _isSingleTriangle == false )
            {
                if ( _isSingleHasTrianglePrefab )
                {
                    SplitSquare( _isMultiple );
                }
            }

            if ( _isSingleTriangle )
            {
                RotationButtons( false );
            }

            GUILayout.Space( 5 );
            EditorGUILayout.Space( 5, false );
        }


        private void ShowSingleSelectionGrid( )
        {
            EditorGUILayout.LabelField( "Group Modules" );

            var index = _currentModuleIndex;
            if ( index < 0 )
                index = 0;


            SelectionGrid.ModuleSelectionGrid(
                (int)EditorGUIUtility.currentViewWidth,
                200,
                w =>
                {
                    Undo.IncrementCurrentGroup( );
                    Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_FLOOR_MODULE_CHANGED );
                    _single.ChangeModule( w as FloorModule, false );
                },
                _originalModule,
                _currentModuleIndex,
                _originalGroup.Modules as Module[ ],
                ref _scrollPos );
        }


        private void MultipleInspector( )
        {
            if ( _isParentOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.CANNOT_GET_PARENT_MODULE_COMPONENT, MessageType.Error );
                return;
            }

            if ( _isAllGroupsOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.SOME_GROUP_MISSING, MessageType.Error );
                return;
            }

            if ( _isOriginalGroupEmpty )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.GROUP_EMPTY, MessageType.Error );
                return;
            }

            if ( _isAllGroupsSame == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.DIFFERENT_GROUPS, MessageType.Error );
            }
            else
                ShowMultipleSelectionGrid( );

            ChangeTileTypeButton( true );

            if ( _isAllSquare )
            {
                SplitSquare( true );
            }

            RotationButtons( true );
        }

        private void ShowMultipleSelectionGrid( )
        {
            SelectionGrid.ModuleSelectionGrid(
                (int)EditorGUIUtility.currentViewWidth,
                200,
                w =>
                {
                    var changedDWs = new List<MBSFloorModule>( );

                    Undo.IncrementCurrentGroup( );
                    Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_FLOOR_MODULE_CHANGED );

                    for ( var i = 0; i < _multiple.Length; i++ )
                        changedDWs.Add( _multiple[ i ].ChangeModule( w as FloorModule, true ) );
                },
                _originalModule,
                _currentModuleIndex,
                _originalGroup.Modules as Module[ ],
                ref _scrollPos );
        }


        private void ChangeTileTypeButton( bool isMultiple )
        {
            EditorGUILayout.Space( 3 );

            EditorGUILayout.BeginHorizontal( );

            EditorGUILayout.PrefixLabel( "Change tile type" );


            string buttonText;
            if ( isMultiple == false )
            {
                if ( _single.Data.tileType == FloorTileType.Square )
                    buttonText = "To Triangle";
                else
                    buttonText = "To Square";
            }
            else
            {
                buttonText = "Change";
            }


            if ( GUILayout.Button( buttonText ) )
            {
                Undo.IncrementCurrentGroup( );
                Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_FLOOR_TYPE_CHANGED );

                if ( isMultiple == false )
                    _single.ChangeTileType( false );
                else
                {
                    for ( var i = 0; i < _multiple.Length; i++ )
                    {
                        _multiple[ i ].ChangeTileType( true );
                    }
                }
            }

            EditorGUILayout.EndHorizontal( );
        }

        private void RotationButtons( bool isMultiple )
        {
            EditorGUILayout.BeginHorizontal( );

            EditorGUILayout.PrefixLabel( "Rotate tile" );

            if ( GUILayout.Button( "Left" ) )
            {
                if ( isMultiple == false )
                    _single.Rotate( false, false );
                else
                {
                    for ( var i = 0; i < _multiple.Length; i++ )
                    {
                        _multiple[ i ].Rotate( false, true );
                    }
                }
            }

            if ( GUILayout.Button( "Right" ) )
            {
                if ( isMultiple == false )
                    _single.Rotate( true, false );
                else
                {
                    for ( var i = 0; i < _multiple.Length; i++ )
                    {
                        _multiple[ i ].Rotate( true, true );
                    }
                }
            }

            EditorGUILayout.EndHorizontal( );
        }

        private void SplitSquare( bool isMultiple )
        {
            EditorGUILayout.BeginHorizontal( );

            EditorGUILayout.PrefixLabel( "Split into two triangles" );

            if ( GUILayout.Button( "Split" ) )
            {
                Undo.IncrementCurrentGroup( );
                Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_FLOOR_TILE_SPLIT );

                if ( isMultiple == false )
                    _single.SplitSquareIntoTriangles( false );
                else
                {
                    for ( var i = 0; i < _multiple.Length; i++ )
                    {
                        _multiple[ i ].SplitSquareIntoTriangles( true );
                    }
                }
            }

            EditorGUILayout.EndHorizontal( );
        }
    }
}
#endif