#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Scene;
using MBS.Controller.Scene.Mono;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Scene
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( MBSWallModuleModifier ) )]
    public  class WallModule_Modifier_Editor : Editor
    {
        private GUIStyle _boldFoldoutStyle;

        private bool _areasFoldout;

        private int _currentModuleIndex;

        private bool _foldoutMeshModifiers;
        private bool _haveLockedConnections;

        private bool _isParentOk;

        private bool _isConstructionOk;

        private bool _isOriginalGroupOk;
        private bool _isOriginalGroupEmpty;

        private bool _isModuleOk;

        private bool _lockedConnectionsFoldout;


        private MBSWallModule[ ] _multiple;
        private bool _multipleChildrenMoreThanOne;
        private bool _isAllGroupOk;
        private bool _isAllSameGroup;
        private List<bool> _multipleFoldouts;
        private List<MBSWallModuleModifier[ ]> _multipleMeshModifiers;


        private bool _multipleSelection;

        private WallGroup _originalGroup;
        private WallModule _originalModule;
        private Vector2 _scrollPos;


        private MBSWallModule _single;
        private RoomArea[ ] _singleAreas;
        private MBSWallModuleModifier[ ] _singleMeshModifiers;
        private bool _twoWallsConnected;
        private bool _twoWallsLocked;

        private bool _bothComponentsAttached;


        private void OnEnable( )
        {
            if ( targets.Length == 1 )
                _multipleSelection = false;
            else
                _multipleSelection = true;

            _currentModuleIndex = -1;

            if ( !_multipleSelection )
            {
                var currentModifier = (MBSWallModuleModifier)target;
                _isParentOk = currentModifier.Root != null;

                if ( _isParentOk == false )
                    return;

                if ( currentModifier.GetComponent<MBSWallModule>( ) != null )
                    _bothComponentsAttached = true;

                if ( _bothComponentsAttached == true )
                    return;

                _single = currentModifier.Root;

                _singleMeshModifiers = _single.GetComponentsInChildren<MBSWallModuleModifier>( );

                _isConstructionOk = _single.MbsConstruction != null;
                if ( _isConstructionOk == false )
                    return;

                _originalGroup = _single.OriginalGroup;

                _isOriginalGroupOk = _originalGroup != null;
                if ( _isOriginalGroupOk == false )
                    return;

                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );
                if ( _isOriginalGroupEmpty )
                    return;

                _originalModule = _single.OriginalModule;
                _isModuleOk = _originalModule != null;

                if ( _isModuleOk )
                    _currentModuleIndex = _originalGroup.Modules.ToList( )
                                                        .FindIndex( i => i.Guid == _originalModule.Guid );
                else
                    _currentModuleIndex = 0;

                _singleAreas = _single.MbsConstruction.FindAreasWith( _single );
                _haveLockedConnections = _single.connectedToFront != null || _single.connectedToRear != null;
            }
            else
            {
                var currentModifiers = targets.Select( i => (MBSWallModuleModifier)i ).ToArray( );

                _isParentOk = currentModifiers.All( i => i.Root != null );
                if ( _isParentOk == false )
                    return;

                _bothComponentsAttached = currentModifiers.All( i => i.GetComponent<MBSWallModule>( ) != null );

                _multiple = currentModifiers.Select( i => i.Root ).ToArray( );

                _isConstructionOk = _multiple.All( i => i.MbsConstruction != null );
                if ( _isConstructionOk == false )
                    return;


                if ( _multiple.Length == 2 )
                {
                    _twoWallsConnected = AreTwoWallsConnected( _multiple.ElementAt( 0 ), _multiple.ElementAt( 1 ) );
                    _twoWallsLocked = AreTwoHaveLockedConnection( _multiple.ElementAt( 0 ), _multiple.ElementAt( 1 ) );
                }


                _isAllGroupOk = _multiple.All( i => i.OriginalGroup != null );
                if ( _isAllGroupOk == false )
                    return;


                _originalGroup = _multiple[ 0 ].OriginalGroup;


                _isAllSameGroup = _multiple.All( i => i.OriginalGroup == _originalGroup );

                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );

                var module = _multiple.Where( i => i.OriginalModule != null )
                                      .Select( i => i.OriginalModule )
                                      .FirstOrDefault( );

                _isModuleOk = module != null;
                if ( _isModuleOk )
                    _currentModuleIndex =
                        _originalGroup.Modules.ToList( ).IndexOf( _multiple[ 0 ].OriginalModule );
                else
                    _currentModuleIndex = 0;

                _originalModule = module;

                _multipleFoldouts = new List<bool>( );
                _multipleMeshModifiers = new List<MBSWallModuleModifier[ ]>( );

                for ( var i = 0; i < _multiple.Length; i++ )
                {
                    var meshModifiersList =
                        _multiple[ i ].GetComponentsInChildren<MBSWallModuleModifier>( ).ToArray( );
                    _multipleMeshModifiers.Add( meshModifiersList );
                    _multipleFoldouts.Add( false );
                }
            }
        }

        public  override void OnInspectorGUI( )
        {
            _boldFoldoutStyle = new GUIStyle( EditorStyles.foldout );
            _boldFoldoutStyle.font = EditorStyles.boldFont;

            if ( !_multipleSelection )
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

            if ( _bothComponentsAttached )
                return;

            if ( _isConstructionOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.PARENT_CONSTRUCTION_MISSING, MessageType.Error );
                return;
            }

            if ( _isOriginalGroupOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.SINGLE_GROUP_MISSING, MessageType.Error );
                return;
            }

            if ( _isModuleOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.MODULE_MISSING, MessageType.Error );
                return;
            }

            ShowSingleSelectionGrid( );


            GUILayout.Space( 5 );

            EditorGUILayout.BeginHorizontal( );
            EditorGUILayout.PrefixLabel( "Facing" );

            if ( GUILayout.Button( "Flip" ) )
                _single.FlipFace( );

            EditorGUILayout.EndHorizontal( );

            if ( _haveLockedConnections )
            {
                _lockedConnectionsFoldout = EditorGUILayout.Foldout( _lockedConnectionsFoldout,
                                                                     "Locked Connections", _boldFoldoutStyle );

                if ( _lockedConnectionsFoldout )
                {
                    if ( _single.connectedToFront != null )
                    {
                        EditorGUILayout.BeginHorizontal( );
                        EditorGUILayout.PrefixLabel( "Front connection" );

                        if ( GUILayout.Button( "Reset" ) )
                            _single.ResetFrontLockedConnection( );

                        EditorGUILayout.EndHorizontal( );
                    }

                    if ( _single.connectedToRear != null )
                    {
                        EditorGUILayout.BeginHorizontal( );
                        EditorGUILayout.PrefixLabel( "Rear connection" );

                        if ( GUILayout.Button( "Reset" ) )
                            _single.ResetRearLockedConnection( );

                        EditorGUILayout.EndHorizontal( );
                    }
                }
            }

            ShowAreaSelectionButtons( );
            ShowMeshModifiersAffectToggles( );

            EditorGUILayout.Space( 5, false );
        }

        private void MultipleInspector( )
        {
            if ( _isParentOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.CANNOT_GET_PARENT_MODULE_COMPONENT, MessageType.Error );
                return;
            }

            if ( _bothComponentsAttached )
            {
                return;
            }

            if ( _isConstructionOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.PARENT_CONSTRUCTION_MISSING, MessageType.Error );
                return;
            }

            if ( _isAllGroupOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.SOME_GROUP_MISSING, MessageType.Error );
                return;
            }

            if ( _isAllSameGroup == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.DIFFERENT_GROUPS, MessageType.Error );
            }
            else
            {
                if ( _isOriginalGroupEmpty )
                {
                    EditorGUILayout.HelpBox( Texts.Component.Editor.GROUP_EMPTY, MessageType.Error );
                    return;
                }

                ShowMultipleSelectionGrid( );
            }

            if ( GUILayout.Button( "Flip face" ) )
                foreach ( var i in _multiple )
                    i.FlipFace( );

            if ( _twoWallsConnected && !_twoWallsLocked )
                if ( GUILayout.Button( "Connect" ) )
                {
                    LockConnectionBetween( _multiple[ 0 ], _multiple[ 1 ] );
                    _twoWallsLocked =
                        AreTwoHaveLockedConnection( _multiple.ElementAt( 0 ), _multiple.ElementAt( 1 ) );
                }

            if ( _twoWallsConnected && _twoWallsLocked )
                if ( GUILayout.Button( "Disconnect" ) )
                {
                    UnlockConnectionBetween( _multiple[ 0 ], _multiple[ 1 ] );
                    _twoWallsLocked =
                        AreTwoHaveLockedConnection( _multiple.ElementAt( 0 ), _multiple.ElementAt( 1 ) );
                }

            ShowMeshModifiersAffectToggles( );
        }


        private void ShowSingleSelectionGrid( )
        {
            EditorGUILayout.LabelField( "Wall Group Modules" );

            SelectionGrid.ModuleSelectionGrid( (int)EditorGUIUtility.currentViewWidth,
                                               200,
                                               w => { _single.ChangeModule( w as WallModule, false ); },
                                               _originalModule,
                                               _currentModuleIndex,
                                               _originalGroup.Modules,
                                               ref _scrollPos );
        }

        
        private void ShowMultipleSelectionGrid( )
        {
            EditorGUILayout.LabelField( "Wall Group Modules" );

            SelectionGrid.ModuleSelectionGrid( (int)EditorGUIUtility.currentViewWidth, 200, w =>
            {
                Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_WALL_MODULE_CHANGED );
                var construction = _multiple[ 0 ].MbsConstruction;
                for ( var i = 0; i < _multiple.Length; i++ )
                {
                    construction = _multiple[ i ].MbsConstruction;
                    if ( construction == null )
                        continue;

                    construction.RemoveWallAndDisconnect( _multiple[ i ] );
                    construction.RemoveAreasWithWall( _multiple[ i ] );
                }

                var changedDWs = new List<MBSWallModule>( );
                for ( var i = 0; i < _multiple.Length; i++ )
                    changedDWs.Add( _multiple[ i ].ChangeModule( w as WallModule, true ) );


                construction = changedDWs[ 0 ].MbsConstruction;
                var instantiated = new List<MBSWallModule>( );
                for ( var i = 0; i < changedDWs.Count; i++ )
                {
                    WallConnectionController.RecalculateConnectionNodes( changedDWs[ i ] );

                    foreach ( var c in changedDWs[ i ].frontConnections )
                        if ( !changedDWs.Contains( c ) && !instantiated.Contains( c ) )
                        {
                            WallConnectionController.RecalculateConnectionNodes( c );
                            instantiated.Add( c );
                        }

                    foreach ( var c in changedDWs[ i ].rearConnections )
                        if ( !changedDWs.Contains( c ) && !instantiated.Contains( c ) )
                        {
                            WallConnectionController.RecalculateConnectionNodes( c );
                            instantiated.Add( c );
                        }
                }

                construction.UpdateAreas( );
            }, _originalModule, _currentModuleIndex, _originalGroup.Modules, ref _scrollPos );
        }

        private bool AreTwoWallsConnected( MBSWallModule wall1, MBSWallModule wall2 )
        {
            if ( wall1.frontConnections == null && wall1.rearConnections == null )
                return false;

            if ( wall1.frontConnections.Count == 0 && wall1.rearConnections.Count == 0 )
                return false;

            if ( wall2.frontConnections == null && wall2.rearConnections == null )
                return false;

            if ( wall2.frontConnections.Count == 0 && wall2.rearConnections.Count == 0 )
                return false;

            if ( !wall1.frontConnections.Contains( wall2 ) && !wall1.rearConnections.Contains( wall2 ) )
                return false;

            if ( !wall2.frontConnections.Contains( wall1 ) && !wall2.rearConnections.Contains( wall1 ) )
                return false;

            return true;
        }

        private void LockConnectionBetween( MBSWallModule wall1, MBSWallModule wall2 )
        {
            wall1.LockConnectionWith( wall2 );
            wall2.LockConnectionWith( wall1 );

            WallConnectionController.RecalculateConnectionNodes( wall1 );
        }

        private void UnlockConnectionBetween( MBSWallModule wall1, MBSWallModule wall2 )
        {
            wall1.UnlockConnectionWith( wall2 );
            wall2.UnlockConnectionWith( wall1 );

            WallConnectionController.RecalculateConnectionNodes( wall1 );
        }

        private void ShowAreaSelectionButtons( )
        {
            if ( _singleAreas != null && _singleAreas.Length > 0 )
            {
                _areasFoldout = EditorGUILayout.Foldout( _areasFoldout, "Areas", _boldFoldoutStyle );

                if ( _areasFoldout )
                {
                    EditorGUILayout.BeginHorizontal( );
                    EditorGUILayout.Space( 5, false );
                    EditorGUILayout.BeginVertical( );

                    for ( var i = 0; i < _singleAreas.Length; i++ )
                    {
                        EditorGUILayout.BeginHorizontal( );
                        EditorGUILayout.PrefixLabel( i + 1 + ". Area (" + _singleAreas[ i ].area + ")" );
                        if ( GUILayout.Button( "Select all elements" ) ) _singleAreas[ i ].SelectAreaItems( );

                        EditorGUILayout.EndHorizontal( );
                    }

                    EditorGUILayout.EndVertical( );
                    EditorGUILayout.EndHorizontal( );
                    EditorGUILayout.Space( 5, false );
                }
            }
        }

        private void ShowMeshModifiersAffectToggles( )
        {
            if ( !_multipleSelection )
            {
                if ( _singleMeshModifiers.Length > 0 )
                {
                    _foldoutMeshModifiers = EditorGUILayout.Foldout( _foldoutMeshModifiers, "Children Modifications",
                                                                     true, _boldFoldoutStyle );

                    if ( _foldoutMeshModifiers )
                    {
                        EditorGUILayout.BeginHorizontal( );
                        EditorGUILayout.Space( 5, false );
                        EditorGUILayout.BeginVertical( );
                        EditorGUI.BeginChangeCheck( );
                        for ( var i = 0; i < _singleMeshModifiers.Length; i++ )
                            _singleMeshModifiers[ i ].doModify = EditorGUILayout.Toggle(
                                _singleMeshModifiers[ i ].gameObject.name,
                                _singleMeshModifiers[ i ].doModify );
                        if ( EditorGUI.EndChangeCheck( ) ) WallConnectionController.UpdateMesh( _single );
                        EditorGUILayout.EndVertical( );
                        EditorGUILayout.EndHorizontal( );
                    }
                }
            }
            else
            {
                if ( _multipleMeshModifiers.Count > 0 )
                {
                    _foldoutMeshModifiers = EditorGUILayout.Foldout( _foldoutMeshModifiers, "Children Modifications",
                                                                     true, _boldFoldoutStyle );

                    if ( _foldoutMeshModifiers )
                    {
                        EditorGUILayout.BeginHorizontal( );
                        EditorGUILayout.Space( 10, false );

                        EditorGUILayout.BeginVertical( );

                        for ( var i = 0; i < _multipleMeshModifiers.Count; i++ )
                            if ( _multipleMeshModifiers[ i ].Length > 0 )
                            {
                                _multipleFoldouts[ i ] = EditorGUILayout.Foldout( _multipleFoldouts[ i ],
                                    _multiple[ i ].gameObject.name, true );

                                if ( _multipleFoldouts[ i ] )
                                {
                                    EditorGUILayout.BeginHorizontal( );
                                    EditorGUILayout.Space( 5, false );

                                    EditorGUILayout.BeginVertical( );

                                    EditorGUI.BeginChangeCheck( );
                                    {
                                        for ( var j = 0; j < _multipleMeshModifiers[ i ].Length; j++ )
                                            _multipleMeshModifiers[ i ][ j ].doModify = EditorGUILayout.Toggle(
                                                _multipleMeshModifiers[ i ][ j ].gameObject.name,
                                                _multipleMeshModifiers[ i ][ j ].doModify );
                                        if ( EditorGUI.EndChangeCheck( ) )
                                            WallConnectionController.UpdateMesh( _multiple[ i ] );
                                    }

                                    EditorGUILayout.EndVertical( );
                                    EditorGUILayout.EndHorizontal( );
                                }
                            }

                        EditorGUILayout.EndVertical( );

                        EditorGUILayout.EndHorizontal( );
                    }
                }
            }
        }

        private bool AreTwoHaveLockedConnection( MBSWallModule wall1, MBSWallModule wall2 )
        {
            if ( wall1.connectedToFront == wall2 || wall1.connectedToRear == wall2 )
                if ( wall2.connectedToFront == wall1 || wall2.connectedToRear == wall1 )
                    return true;
            return false;
        }
    }
}

#endif