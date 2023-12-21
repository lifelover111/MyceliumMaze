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
    public  class FloorModuleEditorWindow : ImporterWindow
    {
        public  FloorModule module;
        public  List<FloorModule> floorModules;
        private Button _cancelButton;
        private Label _infoLabel;

        private ScrollView _mbsContainer;
        private TextField _moduleNameTextfield;
        private float _prevHeight;
        private Button _saveButton;
        private ObjectField _squarePrefabObjectField;
        private ObjectField _triangularPrefabObjectField;


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
            if ( !HasOpenInstances<FloorGroupEditorWindow>( ) )
            {
                Close( );
            }
            else
            {
                var mpw = GetWindow<FloorGroupEditorWindow>( );
                if ( !mpw.IsOpenningModuleWindow )
                    Close( );
            }

            SetTitle( Texts.AssetSystem.Module.FLOOR_MODULE_EDITOR_WINDOW_NAME );
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
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_FloorModuleEditorWindow( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _mbsContainer = rootVisualElement.Q<ScrollView>( className: "mbs-container" );

            _moduleNameTextfield = rootVisualElement.Q<TextField>( "module-name-textfield" );
            _squarePrefabObjectField = rootVisualElement.Q<ObjectField>( "square-prefab-objectfield" );
            _triangularPrefabObjectField = rootVisualElement.Q<ObjectField>( "triangular-prefab-objectfield" );
            _infoLabel = rootVisualElement.Q<Label>( "info-label" );
            _saveButton = rootVisualElement.Q<Button>( "save-button" );
            _cancelButton = rootVisualElement.Q<Button>( "cancel-button" );
        }

        private void ConfigureObjectFields( )
        {
            _squarePrefabObjectField.RegisterValueChangedCallback( c =>
            {
                if ( c.newValue == null )
                    return;

                if ( !GameObjectHelper.IsPrefab( (GameObject)c.newValue ) )
                {
                    Debug.LogWarning( MessageController.MODULE_ObjectNotPrefab(
                                          titleContent.text,
                                          _moduleNameTextfield.text,
                                          _squarePrefabObjectField.label ) );
                    _squarePrefabObjectField.SetValueWithoutNotify( c.previousValue );
                }
            } );

            _triangularPrefabObjectField.RegisterValueChangedCallback( c =>
            {
                if ( c.newValue == null )
                    return;

                if ( !GameObjectHelper.IsPrefab( (GameObject)c.newValue ) )
                {
                    Debug.LogWarning( MessageController.MODULE_ObjectNotPrefab(
                                          titleContent.text,
                                          _moduleNameTextfield.text,
                                          _triangularPrefabObjectField.label ) );
                    _triangularPrefabObjectField.SetValueWithoutNotify( c.previousValue );
                }
            } );
        }

        private void ConfigureInfoLabel( )
        {
            _infoLabel.text = MessageController.MODULE_Floor_Info( );
        }

        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                var trimmedName = Text_Helper.TrimAndRemoveDoubleSpaces( _moduleNameTextfield.value );
                var name = ObjectNames.GetUniqueName(
                    floorModules.Select( i => i.Name ).ToArray( ), trimmedName );

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


                var squarePrefab = (GameObject)_squarePrefabObjectField.value;
                var triangularPrefab = (GameObject)_triangularPrefabObjectField.value;

                if ( squarePrefab == null )
                    Debug.LogWarning( MessageController.MODULE_ONSAVE_FieldValueIsNull(
                                          titleContent.text,
                                          name,
                                          _squarePrefabObjectField.label
                                      ) );

                module.Name = name;
                module.SquarePrefab = squarePrefab;
                module.TriangularPrefab = triangularPrefab;

                if ( HasOpenInstances<FloorGroupEditorWindow>( ) )
                    GetWindow<FloorGroupEditorWindow>( ).UpdateElements( );

                Close( );
            };
        }

        private void ConfigureCancelButton( )
        {
            _cancelButton.clicked += ( ) => { Close( ); };
        }

        public  void SetupData( FloorModule module, List<FloorModule> floorModules )
        {
            this.module = module;
            this.floorModules = new List<FloorModule>( floorModules );
            this.floorModules.Remove( module );

            _moduleNameTextfield.value = this.module.Name;
            _squarePrefabObjectField.value = this.module.SquarePrefab;
            _triangularPrefabObjectField.value = this.module.TriangularPrefab;
        }
    }
}

#endif