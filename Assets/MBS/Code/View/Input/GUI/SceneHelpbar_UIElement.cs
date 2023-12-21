#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Controller.Builder;
using MBS.Controller.Configuration;
using MBS.Model.Configuration;
using MBS.View.Builder;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.Input.GUI
{
    public class SceneHelpbarUIElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SceneHelpbarToggleUIElement, UxmlTraits>
        {
        }

        private const string HELPBAR_CLASS_NAME = "mbs-helpbar";
        private const string HELPBAR_ITEM_CLASS_NAME = "mbs-helpbar-item";
        private const string HELPBAR_ITEM_TOGGLE_CLASS_NAME = "mbs-helpbar-toggle";
        private const string HELPBAR_ITEM_DROPDOWN_CLASS_NAME = "mbs-helpbar-dropdown";
        private const string HELPBAR_ITEM_SPACER_CLASS_NAME = "mbs-helpbar-spacer";

        public SceneHelpbarUIElement( )
        {
            AddToClassList( HELPBAR_CLASS_NAME );

            var styles =
                AssetDatabase.LoadAssetAtPath<StyleSheet>( PathController.GetPATH_SceneHelpbarStylesheet( ) );

            if ( styles != null ) styleSheets.Add( styles );

            RegisterCallback<GeometryChangedEvent>( gevt =>
            {
                if ( parent == null )
                    return;

                style.top = parent.resolvedStyle.height - resolvedStyle.height;
                style.left = 0;
            } );
        }

        public void AddMultiElement( bool firstActionAsMainAction, params UIElement[ ] uiElements )
        {
            var element = CreateMultiElement( firstActionAsMainAction, uiElements );
            Add( element );
        }

        private VisualElement CreateMultiElement( bool firstActionAsMainAction, params UIElement[ ] elements )
        {
            string firstLabel = elements[ 0 ].Label;
            if ( string.IsNullOrEmpty( firstLabel ) )
            {
                Debug.LogError(
                    "MBS. Helpbar. Label of the first element is null or empty. Cannot create an element." );
                return null;
            }

            var root = CreateRoot( );
            var firstKeyAction = elements.FirstOrDefault( i => i.KeyAction != null )?.KeyAction;

            if ( firstActionAsMainAction )
                if ( firstKeyAction != null )
                    root.clicked += firstKeyAction;

            for ( var i = 0; i < elements.Length; i++ )
            {
                var index = i;
                var curElementData = elements[ i ];

                if ( i > 0 )
                {
                    var spacer = new VisualElement( );
                    spacer.AddToClassList( HELPBAR_ITEM_SPACER_CLASS_NAME );
                    root.Add( spacer );
                }

                switch ( curElementData.type )
                {
                    case UIElement.ElementType.Label:
                        var labelData = (UIElement)curElementData;
                        var label = CreateLabel( labelData );
                        if ( label != null )
                            root.Add( label );
                        break;

                    case UIElement.ElementType.Toggle:
                        var castToggleData = (UIToggleElementData)elements[ index ];

                        var toggleLabel = CreateLabel( castToggleData );
                        if ( toggleLabel != null )
                            root.Add( toggleLabel );

                        var toggle = CreateToggle( castToggleData );
                        if ( toggle != null )
                            root.Add( toggle );
                        break;

                    case UIElement.ElementType.Dropdown:
                        var castDropdownData = (UIDropdownElement)elements[ index ];

                        var dropdownLabel = CreateLabel( castDropdownData );
                        if ( dropdownLabel != null )
                            root.Add( dropdownLabel );

                        var dropdown = CreateDropdown( castDropdownData );
                        if ( dropdown != null )
                            root.Add( dropdown );
                        break;

                    case UIElement.ElementType.FloatInput:
                        var castFloatInputData = (UIFloatInputData)elements[ index ];

                        var floatInputLabel = CreateLabel( castFloatInputData );
                        if ( floatInputLabel != null )
                            root.Add( floatInputLabel );

                        var floatInput = CreateFloatInput( castFloatInputData );
                        if ( floatInput != null )
                            root.Add( floatInput );
                        break;

                    case UIElement.ElementType.IntegerInput:
                        var castIntInputData = (UIIntegerInputData)elements[ index ];

                        var intInputLabel = CreateLabel( castIntInputData );
                        if ( intInputLabel != null )
                            root.Add( intInputLabel );

                        var intInput = CreateIntegerInput( castIntInputData );
                        if ( intInput != null )
                            root.Add( intInput );
                        break;
                }
            }

            return root;
        }

        private Button CreateRoot( )
        {
            var item = new Button( );
            item.AddToClassList( HELPBAR_ITEM_CLASS_NAME );
            return item;
        }

        private VisualElement CreateLabel( UIElement elementData )
        {
            if ( string.IsNullOrEmpty( elementData.Label ) )
                return null;

            var label = new Label( elementData.Label );
            return label;
        }

        private VisualElement CreateToggle( UIToggleElementData toggleData )
        {
            var toggle = new SceneHelpbarToggleUIElement( toggleData.OnText, toggleData.OffText );

            toggle.RegisterValueChangedCallback( e => toggleData.OnValueChangeAction( e.newValue ) );

            toggle.schedule.Execute( ( ) =>
            {
                var observeValue = toggleData.GetValueRemote( );
                if ( observeValue != toggle.value )
                {
                    toggle.SetValueWithoutNotify( observeValue );
                    toggle.OnValueChangeCallback( observeValue );
                }
            } ).Every( Config.Sgt.SceneValueCheckEveryMS );

            toggle.AddToClassList( HELPBAR_ITEM_TOGGLE_CLASS_NAME );

            return toggle;
        }

        private VisualElement CreateDropdown( UIDropdownElement dropdownElement )
        {
            var dropdown = new DropdownField( "", dropdownElement.Choices, dropdownElement.GetValueRemote( ) );

            dropdown.AddToClassList( HELPBAR_ITEM_DROPDOWN_CLASS_NAME );

            var largerString = dropdownElement.Choices.OrderByDescending( s => s.Length ).First( );
            var size = largerString.Length * 9 + 12;
            dropdown.style.width = new StyleLength( size );

            dropdown.RegisterValueChangedCallback( e => dropdownElement.OnValueChangeAction( e.newValue ) );
            dropdown.schedule.Execute( ( ) =>
            {
                var observedValue = dropdownElement.GetValueRemote( );

                if ( !string.IsNullOrEmpty( observedValue ) )
                    dropdown.SetValueWithoutNotify( observedValue );
            } ).Every( Config.Sgt.SceneValueCheckEveryMS );

            return dropdown;
        }

        private VisualElement CreateFloatInput( UIFloatInputData floatInputData )
        {
            var floatField = new FloatField( 5 );
            floatField.style.width = 5 * 9;
            floatField.value = floatInputData.GetValueRemote( );
             


            if ( floatInputData.Disabled == true )
                floatField.SetEnabled( false );


            if ( floatInputData.AddValuePointer )
            {
                var parentElement = new VisualElement( );
                parentElement.style.flexDirection = new StyleEnum<FlexDirection>( FlexDirection.Row );
                parentElement.style.alignItems = new StyleEnum<Align>( Align.FlexStart );

                var pickerBlock = CreateFloatFieldAdjBlock( );

                parentElement.Add( floatField );
                parentElement.Add( pickerBlock );

                var objectLinkSizeToggle = pickerBlock.Q<ToolbarToggle>( "object-size-link-toggle" );
                var positionPointerToggle = pickerBlock.Q<ToolbarToggle>( "position-pointer-toggle" );
                var objectPickerToggle = pickerBlock.Q<ToolbarToggle>( "object-picker-toggle" );


                floatField.RegisterValueChangedCallback( ( e ) =>
                {
                    if ( objectLinkSizeToggle != null && objectLinkSizeToggle.value == true )
                        return;

                    floatInputData.OnValueChangeAction( e.newValue );
                } );

                floatField.schedule.Execute( ( ) =>
                {
                    if ( floatField.focusController.focusedElement != floatField )
                    {
                        if ( objectLinkSizeToggle != null && objectLinkSizeToggle.value == true )
                            floatField.SetValueWithoutNotify( floatInputData.GetLinkedValue( ) );
                        else
                            floatField.SetValueWithoutNotify( floatInputData.GetValueRemote( ) );
                    }
                } ).Every( Config.Sgt.SceneValueCheckEveryMS );


                objectLinkSizeToggle.RegisterValueChangedCallback( ( e ) =>
                {
                    if ( e.newValue == true )
                    {
                         
                        floatInputData.OnValueChangeAction( floatInputData.GetLinkedValue( ) );
                    }

                    floatField.SetEnabled( !e.newValue );
                } );


                positionPointerToggle.RegisterValueChangedCallback( ( e ) =>
                {
                    positionPointerToggle.SetValueWithoutNotify( true );

                    Builder_Controller.ChangeSceneMode( SceneMode.Pointer );
                    Pointer_Mode.Setup_Inputs( ( value ) =>
                    {
                        if ( objectLinkSizeToggle.value == true )
                            return;

                        floatInputData.OnValueChangeAction( value );
                    } );
                } );

                objectPickerToggle.RegisterValueChangedCallback( ( e ) =>
                {
                    objectPickerToggle.SetValueWithoutNotify( true );

                    Builder_Controller.ChangeSceneMode( SceneMode.Picker );

                    Picker_Mode.SetParameter( LeadAxis.YAxis );
                    Picker_Mode.Setup_Inputs( ( value ) =>
                    {
                        if ( objectLinkSizeToggle.value == true )
                            return;

                        floatInputData.OnValueChangeAction( value );
                    } );
                } );

                return parentElement;
            }
            
            floatField.RegisterValueChangedCallback( ( e ) => floatInputData.OnValueChangeAction( e.newValue ) );

            floatField.schedule.Execute( ( ) =>
            {
                if ( floatField.focusController.focusedElement != floatField )
                {
                    floatField.SetValueWithoutNotify( floatInputData.GetValueRemote( ) );
                }
            } ).Every( Config.Sgt.SceneValueCheckEveryMS );

            return floatField;
        }

        private VisualElement CreateIntegerInput( UIIntegerInputData intInputData )
        {
            var intField = new IntegerField( 3 );
            intField.style.width = 3 * 9;
            intField.value = intInputData.GetValueRemote( );
            intField.RegisterValueChangedCallback( e => intInputData.OnValueChangeAction( e.newValue ) );

            intField.schedule.Execute( ( ) =>
            {
                if ( intField.focusController.focusedElement != intField )
                {
                    intField.SetValueWithoutNotify( intInputData.GetValueRemote( ) );
                }
            } ).Every( Config.Sgt.SceneValueCheckEveryMS );

            return intField;
        }

        private VisualElement CreateFloatFieldAdjBlock( )
        {
            VisualTreeAsset pickerBlockVTA;

            if ( EditorGUIUtility.isProSkin )
                pickerBlockVTA = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathsManager.Singleton.InternalDataPath +
                    PredefinedPaths.HELPBAR_PICKER_BLOCK_DARK );
            else
                pickerBlockVTA = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathsManager.Singleton.InternalDataPath +
                    PredefinedPaths.HELPBAR_PICKER_BLOCK_LIGHT );

            if ( pickerBlockVTA == null )
            {
                Debug.LogError( "MBS. Scene Helpbar. Cannot find helpbar picker block uxml file." );
                return null;
            }

            var pickerBlock = pickerBlockVTA.Instantiate( );

            return pickerBlock;
        }
    }
}
#endif