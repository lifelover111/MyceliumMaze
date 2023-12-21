#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Scene;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Scene
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( MBSDecoratorModule_Child ) )]
    public  class DecoratorModule_Child_Editor : Editor
    {
        private GUIStyle _boldFoldoutStyle;

        private int _currentModuleIndex;


        private DecoratorGroup _originalGroup;
        private DecoratorModule _originalModule;

        private MBSDecoratorModule _single;
        private bool _isOriginalGroupOk;
        private bool _isOriginalGroupEmpty;
        private bool _isSingleModuleOk;
        private bool _isSingleTriangle;
        private bool _isSingleHasTrianglePrefab;

        private MBSDecoratorModule[ ] _multiple;
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


        private void OnEnable( )
        {
            _currentModuleIndex = -1;


            if ( targets.Length == 1 )
                _isMultiple = false;
            else
                _isMultiple = true;


            if ( _isMultiple == false )
            {
                var targetObject = (MBSDecoratorModule_Child)target;
                _bothComponentsAttached = targetObject.GetComponent<MBSDecoratorModule>( ) != null;

                if ( _bothComponentsAttached )
                    return;

                var targetParent = targetObject.gameObject.GetComponentInParent<MBSDecoratorModule>( true );

                _single = targetParent;


                _originalGroup = _single.OrigGroup;
                _isOriginalGroupOk = _originalGroup != null;
                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );

                if ( !_isOriginalGroupOk ) return;
                if ( _isOriginalGroupEmpty ) return;

                _originalModule = _single.OrigModule;
                _isSingleModuleOk = _originalModule != null;

                if ( _isSingleModuleOk )
                {
                    _currentModuleIndex = _originalGroup.Modules
                                                        .ToList( )
                                                        .FindIndex( i => i.Guid == _originalModule.Guid );
                }

                if ( _currentModuleIndex < 0 )
                    _currentModuleIndex = 0;
            }
            else
            {
                var targetsCasted = targets.Cast<MBSDecoratorModule_Child>( ).ToArray( );

                var targetsRoots = targetsCasted.Select( i => i.GetComponentInParent<MBSDecoratorModule>( ) );
                _multiple = targetsRoots.Distinct( ).ToArray( );

                _isAllGroupsOk = _multiple.All( i => i.OrigGroup != null );
                if ( !_isAllGroupsOk ) return;

                _originalGroup = _multiple[ 0 ].OrigGroup;

                _isAllGroupsSame = _multiple.All( i => i.OrigGroup == _originalGroup );
                if ( !_isAllGroupsSame ) return;

                _isOriginalGroupEmpty = _originalGroup.IsEmpty( );

                _isAllModulesOk = _multiple.All( i => i.OrigModule != null );
                if ( !_isAllModulesOk ) return;


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

            if ( _isSingleModuleOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.MODULE_MISSING, MessageType.Error );
                return;
            }

            ShowSingleSelectionGrid( );

            GUILayout.Space( 5 );
            EditorGUILayout.Space( 5, false );
        }


        private void ShowSingleSelectionGrid( )
        {
            EditorGUILayout.LabelField( "Group Modules" );

            var index = _currentModuleIndex;
            if ( index < 0 )
                index = 0;


            SelectionGrid.ModuleSelectionGrid( (int)EditorGUIUtility.currentViewWidth,
                                               200,
                                               w =>
                                               {
                                                   Undo.IncrementCurrentGroup( );
                                                   Undo.SetCurrentGroupName(
                                                       Texts.Component.Editor.UNDO_DECOR_MODULE_CHANGED );
                                                   _single.ChangeModule( w as DecoratorModule, false );
                                               },
                                               _originalModule,
                                               _currentModuleIndex,
                                               _originalGroup.Modules as Module[ ],
                                               ref _scrollPos );
        }


        private void MultipleInspector( )
        {
            if ( _isAllGroupsOk == false )
            {
                EditorGUILayout.HelpBox( Texts.Component.Editor.SOME_GROUP_MISSING, MessageType.Error );
                return;
            }

            if ( _isAllGroupsSame == false )
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
        }

        private void ShowMultipleSelectionGrid( )
        {
            SelectionGrid.ModuleSelectionGrid(
                (int)EditorGUIUtility.currentViewWidth,
                200,
                w =>
                {
                    var changedDWs = new List<MBSDecoratorModule>( );

                    Undo.IncrementCurrentGroup( );
                    Undo.SetCurrentGroupName( Texts.Component.Editor.UNDO_DECOR_MODULE_CHANGED );

                    for ( var i = 0; i < _multiple.Length; i++ )
                        changedDWs.Add( _multiple[ i ].ChangeModule( w as DecoratorModule, true ) );
                },
                _originalModule,
                _currentModuleIndex,
                _originalGroup.Modules as Module[ ],
                ref _scrollPos );
        }
    }
}

#endif