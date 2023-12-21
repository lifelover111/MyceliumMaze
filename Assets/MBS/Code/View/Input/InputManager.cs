#if UNITY_EDITOR

using MBS.View.Input.GUI;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Input
{
    public  static class InputManager
    {
        public  static void Start( )
        {
            InputPhysical.Start( );
            InputGUI.Start( );
        }

        public  static void DuringSceneGUI( SceneView newSceneView )
        {
            InputGUI.AttachHelpbarToScene( newSceneView );
        }

        public  static void Stop( )
        {
            InputPhysical.Stop( );
            InputGUI.Stop( );
        }


        public  static void ExecuteIMGUI( Event evt )
        {
            InputPhysical.ExecuteIMGUI( evt );
        }

        public  static void ClearInputData( )
        {
            InputPhysical.Clear( );
            InputGUI.Clear( );
        }

        
        public  static void AddMouseElement( UIElement uiElement )
        {
            InputPhysical.AddPointerUp( uiElement.KeyAction );
            InputGUI.AddMouseElement( uiElement );
        }
        
        public  static void AddKeyElement( bool firstActionAsMainAction = true, params UIElement[ ] uiElements )
        {
            for ( var i = 0; i < uiElements.Length; i++ )
            {
                var curElement = uiElements[ i ];
                if ( curElement.Key != default )
                {
                    InputPhysical.AddKeyUp( curElement.Key, curElement.KeyAction );
                }
            }
            InputGUI.AddKeyElement( firstActionAsMainAction, uiElements );
        }
    }
}

#endif