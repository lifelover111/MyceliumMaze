#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using MBS.Model.Scene;
using MBS.View.Input.GUI;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class InputPhysical
    {
        public  static Action<Event> MouseMove;
        public  static List<Action> PointerUp;
        public  static List<(KeyCode key, Action action)> KeyUp;


        public  static void Start( )
        {
            MouseMove = MouseMoveDefault;
            PointerUp = new List<Action>( );
            KeyUp = new List<(KeyCode key, Action action)>( );
        }

        public  static void Clear( )
        {
             
            PointerUp = new List<Action>( );
            KeyUp = new List<(KeyCode key, Action action)>( );
        }

        public  static void Stop( )
        {
            MouseMove = null;
            PointerUp = null;
            KeyUp = null;
        }


        private static void MouseMoveDefault( Event evt )
        {
            Mouse.CalcFreePosition( evt.mousePosition );

            Mouse.CalcSnappedPosition( );

            MouseHelpbarBoundariesCheck( evt );
        }

        private static void MouseAdditionalCalc( Event evt )
        {
            Mouse.CalcSnappedPosition( );

            MouseHelpbarBoundariesCheck( evt );
        }

        private static void MouseHelpbarBoundariesCheck( Event evt )
        {
            var (pos, size) = InputGUI.GetHelpbarSize( );

            var minX = pos.x;
            var maxX = pos.x + size.x;
            var minY = pos.y;
            var maxY = pos.y + size.y;

            if ( evt.mousePosition.x >= minX && evt.mousePosition.x <= maxX &&
                 evt.mousePosition.y >= minY && evt.mousePosition.y <= maxY )
                Mouse.IsMouseInScene = false;
            else
                Mouse.IsMouseInScene = true;
        }


        public  static void AddPointerUp( Action action )
        {
            PointerUp.Add( action );
        }

        public  static void AddKeyUp( KeyCode key, Action action )
        {
            KeyUp.Add( ( key, action ) );
        }


        public  static void ExecuteIMGUI( Event evt )
        {
            if ( evt.type == EventType.MouseMove || SceneData.IsMouseRecalcNeeded )
            {
                MouseMoveDefault( evt );
                SceneData.IsMouseRecalcNeeded = false;
            }

            if ( !Mouse.IsMouseInScene )
                return;

            if ( evt.type == EventType.MouseUp && evt.button == 0 )
                ExecutePointerUp( );

            if ( evt.type == EventType.KeyUp )
                ExecuteKeyUpIMGUI( evt );
        }

        private static void ExecuteMouseMove( Event evt )
        {
            MouseMove?.Invoke( evt );
        }

        private static void ExecuteKeyUpIMGUI( Event evt )
        {
            if ( KeyUp == null || KeyUp.Count == 0 ) return;
            if ( evt.modifiers != EventModifiers.None )
                return;
            
            for ( var i = 0; i < KeyUp.Count; i++ )
            {
                var curKey = KeyUp[ i ].key;

                if ( evt.keyCode == curKey )
                {
                    KeyUp[ i ].action.Invoke( );

                    if ( curKey == KeyCode.Escape )
                        return;
                }
            }
        }

        private static void ExecutePointerUp( )
        {
            if ( PointerUp == null || PointerUp.Count == 0 )
                return;

            for ( var i = 0; i < PointerUp.Count; i++ ) PointerUp[ i ].Invoke( );
        }
    }
}

#endif