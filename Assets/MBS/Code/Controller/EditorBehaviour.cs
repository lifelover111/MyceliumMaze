#if UNITY_EDITOR
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  abstract class EditorBehaviour : MonoBehaviour
    {
        private void OnDisable( )
        {
            if ( Event.current != null )
                if ( Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete" )
                {
                    OnExternalDestroy( );
                    return;
                }

            OnExternalDisable( );
        }

        private void OnDestroy( )
        {
            if ( Event.current != null )
                if ( Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete" )
                {
                    OnExternalDestroy( );
                    return;
                }

            OnOriginalDestroy( );
        }

         
        protected virtual void OnOriginalDestroy( )
        {
        }

         
        protected virtual void OnExternalDestroy( )
        {
        }


        protected virtual void OnNaturalDisable( )
        {
        }

        protected virtual void OnExternalDisable( )
        {
        }
    }
}
#endif