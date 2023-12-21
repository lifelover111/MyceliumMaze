#if UNITY_EDITOR

using System;
using MBS.Controller.Builder;
using MBS.Model.Configuration;
using MBS.Model.Scene;
using MBS.Utilities.Extensions;
using MBS.View.Builder;
using MBS.View.Input.GUI;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class Pointer_Mode
    {
        private static float _value;

        public  static SceneFuncData DoPointerMode( Event e )
        {
            SceneFuncData retval = new SceneFuncData( );
            retval.rotation = Quaternion.identity;

            if ( Mouse.IsMouseInScene == false )
                return retval;

            var ray = Mouse.WorldRay;
            var isTopOrBot = false;

            if ( Physics.Raycast( ray, out var hit, 1000 ) )
            {
                var hitPointWorld = hit.point;

                var tr = hit.collider.transform;
                var hitPointLocal = tr.InverseTransformPoint( hit.point );
                if ( tr.TryGetComponent<MeshFilter>( out var mf ) )
                {
                    var meshBounds = mf.sharedMesh.bounds;
                    var center = meshBounds.center;
                    var extents = meshBounds.extents;
                    var diff = hitPointLocal - center;

                    if ( Mathf.Abs( diff.y ).ApxEquals( extents.y ) ) isTopOrBot = true;

                    retval.isOk = true;
                    retval.matrix = Matrix4x4.TRS( hitPointWorld, tr.rotation, Vector3.one );
                    retval.position = hitPointWorld;
                    retval.defaultOrAccent = isTopOrBot;
                }

                _value = hitPointWorld.y.RoundDecimals( );
                
                Handles.BeginGUI( );
                {
                    var style = new GUIStyle( "button" );
                    style.alignment = TextAnchor.MiddleLeft;
                    style.richText = true;
                    var text = _value.ToString();
                    text = "<b>" + text + "</b>";

                    var rectSize = style.CalcSize( new GUIContent( text ) ) * 1.2f;
                    var screenPos = HandleUtility.WorldToGUIPoint( hit.point ) + Vector2.right;
                    var mouseOffset = new Vector2( 30, 0 );
                    var rect = new Rect( screenPos.x + mouseOffset.x, screenPos.y, rectSize.x, rectSize.y );

                    UnityEngine.GUI.backgroundColor = new Color( 1.0f, 1.0f, 1.0f, 0.85f );
                    UnityEngine.GUI.Box( rect, text, style );
                    UnityEngine.GUI.backgroundColor = Color.white;
                }
            }

            return retval;
        }

        public  static void Setup_Inputs( Action<float> mouseClickAction )
        {
            InputManager.ClearInputData( );


            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Pointer.MOUSE_ACTION_LABEL,
                KeyAction = ( ) =>
                {
                    SceneData.IsMouseRecalcNeeded = true;
                    mouseClickAction?.Invoke( _value );
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
                }
            } );

            InputManager.AddKeyElement( true,new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = ( ) =>
                {
                    SceneData.IsMouseRecalcNeeded = true;
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder);
                }
            } );
        }

        public  static void Clear( )
        {
            _value = default;
        }
    }
}

#endif