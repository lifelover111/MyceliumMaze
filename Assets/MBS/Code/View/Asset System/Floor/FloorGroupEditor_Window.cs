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
    public  class FloorGroupEditorWindow : ImporterWindow
    {
        public  FloorGroup floorGroup;
        public  List<FloorGroup> tempFloorGroups;
        public  List<FloorModule> modulesList;
        private Button _cancelButton;

        private TextField _groupNameTextField;
        private VisualTreeAsset _listItemVta;
        private ListView _modulesListView;
        private Button _saveButton;

        public  bool IsOpenningModuleWindow { get; private set; }

        private void OnDisable( )
        {
            if ( HasOpenInstances<FloorModuleEditorWindow>( ) ) GetWindow<FloorModuleEditorWindow>( ).Close( );
        }

        private void OnDestroy( )
        {
            if ( HasOpenInstances<FloorModuleEditorWindow>( ) ) GetWindow<FloorModuleEditorWindow>( ).Close( );
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

            SetTitle( Texts.AssetSystem.Group.FLOOR_GROUP_EDITOR_WINDOW_NAME );
            SetMinSize( 480, 220 );

            ImportVTAs( );
            QueryVisualElements( );

             
            ConfigureAssetsListView( );
            ConfigureSaveButton( );
            ConfigureCancelButton( );
        }

        public  override void SaveChanges( )
        {
            base.SaveChanges( );
        }

        private void ImportVTAs( )
        {
            var windowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_FloorGroupEditorWindow( ) );
            _listItemVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_ListItemWithIcon( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _groupNameTextField = rootVisualElement.Q<TextField>( "modular-group-name-textfield" );
            _modulesListView = rootVisualElement.Q<ListView>( "modules-listview" );
            _saveButton = rootVisualElement.Q<Button>( "save-button" );
            _cancelButton = rootVisualElement.Q<Button>( "cancel-button" );
        }


        private void ConfigureAssetsListView( )
        {
            _modulesListView.makeItem += ( ) => _listItemVta.Instantiate( );
            _modulesListView.itemsAdded += e => { modulesList[ e.First( ) ] = new FloorModule( ); };

            _modulesListView.bindItem += ( visualElement, index ) =>
            {
                var label = visualElement.Q<Label>( );
                if ( string.IsNullOrEmpty( modulesList[ index ].Name ) )
                {
                    var newName = Texts.AssetSystem.Creation.NEW_FLOOR_MODULE;
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
            _modulesListView.itemsChosen += e =>
            {
                IsOpenningModuleWindow = true;
                var window = GetWindow<FloorModuleEditorWindow>( );
                window.SetupData( modulesList[ _modulesListView.selectedIndex ], modulesList );
                IsOpenningModuleWindow = false;
            };
#else
            _modulesListView.onItemsChosen += e =>
            {
                IsOpenningModuleWindow = true;
                var window = GetWindow<FloorModuleEditorWindow>( );
                window.SetupData( modulesList[ _modulesListView.selectedIndex ], modulesList );
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
                    tempFloorGroups.Select( i => i.Name ).ToArray( ),
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

                floorGroup.Name = name;
                floorGroup.Modules = modulesList.ToArray( );

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
            _modulesListView.Rebuild( );
        }

        public  void SetupData( FloorGroup floorGroup, List<FloorGroup> floorGroups )
        {
            this.floorGroup = floorGroup;
            tempFloorGroups = new List<FloorGroup>( floorGroups );
            tempFloorGroups.Remove( this.floorGroup );

            _groupNameTextField.value = floorGroup.Name;

            modulesList = this.floorGroup.Modules.ToList( );
            _modulesListView.itemsSource = modulesList;
        }
    }
}

#endif