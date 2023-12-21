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
    public  class WallModuleEditorWindow : ImporterWindow
    {
        public  WallGroup modularGroup;
        public  WallModule module;
        public  List<WallModule> tempGroupModules;
        private Button _cancelButton;
        private ObjectField _defaultPrefabObjectField;
        private ObjectField _extendedPrefabObjectField;
        private Label _infoLabel;


        private ScrollView _mbsContainer;
        private TextField _moduleNameTextfield;
        private float _prevHeight;
        private Button _saveButton;

        private void Update( )
        {
            if ( _mbsContainer.contentContainer.resolvedStyle.height != _prevHeight )
            {
                SetMinSize( 410, _mbsContainer.contentContainer.resolvedStyle.height + 30 );
                SetMaxSize( 410, _mbsContainer.contentContainer.resolvedStyle.height + 30 );
                SetMinSize( 410, 180 );
                _prevHeight = _mbsContainer.contentContainer.resolvedStyle.height;
            }
        }


         
        private void CreateGUI( )
        {
            if ( !HasOpenInstances<WallGroupEditorWindow>( ) )
            {
                Close( );
            }
            else
            {
                var mpw = GetWindow<WallGroupEditorWindow>( );
                if ( !mpw.IsOpenningModuleWindow )
                    Close( );
            }

            SetTitle( Texts.AssetSystem.Module.WALL_MODULE_EDITOR_WINDOW_NAME );
            SetMinSize( 410, 185 );

            ImportVTAs( );
            QueryVisualElements( );
            ConfigureObjectFields( );
            ConfigureInfoLabel( );
            ConfigureSaveButton( );
            ConfigureCancelButton( );
        }


        private void ImportVTAs( )
        {
            var windowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_WallModuleEditorWindow( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _mbsContainer = rootVisualElement.Q<ScrollView>( className: "mbs-container" );


            _moduleNameTextfield = rootVisualElement.Q<TextField>( "module-name-textfield" );
            _defaultPrefabObjectField = rootVisualElement.Q<ObjectField>( "default-prefab-objectfield" );
            _extendedPrefabObjectField = rootVisualElement.Q<ObjectField>( "extended-prefab-objectfield" );
            _infoLabel = rootVisualElement.Q<Label>( "info-label" );
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

            _extendedPrefabObjectField.RegisterValueChangedCallback( c =>
            {
                if ( c.newValue == null )
                    return;

                if ( !GameObjectHelper.IsPrefab( (GameObject)c.newValue ) )
                {
                    Debug.LogWarning( MessageController.MODULE_ObjectNotPrefab(
                                          titleContent.text,
                                          _moduleNameTextfield.text,
                                          _extendedPrefabObjectField.label ) );
                    _extendedPrefabObjectField.SetValueWithoutNotify( c.previousValue );
                }
            } );
        }

        private void ConfigureInfoLabel( )
        {
            _infoLabel.text = MessageController.MODULE_Wall_Info( );
        }

        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                var trimmedName = Text_Helper.TrimAndRemoveDoubleSpaces( _moduleNameTextfield.value );
                var name = ObjectNames.GetUniqueName(
                    tempGroupModules.Select( i => i.Name ).ToArray( ), trimmedName );

                var defaultPrefab = (GameObject)_defaultPrefabObjectField.value;
                var extendedPrefab = (GameObject)_extendedPrefabObjectField.value;

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
                                          name,
                                          _defaultPrefabObjectField.label ) );

                module.Name = name;
                module.DefaultPrefab = defaultPrefab;
                module.ExtendedPrefab = extendedPrefab;

                if ( HasOpenInstances<WallGroupEditorWindow>( ) )
                    GetWindow<WallGroupEditorWindow>( ).UpdateElements( );

                Close( );
            };
        }

        private void ConfigureCancelButton( )
        {
            _cancelButton.clicked += Close;
        }


        public  void SetupData( WallModule module, List<WallModule> tempGroupModules )
        {
            this.module = module;
            this.tempGroupModules = new List<WallModule>( tempGroupModules );
            this.tempGroupModules.Remove( module );

            _moduleNameTextfield.value = this.module.Name;
            _defaultPrefabObjectField.value = this.module.DefaultPrefab;
            _extendedPrefabObjectField.value = this.module.ExtendedPrefab;
        }
    }
}

#endif