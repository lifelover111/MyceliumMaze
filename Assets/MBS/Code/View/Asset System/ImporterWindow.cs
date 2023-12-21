#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.AssetSystem
{
    public  class ImporterWindow : EditorWindow
    {
        protected void SetTitle( string title )
        {
            titleContent = new GUIContent( title );
        }

        protected void SetMinSize( float width, float height )
        {
            minSize = new Vector2( width, height );
        }

        protected void SetMaxSize( float width, float height )
        {
            maxSize = new Vector2( width, height );
        }

        protected void GetMbsContainer( )
        {
            var v = rootVisualElement.Q<VisualElement>( className: "mbs-container" );
        }
    }
}

#endif