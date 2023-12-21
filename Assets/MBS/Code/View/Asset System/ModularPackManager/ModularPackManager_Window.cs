#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Controller.AssetSystem;
using MBS.Controller.Configuration;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.AssetSystem
{
    public  class ModularPackManager_Window : ImporterWindow, IHasCustomMenu
    {
        private int _curIndex;

        private ImporterDataContainer _currentData;

        private bool _doShowHidden;
        private List<ImporterDataContainer> _iData;
        private List<string> _modularPacksNames;
        private List<ModularGroupCategory> _decoratorCategories;
        private List<WallGroup> _wallGroups;
        private List<FloorGroup> _floorGroups;
        private List<DecoratorGroup> _decoratorGroups;


        public  bool IsOpenningGroupWindow { get; private set; }

        private void OnDisable( )
        {
            if ( HasOpenInstances<WallGroupEditorWindow>( ) )
                GetWindow<WallGroupEditorWindow>( ).Close( );

            if ( HasOpenInstances<FloorGroupEditorWindow>( ) )
                GetWindow<FloorGroupEditorWindow>( ).Close( );

            if ( HasOpenInstances<DecoratorGroupEditorWindow>( ) )
                GetWindow<DecoratorGroupEditorWindow>( ).Close( );
        }

        private void OnDestroy( )
        {
            if ( HasOpenInstances<WallGroupEditorWindow>( ) ) GetWindow<WallGroupEditorWindow>( ).Close( );
        }


        public  void CreateGUI( )
        {
            SetTitle( Texts.AssetSystem.WINDOW_NAME );
            SetMinSize( 380, 340 );

            LoadVTAs( );
            QueryVisualElements( );

            ConfigurePackSelector( );
            ConfigureNewPackButton( );

            ConfigureWallGroupsList( );
            ConfigureFloorGroupsList( );
            ConfigureDecoratorGroupsList( );

            ConfigureRemovePackButton( );
            ConfigureHidePackButton( );

            ConfigureSaveButton( );
            ConfigureDiscardButton( );
            ConfigureCloseButton( );

            PathsManager.Start(  );
            AssetsImporterController.RefreshAssetsData( );
            SetupVElementsData( );

            IsOpenningGroupWindow = false;
            
            _packSelector.index = 0;
        }


        public  void AddItemsToMenu( GenericMenu menu )
        {
            var refreshItem = new GUIContent( Texts.AssetSystem.CONTEXT_MENU_REFRESH );
            var showHidden = new GUIContent( Texts.AssetSystem.CONTEXT_MENU_SHOW_HIDDEN );
            menu.AddItem( refreshItem, false, ReloadWindow );
            menu.AddItem( showHidden, _doShowHidden, ToggleShowHiddenPacks );
        }


        [MenuItem( Texts.MenuItems.MODULAR_PACK_MANAGER, false, 11 )]
        public  static void ShowWindow( )
        {
            var window = GetWindow<ModularPackManager_Window>( );
            window.Show( );
        }


        private void LoadVTAs( )
        {
            _mainWindowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_ModularPackManagerWindow( ) );
            _listItemVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_ListItemWithIcon( ) );

            rootVisualElement.Add( _mainWindowVta.Instantiate( ) );
        }

        private void QueryVisualElements( )
        {
            _packSelector = rootVisualElement.Q<DropdownField>( "pack-selector-dropdown" );
            _createPackButton = rootVisualElement.Q<Button>( "create-new-pack-button" );

            _propertiesLabel = rootVisualElement.Q<Label>( "pack-properties-label" );
            _nameTextField = rootVisualElement.Q<TextField>( "pack-name-textfield" );

            _wallListView = rootVisualElement.Q<ListView>( "wall-list" );
            _floorListView = rootVisualElement.Q<ListView>( "floor-list" );
            _decoratorListView = rootVisualElement.Q<ListView>( "placer-list" );

            _removePackButton = rootVisualElement.Q<Button>( "remove-pack-button" );
            _hidePackButton = rootVisualElement.Q<Button>( "hide-pack-button" );

            _saveButton = rootVisualElement.Q<Button>( "save-changes-button" );
            _discardButton = rootVisualElement.Q<Button>( "discard-changes-button" );
            _closeButton = rootVisualElement.Q<Button>( "close-button" );
        }


         
        private void ConfigurePackSelector( )
        {
            _packSelector.SetValueWithoutNotify( "None" );

            _packSelector.RegisterValueChangedCallback( e =>
            {
                if ( _iData.Count == 0 )
                {
                    DisableAllElementExcept( _createPackButton, _closeButton );
                    return;
                }

                _curIndex = Mathf.Clamp( _packSelector.index, 0, _iData.Count - 1 );

                 
                 
                 
                if ( _currentData != null )
                    if ( _currentData.IsUnsaved )
                    {
                        var prevName = e.previousValue;

                        FlushElementsData( );

                        _iData.Remove( _currentData );
                        _modularPacksNames.Remove( prevName );
                        _currentData = null;

                        _packSelector.choices = _modularPacksNames;
                        _packSelector.index = _curIndex;
                        return;
                    }

                SelectAndSetupContainer( _curIndex );
            } );
        }

        private void ConfigureNewPackButton( )
        {
            _createPackButton.clicked += ( ) =>
            {
                FlushElementsData( );

                if ( _currentData != null )
                    if ( _currentData.IsUnsaved )
                    {
                        var prevName = _currentData.Pack.Name;

                        _currentData.Pack = new ModularPack( );
                        _currentData.Pack.Name = prevName;

                        SetupDataContainer( _currentData );
                        return;
                    }

                var newName = Texts.AssetSystem.Creation.NEW_PACK;
                var uniqueName = ObjectNames.GetUniqueName(
                    _iData.Select( i => i.Pack.Name ).ToArray( ), newName );

                var newPack = new ModularPack( );
                newPack.Name = uniqueName;

                var newDataContainer = new ImporterDataContainer( );
                newDataContainer.IsUnsaved = true;
                newDataContainer.Pack = newPack;

                _iData.Add( newDataContainer );
                _modularPacksNames.Add( newDataContainer.GetName( ) );

                _packSelector.choices = _modularPacksNames;
                _packSelector.index = _modularPacksNames.Count - 1;
            };
        }


         
        private void ConfigureWallGroupsList( )
        {
            _wallListView.bindItem = ( visualElement, index ) =>
            {
                var label = visualElement.Q<Label>( );

                if ( string.IsNullOrEmpty( _wallGroups[ index ].Name ) )
                {
                    var newName = Texts.AssetSystem.Creation.NEW_WALL_GROUP;
                    var wallGroupUniqueName =
                        ObjectNames.GetUniqueName( _wallGroups.Select( i => i.Name ).ToArray( ), newName );
                    _wallGroups[ index ].Name = wallGroupUniqueName;
                }

                label.text = _wallGroups[ index ].Name;

                Texture tex = ModularGroup.GetPreviewOrEmptyIcon( _wallGroups[ index ] );
                var image = visualElement.Q<MBSImageVisualElement>( );
                image.image = tex;
            };

             
            _wallListView.makeItem += ( ) => _listItemVta.Instantiate( );
            _wallListView.selectionType = SelectionType.Single;
            _wallListView.itemsAdded += e => { _wallGroups[ e.First( ) ] = new WallGroup( ); };

#if UNITY_2022_3_OR_NEWER
            _wallListView.itemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<WallGroupEditorWindow>( );
                w.SetupData( _wallGroups.ElementAt( _wallListView.selectedIndex ), _wallGroups );
                IsOpenningGroupWindow = false;
            };
#else
            _wallListView.onItemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<WallGroupEditorWindow>( );
                w.SetupData( _wallGroups.ElementAt( _wallListView.selectedIndex ), _wallGroups );
                IsOpenningGroupWindow = false;
            };
#endif
            
        }

        private void ConfigureFloorGroupsList( )
        {
            Action<VisualElement, int> floorBindItem = ( visualElement, index ) =>
            {
                var label = visualElement.Q<Label>( );

                if ( string.IsNullOrEmpty( _floorGroups[ index ].Name ) )
                {
                    var newName = Texts.AssetSystem.Creation.NEW_FLOOR_GROUP;
                    var floorGroupUniqueName =
                        ObjectNames.GetUniqueName( _wallGroups.Select( i => i.Name ).ToArray( ), newName );
                    _floorGroups[ index ].Name = floorGroupUniqueName;
                }

                label.text = _floorGroups[ index ].Name;

                Texture tex = ModularGroup.GetPreviewOrEmptyIcon( _floorGroups[ index ] );
                var image = visualElement.Q<MBSImageVisualElement>( );
                image.image = tex;
            };

             
            _floorListView.makeItem += ( ) => _listItemVta.Instantiate( );
            _floorListView.bindItem = floorBindItem;
            _floorListView.selectionType = SelectionType.Single;
            _floorListView.itemsAdded += e => { _floorGroups[ e.First( ) ] = new FloorGroup( ); };
            
#if UNITY_2022_3_OR_NEWER
            _floorListView.itemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<FloorGroupEditorWindow>( );
                w.SetupData( _floorGroups.ElementAt( _floorListView.selectedIndex ), _floorGroups );
                IsOpenningGroupWindow = false;
            };
#else
            _floorListView.onItemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<FloorGroupEditorWindow>( );
                w.SetupData( _floorGroups.ElementAt( _floorListView.selectedIndex ), _floorGroups );
                IsOpenningGroupWindow = false;
            };
#endif
        }

        private void ConfigureDecoratorGroupsList( )
        {
            Action<VisualElement, int> decoratorBindItem = ( visualElement, index ) =>
            {
                var label = visualElement.Q<Label>( );

                if ( string.IsNullOrEmpty( _decoratorGroups[ index ].Name ) )
                {
                    var newName = Texts.AssetSystem.Creation.NEW_DECOR_GROUP;
                    var prefabGroupUniqueName =
                        ObjectNames.GetUniqueName( _wallGroups.Select( i => i.Name ).ToArray( ), newName );
                    _decoratorGroups[ index ].Name = prefabGroupUniqueName;
                }

                label.text = _decoratorGroups[ index ].Name;

                Texture tex = ModularGroup.GetPreviewOrEmptyIcon( _decoratorGroups[ index ] );
                var image = visualElement.Q<MBSImageVisualElement>( );
                image.image = tex;
            };

             
            _decoratorListView.makeItem += ( ) => _listItemVta.Instantiate( );
            _decoratorListView.bindItem = decoratorBindItem;
            _decoratorListView.itemsAdded += e => { _decoratorGroups[ e.First( ) ] = new DecoratorGroup( ); };

#if UNITY_2022_3_OR_NEWER
            _decoratorListView.itemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<DecoratorGroupEditorWindow>( );
                w.SetupData( _decoratorGroups[ _decoratorListView.selectedIndex ], _decoratorGroups, _currentData.Pack );
                IsOpenningGroupWindow = false;
            };
#else
            _decoratorListView.onItemsChosen += e =>
            {
                IsOpenningGroupWindow = true;
                var w = GetWindow<DecoratorGroupEditorWindow>( );
                w.SetupData( _decoratorGroups[ _decoratorListView.selectedIndex ], _decoratorGroups, _currentData.Pack );
                IsOpenningGroupWindow = false;
            };
#endif
            
        }


         
        private void ConfigureRemovePackButton( )
        {
            _removePackButton.clicked += ( ) =>
            {
                if ( !IsButtonActionSafe( ) )
                    return;
                RemovePack( _packSelector.index, _currentData );
            };
        }

        private void RemovePack( int index, ImporterDataContainer dataContainer )
        {
            if ( IsPackUnsaved( index ) )
            {
                RemoveUnsavedPack( index );

                if ( _iData.Count == 0 )
                {
                    DisableAllElementExcept( _createPackButton, _closeButton );
                    _createPackButton.Focus( );
                }
                else
                {
                    EnableAllElementsExcept( );
                    _packSelector.index = _iData.Count - 1;
                }
            }
            else
            {
                _iData.RemoveAll( i => i.Pack.Guid == dataContainer.Pack.Guid );
                ModularPack_Manager.Singleton.RemovePack( dataContainer.Pack );

                AssetsImporterController.RefreshAssetsData( );
                SetupVElementsData( );

                _packSelector.index = Mathf.Clamp( index - 1, 0, _iData.Count - 1 );
            }
        }

        private bool IsPackUnsaved( int index )
        {
            return _iData.ElementAtOrDefault( index ).IsUnsaved;
        }

        private void RemoveUnsavedPack( int index )
        {
            var currentName = _modularPacksNames.ElementAt( index );

            _iData = _iData.Where( i => i.Pack.Guid != _currentData.Pack.Guid ).ToList( );
            _modularPacksNames = _modularPacksNames.Where( i => i != currentName ).ToList( );

            UpdateElements( );

            _currentData = null;
        }

         
        private void ConfigureHidePackButton( )
        {
            _hidePackButton.clicked += ( ) =>
            {
                if ( !IsButtonActionSafe( ) )
                    return;

                if ( _hidePackButton.text == _hideButtonStates[ 0 ] )
                    ModularPack_Manager.Singleton.HidePack( _currentData.Pack );
                else
                    ModularPack_Manager.Singleton.UnhidePack( _currentData.Pack );

                AssetsImporterController.RefreshAssetsData( );
                SetupVElementsData( );
                _packSelector.index = 0;
            };
        }


        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                if ( !IsButtonActionSafe( ) )
                    return;

                 
                var name = Text_Helper.TrimAndRemoveDoubleSpaces( _nameTextField.value );

                if ( !Text_Helper.IsTextLengthOk( name ) )
                {
                    Debug.LogError( MessageController.TextValidation_LengthLess2( _nameTextField.label ) );
                    return;
                }

                if ( !Text_Helper.IsTextContainsOnlyLettersAndDigits( name ) )
                {
                    Debug.LogError( MessageController.TextValidation_InapproppriateCharacters( _nameTextField.label ) );
                    return;
                }

                _currentData.Pack.Name = name;
                _currentData.Pack.WallGroups = _wallGroups.ToArray( );
                _currentData.Pack.FloorGroups = _floorGroups.ToArray( );
                _currentData.Pack.DecoratorGroups = _decoratorGroups.ToArray( );
                _currentData.Pack.DecoratorCategories = _decoratorCategories.ToArray( );

                if ( ModularPack_Manager.Singleton.SavePack( _currentData.Pack ) )
                {
                    SetupVElementsData( );

                    var savedPackIndex = _iData.FindIndex( i => i.Pack.Guid == _currentData.Pack.Guid );
                    _currentData = null;

                    if ( savedPackIndex > -1 )
                        _packSelector.index = savedPackIndex;
                    else
                        _packSelector.index = 0;
                }
            };
        }

        private void ConfigureDiscardButton( )
        {
            _discardButton.clicked += ( ) =>
            {
                if ( !IsButtonActionSafe( ) ) return;

                if ( IsPackUnsaved( _packSelector.index ) )
                {
                    RemoveUnsavedPack( _packSelector.index );
                }
                else
                {
                    var index = _packSelector.index;
                    SetupVElementsData( );
                    _packSelector.index = index;
                }
            };
        }

        private void ConfigureCloseButton( )
        {
            _closeButton.clicked += ( ) =>
            {
                FlushElementsData( );
                Close( );
            };
        }


        private void SetupVElementsData( )
        {
            FlushElementsData( );

            _iData = AssetsImporterController.GetImporterData( _doShowHidden );
            _modularPacksNames = _iData.Select( i => i.GetName( ) ).ToList( );
            UpdatePackSelector( );

            if ( _iData.Count == 0 )
            {
                DisableAllElementExcept( _createPackButton, _closeButton );
                _createPackButton.Focus( );
                return;
            }

            EnableAllElementsExcept( );
        }


        private void UpdatePackSelector( )
        {
            _packSelector.choices = _modularPacksNames;
        }

        private void UpdateElements( )
        {
            var prevIndex = _packSelector.index;

            if ( _iData.Count == 0 )
            {
                DisableAllElementExcept( _createPackButton, _closeButton );
                _createPackButton.Focus( );
                return;
            }

            EnableAllElementsExcept( );

            _packSelector.choices = _modularPacksNames;
            if ( prevIndex > -1 )
                _packSelector.index = prevIndex;

            _wallListView.Rebuild( );
            _floorListView.Rebuild( );
            _decoratorListView.Rebuild( );
        }

        private void EnableAllElementsExcept( params VisualElement[ ] excludeElements )
        {
            _packSelector.SetEnabled( true );
            _createPackButton.SetEnabled( true );

            _propertiesLabel.SetEnabled( true );
            _nameTextField.SetEnabled( true );

            _wallListView.SetEnabled( true );
            _floorListView.SetEnabled( true );
            _decoratorListView.SetEnabled( true );

            _removePackButton.SetEnabled( true );
            _hidePackButton.SetEnabled( true );

            _saveButton.SetEnabled( true );
            _discardButton.SetEnabled( true );
            _closeButton.SetEnabled( true );


            foreach ( var e in excludeElements ) e.SetEnabled( false );
        }

        private void ResetPackSelector( )
        {
            _packSelector.choices = new List<string>( );
            _packSelector.SetValueWithoutNotify( "None" );
        }

        private void DisableAllElementExcept( params VisualElement[ ] excludeElements )
        {
            ResetPackSelector( );
            _packSelector.SetEnabled( false );

            _createPackButton.SetEnabled( false );

            _propertiesLabel.SetEnabled( false );

            _nameTextField.SetValueWithoutNotify( "None" );
            _nameTextField.SetEnabled( false );

            _wallListView.itemsSource = new List<WallGroup>( );
            _wallListView.SetEnabled( false );

            _floorListView.itemsSource = new List<FloorGroup>( );
            _floorListView.SetEnabled( false );

            _decoratorListView.itemsSource = new List<DecoratorGroup>( );
            _decoratorListView.SetEnabled( false );

            _removePackButton.SetEnabled( false );
            _hidePackButton.SetEnabled( false );

            _saveButton.SetEnabled( false );
            _discardButton.SetEnabled( false );
            _closeButton.SetEnabled( false );

            foreach ( var e in excludeElements ) 
                e.SetEnabled( true );
        }


        private bool IsButtonActionSafe( )
        {
            if ( _currentData == null )
            {
                Debug.LogError( Texts.AssetSystem.UNSAFE_ACTION_CURRENT_DATA_MISSING );
                return false;
            }

            if ( _iData.Count == 0 )
            {
                Debug.Log( Texts.AssetSystem.UNSAFE_ACTION_IMPORTER_DATA_COUNT );
                return false;
            }

            if ( _modularPacksNames.Count == 0 )
            {
                Debug.Log( Texts.AssetSystem.UNSAFE_ACTION_PACK_NAMES_COUNT );
                return false;
            }

            if ( _packSelector.index < 0 )
            {
                Debug.LogError( Texts.AssetSystem.UNSAFE_ACTION_PACK_SELECTOR_INDEX_LESS_ZERO );
                return false;
            }

            return true;
        }


        private void SelectAndSetupContainer( int index )
        {
            var container = SelectContainerAt( index );

            if ( container != null )
                SetupDataContainer( container );
            else
                DisableAllElementExcept( _createPackButton, _closeButton );
        }

        private ImporterDataContainer SelectContainerAt( int index )
        {
            if ( _iData.Count == 0 )
                return null;

            index = Mathf.Clamp( index, 0, _iData.Count - 1 );

            return _iData.ElementAtOrDefault( index );
        }

        private void SetupDataContainer( ImporterDataContainer dataContainer )
        {
            if ( SetupPack( dataContainer.Pack ) )
            {
                _currentData = dataContainer;

                if ( _currentData.IsUnsaved )
                    EnableAllElementsExcept( _hidePackButton );
                else
                    EnableAllElementsExcept( );

                _hidePackButton.text = _hideButtonStates[ Convert.ToInt32( _currentData.IsHidden ) ];
            }
            else
            {
                _currentData = null;
                DisableAllElementExcept( _createPackButton, _closeButton );
            }
        }

        private bool SetupPack( ModularPack pack )
        {
            if ( pack == null )
                return false;

            _nameTextField.value = pack.Name;

            _wallGroups = pack.WallGroups.ToList( );
            _floorGroups = pack.FloorGroups.ToList( );
            _decoratorGroups = pack.DecoratorGroups.ToList( );
            _decoratorCategories = pack.DecoratorCategories.ToList( );

            _wallListView.itemsSource = _wallGroups;
            _wallListView.Rebuild(  );
            
            _floorListView.itemsSource = _floorGroups;
            _floorListView.Rebuild(  );

            _decoratorListView.itemsSource = _decoratorGroups;
            _decoratorListView.Rebuild(  );


            return true;
        }


         

        private void FlushElementsData( )
        {
            _nameTextField.SetValueWithoutNotify( "None" );

            _wallListView.Clear( );
            _wallGroups = null;

            _floorListView.Clear( );
            _floorGroups = null;

            _decoratorListView.Clear( );
            _decoratorGroups = null;

            ResetPackSelector(  );
        }

        public  void ReloadWindow( )
        {
            FlushElementsData( );
            SetupVElementsData( );
        }

        public  void RefreshWindow( )
        {
            UpdateElements( );
        }

        private void ToggleShowHiddenPacks( )
        {
            _doShowHidden = !_doShowHidden;
            SetupVElementsData( );
            _packSelector.index = 0;
        }

        #region UI elements

        private VisualTreeAsset _mainWindowVta;
        private VisualTreeAsset _listItemVta;

        private DropdownField _packSelector;
        private Button _createPackButton;

        private Label _propertiesLabel;
        private TextField _nameTextField;

        private ListView _wallListView;
        private ListView _floorListView;
        private ListView _decoratorListView;

        private Button _removePackButton;
        private Button _hidePackButton;

        private Button _saveButton;
        private Button _discardButton;
        private Button _closeButton;

        private readonly string[ ] _hideButtonStates = { "Hide", "Unhide" };

        #endregion
    }
}

#endif