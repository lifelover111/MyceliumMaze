#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Builder
{
    public  class SelectionGrid
    {
        private const int BUTTON_SIZE = 54;
        private const int BUTTON_PADDING = 3;
        private const int PREVIEW_IMAGE_SIZE = 96;

        public  static void GroupsSelectionGrid( int rectWidth, int rectHeight,
                                                int selectedGroupIndex, ModularGroup[ ] selectedGroups,
                                                Vector2 scrollPos,
                                                Action<int> onGroupSelected, Action<Vector2> onScroll )
        {
            if ( selectedGroups == null || selectedGroups.Length == 0 )
            {
                EditorGUILayout.BeginHorizontal( GUILayout.MinHeight( PREVIEW_IMAGE_SIZE ),
                                                 GUILayout.Height( 200 ), GUILayout.MaxHeight( rectHeight - 180 ) );

                EditorGUILayout.HelpBox( Texts.Builder.Window.Errors.SELECTION_GRID_NO_GROUPS,
                                         MessageType.Error );

                EditorGUILayout.EndHorizontal( );
                return;
            }

            var itemsNumber = selectedGroups.Length;

            var totalColumnsNumber =
                Mathf.RoundToInt( ( rectWidth - ( PREVIEW_IMAGE_SIZE + 14 ) ) / ( BUTTON_SIZE + BUTTON_PADDING * 2 ) );
            var clampedColumnsNumber = Mathf.Clamp( totalColumnsNumber, 1, int.MaxValue );
            var totalRowsNumber = Mathf.CeilToInt( itemsNumber / (float)clampedColumnsNumber );
            var clampedRowsNumber = Mathf.Clamp( totalRowsNumber, 1, int.MaxValue );

            var horizontalHeight = totalRowsNumber * ( BUTTON_SIZE + BUTTON_PADDING * 2 );
            horizontalHeight = Mathf.Clamp( horizontalHeight, PREVIEW_IMAGE_SIZE, int.MaxValue );


            EditorGUILayout.BeginHorizontal( GUILayout.MinHeight( PREVIEW_IMAGE_SIZE ),
                                             GUILayout.Height( horizontalHeight ),
                                             GUILayout.MaxHeight( rectHeight - 180 ) );
            {
                 
                var itemPreviewRect = EditorGUILayout.GetControlRect( false, GUILayout.Width( PREVIEW_IMAGE_SIZE ),
                                                                      GUILayout.Height( PREVIEW_IMAGE_SIZE ) );
                itemPreviewRect.position = itemPreviewRect.position + Vector2.up * 2;
                EditorGUI.DrawPreviewTexture( itemPreviewRect,
                                              ModularGroup.GetPreviewOrEmptyIcon(
                                                  selectedGroups.ElementAtOrDefault( selectedGroupIndex ) ) );

                 
                var verticalStyle = new GUIStyle( "box" );
                verticalStyle.padding = new RectOffset( 3, 3, 3, 3 );

                var prevScrollPos = scrollPos;

                var newScrollPos =
                    EditorGUILayout.BeginScrollView( prevScrollPos, verticalStyle,
                                                     GUILayout.MaxHeight( horizontalHeight ) );
                {
                    GUILayout.BeginVertical( );
                    {
                        var thumbnails = new GUIContent[ selectedGroups.Length ];

                        for ( var i = 0; i < thumbnails.Length; i++ )
                        {
                            var group = selectedGroups.ElementAtOrDefault( i );
                            var preivewTexture = ModularGroup.GetPreviewOrEmptyIcon( group );
                            thumbnails[ i ] = new GUIContent( preivewTexture, group.Name );
                        }

                        GUIStyle style = new GUIStyle( "button" );
                        style.active.background = Texture2D.grayTexture;
                        style.fixedWidth = BUTTON_SIZE;
                        style.fixedHeight = BUTTON_SIZE;
                        style.padding =
                            new RectOffset( BUTTON_PADDING, BUTTON_PADDING, BUTTON_PADDING, BUTTON_PADDING );

                        var rect = GUILayoutUtility.GetRect( rectWidth - ( PREVIEW_IMAGE_SIZE + 10 * 5 ),
                                                             horizontalHeight - 10 );

                        EditorGUI.BeginChangeCheck( );
                        {
                            var prevIndex = Math.Min( selectedGroupIndex, itemsNumber - 1 );
                            prevIndex = Mathf.Clamp( prevIndex, 0, itemsNumber );

                            var newIndex = GUI.SelectionGrid( rect, prevIndex, thumbnails, clampedColumnsNumber,
                                                              style );

                            if ( EditorGUI.EndChangeCheck( ) ) onGroupSelected?.Invoke( newIndex );
                        }
                    }
                    GUILayout.EndVertical( );
                }
                EditorGUILayout.EndScrollView( );
                onScroll.Invoke( newScrollPos );
            }
            EditorGUILayout.EndHorizontal( );
        }


        public  static void ModuleSelectionGrid( int rectWidth, int rectHeight,
                                                Action<Module> onModuleSelected,
                                                Module currentModule,
                                                int currentModuleIndex,
                                                Module[ ] modules,
                                                ref Vector2 scrollPos )
        {
            var itemsNumber = modules.Length;

            var totalColumnsNumber =
                Mathf.RoundToInt( ( rectWidth - ( PREVIEW_IMAGE_SIZE + 14 ) ) / ( BUTTON_SIZE + BUTTON_PADDING * 2 ) );
            var clampedColumnsNumber = Mathf.Clamp( totalColumnsNumber, 1, int.MaxValue );
            var totalRowsNumber = Mathf.CeilToInt( itemsNumber / (float)clampedColumnsNumber );
            var clampedRowsNumber = Mathf.Clamp( totalRowsNumber, 1, int.MaxValue );

            var horizontalHeight = totalRowsNumber * ( BUTTON_SIZE + BUTTON_PADDING * 2 );
            horizontalHeight = Mathf.Clamp( horizontalHeight, PREVIEW_IMAGE_SIZE, int.MaxValue );


            EditorGUILayout.BeginHorizontal( GUILayout.MinHeight( PREVIEW_IMAGE_SIZE ),
                                             GUILayout.Height( horizontalHeight ),
                                             GUILayout.MaxHeight( rectHeight - 180 ) );
            {
                 
                var itemPreviewRect = EditorGUILayout.GetControlRect( false, GUILayout.Width( PREVIEW_IMAGE_SIZE ),
                                                                      GUILayout.Height( PREVIEW_IMAGE_SIZE ) );
                itemPreviewRect.position = itemPreviewRect.position + Vector2.up * 2;
                EditorGUI.DrawPreviewTexture( itemPreviewRect, currentModule.GetPreviewOrEmptyIcon( ) );

                 
                var verticalStyle = new GUIStyle( "box" );
                verticalStyle.padding = new RectOffset( 3, 3, 3, 3 );

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos, verticalStyle,
                                                             GUILayout.MaxHeight( horizontalHeight ) );
                {
                    GUILayout.BeginVertical( );
                    {
                        if ( modules.Length == 0 )
                        {
                            EditorGUILayout.HelpBox( Texts.Builder.Window.Errors.SELECTION_GRID_NO_MODULES,
                                                     MessageType.Error );

                            GUILayout.EndVertical( );
                            GUILayout.EndScrollView( );
                            EditorGUILayout.EndHorizontal( );
                            return;
                        }

                        var thumbnails = new GUIContent[ modules.Length ];

                        for ( var i = 0; i < thumbnails.Length; i++ )
                        {
                            var module = modules.ElementAtOrDefault( i );
                            var previewTexture = module.GetPreviewOrEmptyIcon( );
                            thumbnails[ i ] = new GUIContent( previewTexture, module.Name );
                        }

                        GUIStyle style = new GUIStyle( "button" );
                        style.active.background = Texture2D.grayTexture;
                        style.fixedWidth = BUTTON_SIZE;
                        style.fixedHeight = BUTTON_SIZE;
                        style.padding =
                            new RectOffset( BUTTON_PADDING, BUTTON_PADDING, BUTTON_PADDING, BUTTON_PADDING );

                        var rect = GUILayoutUtility.GetRect( rectWidth - ( PREVIEW_IMAGE_SIZE + 10 * 5 ),
                                                             horizontalHeight - 10 );

                        EditorGUI.BeginChangeCheck( );
                        {
                            var prevIndex = Math.Min( currentModuleIndex, itemsNumber - 1 );
                            prevIndex = Mathf.Clamp( prevIndex, 0, itemsNumber );

                            var newIndex = GUI.SelectionGrid( rect, prevIndex, thumbnails, clampedColumnsNumber,
                                                              style );
                            if ( EditorGUI.EndChangeCheck( ) )
                                onModuleSelected?.Invoke( modules[ newIndex ] );
                        }
                    }
                    GUILayout.EndVertical( );
                }
                EditorGUILayout.EndScrollView( );
            }
            EditorGUILayout.EndHorizontal( );
        }
    }
}

#endif