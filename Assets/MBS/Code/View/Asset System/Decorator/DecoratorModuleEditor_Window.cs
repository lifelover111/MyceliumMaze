#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Configuration;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.AssetSystem
{
    public  class PlacerModuleEditorWindow : ImporterWindow
    {
        public  DecoratorModule module;
        public  List<DecoratorModule> tempPlacerModules;
        private Button _cancelButton;
        private ObjectField _defaultPrefabObjectField;
        private TextField _moduleNameTextfield;
        private Button _saveButton;


         
        private void CreateGUI( )
        {
            if ( !HasOpenInstances<DecoratorGroupEditorWindow>( ) )
            {
                Close( );
            }
            else
            {
                var mpw = GetWindow<DecoratorGroupEditorWindow>( );
                if ( !mpw.IsOpenningModuleWindow )
                    Close( );
            }

            SetTitle( Texts.AssetSystem.Module.DECOR_MODULE_EDITOR_WINDOW_NAME );
            SetMinSize( 410, 115 );
            SetMaxSize( 410, 115 );

            ImportVTAs( );
            QueryVisualElements( );
            ConfigureObjectFields( );
            ConfigureSaveButton( );
            ConfigureCancelButton( );
        }

        private void ImportVTAs( )
        {
            var windowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_PlacerModuleEditorWindow( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _moduleNameTextfield = rootVisualElement.Q<TextField>( "module-name-textfield" );
            _defaultPrefabObjectField = rootVisualElement.Q<ObjectField>( "default-prefab-objectfield" );
            _saveButton = rootVisualElement.Q<Button>( "save-button" );
            _cancelButton = rootVisualElement.Q<Button>( "cancel-button" );
        }

        private void ConfigureObjectFields( )
        {
            _defaultPrefabObjectField.RegisterValueChangedCallback( c =>
            {
                if ( c.newValue == null )
                    return;

                if ( !GameObjectHelper.IsPrefab( (GameObject)c.newValue ) )
                {
                    Debug.LogWarning( MessageController.MODULE_ObjectNotPrefab(
                                          titleContent.text,
                                          _moduleNameTextfield.text,
                                          _defaultPrefabObjectField.label ) );
                    _defaultPrefabObjectField.SetValueWithoutNotify( c.previousValue );
                }
            } );
        }

        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                var trimmedName = Text_Helper.TrimAndRemoveDoubleSpaces( _moduleNameTextfield.value );
                var name = ObjectNames.GetUniqueName(
                    tempPlacerModules.Select( i => i.Name ).ToArray( ),
                    trimmedName );

                var defaultPrefab = (GameObject)_defaultPrefabObjectField.value;

                if ( !Text_Helper.IsTextLengthOk( name ) )
                {
                    Debug.LogError( MessageController.TextValidation_LengthLess2( _moduleNameTextfield.label ) );
                    return;
                }

                if ( !Text_Helper.IsTextContainsOnlyLettersAndDigits( name ) )
                {
                    Debug.LogError(
                        MessageController.TextValidation_InapproppriateCharacters( _moduleNameTextfield.label ) );
                    return;
                }


                if ( defaultPrefab == null )
                    Debug.LogWarning( MessageController.MODULE_ONSAVE_FieldValueIsNull(
                                          titleContent.text,
                                          "On Save Validation ( " + name + " )",
                                          _defaultPrefabObjectField.label ) );

                module.Name = name;
                module.DefaultPrefab = defaultPrefab;

                if ( HasOpenInstances<DecoratorGroupEditorWindow>( ) )
                    GetWindow<DecoratorGroupEditorWindow>( ).UpdateElements( );

                Close( );
            };
        }

        private void ConfigureCancelButton( )
        {
            _cancelButton.clicked += ( ) => { Close( ); };
        }

        public  void SetupData( DecoratorModule module, List<DecoratorModule> placerModules )
        {
            this.module = module;
            tempPlacerModules = new List<DecoratorModule>( placerModules );
            tempPlacerModules.Remove( module );

            _moduleNameTextfield.value = this.module.Name;
            _defaultPrefabObjectField.value = this.module.DefaultPrefab;
        }
    }
}

#endif