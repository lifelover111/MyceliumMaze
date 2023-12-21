#if UNITY_EDITOR

using UnityEngine.UIElements;

namespace MBS.View.Input.GUI
{
    public  class SceneHelpbarToggleUIElement : Toggle
    {
        public  new class UxmlFactory : UxmlFactory<SceneHelpbarToggleUIElement, UxmlTraits> { }

        
        private readonly string[ ] _toggleTexts;

        public  SceneHelpbarToggleUIElement( )
        {
            label = "";

            _toggleTexts = new[ ] { "On", "Off" };

            OnValueChangeCallback( base.value );

            this.RegisterValueChangedCallback( e => OnValueChangeCallback( e.newValue ) );
        }

        public  SceneHelpbarToggleUIElement( string onText = null, string offText = null )
        {
            if ( !string.IsNullOrEmpty( onText ) && !string.IsNullOrEmpty( offText ) )
                _toggleTexts = new[ ] { onText, offText };
            else _toggleTexts = new[ ] { "On", "Off" };

            label = "";

            OnValueChangeCallback( base.value );

            this.RegisterValueChangedCallback( e => OnValueChangeCallback( e.newValue ) );
        }
        
        public  void OnValueChangeCallback( bool newValue )
        {
            if ( newValue )
                text = _toggleTexts[ 0 ];
            else
                text = _toggleTexts[ 1 ];
        }
    }
}

#endif