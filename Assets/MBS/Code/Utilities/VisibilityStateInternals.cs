using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MBS.MBS.Code.Utilities
{
#if UNITY_EDITOR
    public static class VisibilityStateInternals
    {
        #region reflection

        private static Type[ ] _refEditorTypes;
        private static Type _refSceneVisibilityStateType;
        private static System.Reflection.MethodInfo _refSceneVisibilityState_GetInstanceMethod;
        private static System.Reflection.MethodInfo _refSceneVisibilityState_SetGameObjectPickingDisabled;


        private static void BuildReflectionCache( )
        {
            try
            {
                if ( _refEditorTypes != null )
                    return;

                _refEditorTypes = typeof( Editor ).Assembly.GetTypes( );

                if ( _refEditorTypes.Length > 0 )
                {
                    _refSceneVisibilityStateType =
                        _refEditorTypes.FirstOrDefault( t => t.Name == "SceneVisibilityState" );

                    if ( _refSceneVisibilityStateType != null )
                    {
                        _refSceneVisibilityState_GetInstanceMethod = _refSceneVisibilityStateType.GetMethod(
                            "GetInstance",
                            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic );
                        
                        _refSceneVisibilityState_SetGameObjectPickingDisabled = _refSceneVisibilityStateType.GetMethod(
                            "SetGameObjectPickingDisabled",
                            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic );
                    }
                }
            }
            catch ( Exception )
            {
                 
            }
        }

        private static UnityEngine.Object GetSceneVisibilityStateViaReflection( )
        {
            try
            {
                BuildReflectionCache( );
                return (UnityEngine.Object)_refSceneVisibilityState_GetInstanceMethod.Invoke( null, new object[ ] { } );
            }
            catch ( Exception )
            {
                return null;
            }
        }

        public static bool SetPickingNoUndo( GameObject gameObject, bool pickingEnabled, bool includeChildren )
        {
            try
            {
                BuildReflectionCache( );
                var state = GetSceneVisibilityStateViaReflection( );
                if ( state != null )
                {
                    pickingEnabled = !pickingEnabled;
                    _refSceneVisibilityState_SetGameObjectPickingDisabled.Invoke(
                        state, new object[ ] { gameObject, pickingEnabled, includeChildren } );
                    return true;
                }

                return false;
            }
            catch ( Exception )
            {
                return false;
            }
        }
        
        #endregion
    }
#endif
}