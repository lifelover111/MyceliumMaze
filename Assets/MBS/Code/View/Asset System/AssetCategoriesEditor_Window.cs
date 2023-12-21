#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Configuration;
using MBS.Model.AssetSystem;
using UnityEditor;
using UnityEngine.UIElements;

namespace MBS.View.AssetSystem
{
    [Serializable]
    public  class AssetCategoriesEditorWindow : EditorWindow
    {
        private DecoratorGroup _assetGroup;
        private Button _cancelButton;
        private List<ModularGroupCategory> _defaultCategories;

        private ListView _defaultCategoriesListView;
        private ModularPack _modularPack;

        private Button _saveButton;
        private List<ModularGroupCategory> _userCategories;
        private ListView _userCategoriesListView;

        private void OnDestroy( )
        {
            GetWindow<DecoratorGroupEditorWindow>( desiredDockNextTo: typeof( DecoratorGroupEditorWindow ) )
                .UpdateElements( );
        }

        public  void CreateGui( )
        {
            ImportVtAs( );
            QueryVisualElements( );
            ConfigureDefaultCategories_ListView( );
            ConfigureUserCategories_List( );
            ConfigureSaveButton( );
            ConfigureCancelButton( );
        }

        private void ImportVtAs( )
        {
            var windowVta =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_AssetCategoriesWindow( ) );
            rootVisualElement.Add( windowVta.Instantiate( ) );
        }

        private void QueryVisualElements( )
        {
            _defaultCategoriesListView = rootVisualElement.Q<ListView>( "default-categories-listview" );
            _userCategoriesListView = rootVisualElement.Q<ListView>( "user-categories-listview" );
            _saveButton = rootVisualElement.Q<Button>( "save-button" );
            _cancelButton = rootVisualElement.Q<Button>( "cancel-button" );
        }

        private void ConfigureDefaultCategories_ListView( )
        {
            _defaultCategoriesListView.reorderable = true;
            _defaultCategoriesListView.makeItem += ( ) => new TextField( );
            _defaultCategoriesListView.bindItem += ( e, i ) =>
            {
                ( e as TextField ).value = _defaultCategories[ i ].Name;
            };
            _defaultCategoriesListView.SetEnabled( false );
        }


        private void ConfigureUserCategories_List( )
        {
            _userCategoriesListView.makeItem += ( ) => new TextField( );

            _userCategoriesListView.itemsAdded += e =>
            {
                var index = e.First( );

                if ( _userCategories.ElementAtOrDefault( index ) != null ) return;

                if ( index > 5 ) _userCategoriesListView.Q<Button>( "unity-list-view__add-button" ).SetEnabled( false );

                var nameIndex = index + 1;
                var name = "New Category " + nameIndex;

                while ( _userCategories.Any( i => i != null && i.Name == name ) )
                {
                    nameIndex++;
                    name = "New Category " + nameIndex;
                }

                _userCategories[ index ] = new ModularGroupCategory( name );
            };

            _userCategoriesListView.itemsRemoved += e =>
            {
                var count = _userCategories.Count;
                if ( count < 6 ) _userCategoriesListView.Q<Button>( "unity-list-view__add-button" ).SetEnabled( true );
            };

            _userCategoriesListView.bindItem += ( visualElement, index ) =>
            {
                var item = (TextField)visualElement;
                item.SetValueWithoutNotify( _userCategories[ index ].Name );
            };

            _userCategoriesListView.RegisterCallback<ChangeEvent<string>>( e =>
            {
                var index = _userCategoriesListView.selectedIndex;
                _userCategories[ index ].Name = e.newValue;
            } );
        }

        private void ConfigureSaveButton( )
        {
            _saveButton.clicked += ( ) =>
            {
                List<ModularGroupCategory> list = new List<ModularGroupCategory>( );
                list.AddRange( _defaultCategories );
                list.AddRange( _userCategories );
                _modularPack.DecoratorCategories = list.ToArray( );
                if ( HasOpenInstances<DecoratorGroupEditorWindow>( ) )
                    GetWindow<DecoratorGroupEditorWindow>( ).UpdateElements( );

                Close( );
            };
        }

        private void ConfigureCancelButton( )
        {
            _cancelButton.clicked += ( ) => { Close( ); };
        }


        public  void SetupData( DecoratorGroup assetGroup, ModularPack modularPack )
        {
            _modularPack = modularPack;
            _assetGroup = assetGroup;

            var allCategories = modularPack.DecoratorCategories.ToList( );

            _defaultCategories = new List<ModularGroupCategory> { allCategories.First( ) };
            _defaultCategoriesListView.itemsSource = _defaultCategories;

            _userCategories = new List<ModularGroupCategory>( );
            _userCategories.AddRange( allCategories.GetRange( 1, allCategories.Count - 1 ) );
            _userCategoriesListView.itemsSource = _userCategories;
        }
    }
}

#endif