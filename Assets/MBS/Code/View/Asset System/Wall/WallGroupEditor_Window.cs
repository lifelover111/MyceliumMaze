#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Configuration;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.AssetSystem
{
    public  class WallGroupEditorWindow : ImporterWindow
    {
        public  WallGroup modularGroup;
        public  List<WallGroup> tempWallGroups;

        public  List<WallModule> modulesList;
        private ListView _assetsListView;
        private Button _cancelButton;

        private TextField _groupNameTextField;
        private VisualTreeAsset _listItemVta;
        private Button _saveButton;

        public  bool IsOpenningModuleWindow { get; private set; }

        private void OnDisable( )
        {
            if ( HasOpenInstances<WallModuleEditorWindow>( ) ) GetWindow<WallModuleEditorWindow>( ).Close( );
        }


        private void OnDestroy( )
        {
            if ( HasOpenInstances<WallModuleEditorWindow>( ) ) GetWindow<WallModuleEditorWindow>( ).Close( );
        }


        private void CreateGUI( )
        {
            if ( !HasOpenInstances<ModularPackManager_Window>( ) )
            {
                Close( );
            }
            else
            {
                var mpw = GetWindow<ModularPackManager_Window>( );
                if ( !mpw.IsOpenningGroupWindow )
                    Close( );
            }

            SetTitle( Texts.AssetSystem.Group.WALL_GROUP_EDITOR_WINDOW_NAME );
            SetMinSize( 480, 220 );

            ImportVTAs( );
            QueryVisualElements( );

            ConfigureAssetsListView( );
            ConfigureSaveButton( );
            ConfigureCancelButton( );
        }


        private void ImportVTAs( )
        {
            var windowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_WallGroupEditorWindow( ) );
            _listItemVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_ListItemWithIcon( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _groupNameTextField = rootVisualElement.Q<TextField>( "modular-group-name-textfield" );
            _assetsListView = rootVisualElement.Q<ListView>( "modules-listview" );
            _saveButton = rootVisualElement.Q<Button>( "save-button" );
            _cancelButton = rootVisualElement.Q<Button>( "cancel-button" );
        }

        private void ConfigureAssetsListView( )
        {
            _assetsListView.makeItem += ( ) => _listItemVta.Instantiate( );
            _assetsListView.itemsAdded += e => { modulesList[ e.First( ) ] = new WallModule( ); };

            _assetsListView.bindItem += ( visualElement, index ) =>
            {
                var label = visualElement.Q<Label>( );
                if ( string.IsNullOrEmpty( modulesList[ index ].Name ) )
                {
                    var newName = Texts.AssetSystem.Creation.NEW_WALL_MODULE;
                    var uniqueName = ObjectNames.GetUniqueName(
                        modulesList.Select( i => i.Name ).ToArray( ), newName );
                    modulesList[ index ].Name = uniqueName;
                }

                label.text = modulesList[ index ].Name;

                Texture texture = Module.GetPreviewOrEmptyIcon( modulesList[ index ] );
                var image = visualElement.Q<MBSImageVisualElement>( );
                image.image = texture;
            };

#if UNITY_2022_3_OR_NEWER
            _assetsListView.itemsChosen += e =>
            {
                IsOpenningModuleWindow = true;
                var window = GetWindow<WallModuleEditorWindow>( );
                window.SetupData( modulesList[ _assetsListView.selectedIndex ], modulesList );
                IsOpenningModuleWindow = false;
            };
#else
            _assetsListView.onItemsChosen += e =>
            {
                IsOpenningModuleWindow = true;
                var window = GetWindow<WallModuleEditorWindow>( );
                window.SetupData( modulesList[ _assetsListView.selectedIndex ], modulesList );
                IsOpenningModuleWindow = false;
            };
#endif
            

        }

        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                 
                var trimmedName = Text_Helper.TrimAndRemoveDoubleSpaces( _groupNameTextField.value );
                var name = ObjectNames.GetUniqueName(
                    tempWallGroups.Select( i => i.Name ).ToArray( ),
                    trimmedName );


                if ( !Text_Helper.IsTextLengthOk( name ) )
                {
                    Debug.LogError( MessageController.TextValidation_LengthLess2( _groupNameTextField.label ) );
                    return;
                }

                if ( !Text_Helper.IsTextContainsOnlyLettersAndDigits( name ) )
                {
                    Debug.LogError(
                        MessageController.TextValidation_InapproppriateCharacters( _groupNameTextField.label ) );
                    return;
                }

                modularGroup.Name = name;
                modularGroup.Modules = modulesList.ToArray( );

                if ( modulesList.Count == 0 )
                    Debug.LogWarning( MessageController.GROUP_ModulesListEmpty( titleContent.text, name ) );

                if ( HasOpenInstances<ModularPackManager_Window>( ) )
                {
                    var window = GetWindow<ModularPackManager_Window>( );
                    window.RefreshWindow( );
                }

                Close( );
            };
        }

        private void ConfigureCancelButton( )
        {
            _cancelButton.clicked += Close;
        }


        public  void UpdateElements( )
        {
            _assetsListView.Rebuild( );
        }


        public  void SetupData( WallGroup wallGroup, List<WallGroup> tempWallGroups )
        {
            modularGroup = wallGroup;
            this.tempWallGroups = new List<WallGroup>( tempWallGroups );
            this.tempWallGroups.Remove( modularGroup );

            _groupNameTextField.value = wallGroup.Name;

            modulesList = modularGroup.Modules.ToList( );
            _assetsListView.itemsSource = modulesList;
        }
    }
}

#endif