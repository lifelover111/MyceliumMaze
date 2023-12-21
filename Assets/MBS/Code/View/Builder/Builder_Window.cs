#if UNITY_EDITOR

using System.Collections.Generic;
using MBS.Controller.Builder;
using MBS.Controller.Configuration;
using MBS.Model.Builder;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MBS.View.Builder
{
    public class Builder_Window : EditorWindow
    {
        public static Builder_Window Instance { get; private set; }

        private void OnDisable( )
        {
            Stop( );
            Builder_Controller.Stop( );
        }


        private void CreateGUI( )
        {
            if ( Instance != null )
            {
                Close( );
                return;
            }

            Instance = this;

            titleContent = new GUIContent( Texts.Builder.Window.WINDOW_NAME );
            minSize = new Vector2( 480, 200 );
            ShowCreateButton( );
        }


        [MenuItem( Texts.MenuItems.BUILDER, false, 00 )]
        public static void ShowWindow( )
        {
            GetWindow<Builder_Window>( );
        }

        #region UI Variables

        private ToolbarToggle _wallToolToggle;
        private ToolbarToggle _floorToolToggle;
        private ToolbarToggle _placerToolToggle;

        private FloatField _gridSizeFloatField;
        private ToolbarToggle _gridSizeLinkToggle;
        private ToolbarToggle _gridSizePickerToggle;

        private FloatField _levelHeightFloatField;
        private ToolbarToggle _levelHeightPointerToggle;
        private ToolbarToggle _levelHeightPickerToggle;

        private IntegerField _levelNumberIntField;
        private ToolbarButton _levelNumberMinusToggle;
        private ToolbarButton _levelNumberPlusToggle;

        private DropdownField _packSelectorDropdown;
        private IMGUIContainer _gridSelectionIMGUIContainer;

        #endregion

        #region Main Work Logic

        public void Launch( ToolbarTool tool, List<string> packsNames )
        {
            rootVisualElement.Clear( );

            ImportVTAs( );
            QueryVisualElements( );
            ConfigureToolbar( );
            ConfigureGridSizeRow( );
            ConfigureLevelHeightRow( );
            ConfigureLevelNumberRow( );
            ConfigurePackSelector( );
            ConfigureIMGUIContainer( );

            _packSelectorDropdown.choices = packsNames;
            Toolbar_SetTool_Logic( tool );

            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        public void Stop( )
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            rootVisualElement.Clear( );
            ShowCreateButton( );
        }

        private void DuringSceneGUI( SceneView sceneView )
        {
            Builder_Controller.DuringSceneGUI( sceneView );
        }

        #endregion

        #region Init and Config Visual Elements

        private void ImportVTAs( )
        {
            var windowVta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_BuilderWindow( ) );
            rootVisualElement.Add( windowVta.CloneTree( ) );
        }

        private void QueryVisualElements( )
        {
            _wallToolToggle = rootVisualElement.Q<ToolbarToggle>( "toolbar-wall-toggle" );
            _floorToolToggle = rootVisualElement.Q<ToolbarToggle>( "toolbar-floor-toggle" );
            _placerToolToggle = rootVisualElement.Q<ToolbarToggle>( "toolbar-placer-toggle" );

            _gridSizeFloatField = rootVisualElement.Q<FloatField>( "grid-size-floatfield" );
            _gridSizeLinkToggle = rootVisualElement.Q<ToolbarToggle>( "grid-size-link-toggle" );
            _gridSizePickerToggle = rootVisualElement.Q<ToolbarToggle>( "grid-size-picker-toggle" );

            _levelHeightFloatField = rootVisualElement.Q<FloatField>( "level-height-floatfield" );
            _levelHeightPointerToggle = rootVisualElement.Q<ToolbarToggle>( "level-height-pointer-toggle" );
            _levelHeightPickerToggle = rootVisualElement.Q<ToolbarToggle>( "level-height-picker-toggle" );

            _levelNumberIntField = rootVisualElement.Q<IntegerField>( "level-number-intfield" );
            _levelNumberMinusToggle = rootVisualElement.Q<ToolbarButton>( "level-number-minus-button" );
            _levelNumberPlusToggle = rootVisualElement.Q<ToolbarButton>( "level-number-plus-button" );

            _packSelectorDropdown = rootVisualElement.Q<DropdownField>( "pack-selector-dropdown" );
            _gridSelectionIMGUIContainer = rootVisualElement.Q<IMGUIContainer>( "grid-selection-imgui-container" );
        }

        private void ConfigureToolbar( )
        {
            _wallToolToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                    Builder_Controller.ChangeTool( ToolbarTool.WallTool );
                else if ( e.newValue == false )
                    if ( _floorToolToggle.value == false && _placerToolToggle.value == false )
                        _wallToolToggle.SetValueWithoutNotify( true );
            } );

            _floorToolToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                    Builder_Controller.ChangeTool( ToolbarTool.FloorTool );
                else if ( e.newValue == false )
                    if ( _wallToolToggle.value == false && _placerToolToggle.value == false )
                        _floorToolToggle.SetValueWithoutNotify( true );
            } );

            _placerToolToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                    Builder_Controller.ChangeTool( ToolbarTool.PlacerTool );
                else if ( e.newValue == false )
                    if ( _wallToolToggle.value == false && _floorToolToggle.value == false )
                        _placerToolToggle.SetValueWithoutNotify( true );
            } );
        }

        private void ConfigurePackSelector( )
        {
            _packSelectorDropdown.RegisterValueChangedCallback( e =>
            {
                Builder_Controller.SelectPackAt(
                    _packSelectorDropdown.index );
            } );
        }

        private void ConfigureGridSizeRow( )
        {
            _gridSizeFloatField.RegisterValueChangedCallback( e =>
            {
                var absValue = e.newValue == 0
                                   ? 1
                                   : Mathf.Clamp( Mathf.Abs( e.newValue ), Config.Sgt.GridCellSizeMinLimit,
                                                  float.MaxValue );

                _gridSizeFloatField.SetValueWithoutNotify( absValue );


                Builder_Controller.SetGridCellSize( absValue );
            } );

            _gridSizeLinkToggle.RegisterValueChangedCallback( e =>
            {
                var gridCellSize = Builder_Controller.SetLockSizeToggle( e.newValue );

                if ( e.newValue )
                    _gridSizeFloatField.SetValueWithoutNotify( gridCellSize );

                _gridSizeFloatField.SetEnabled( !e.newValue );
            } );

            _gridSizePickerToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                {
                    SetGridPickerToggleValue( true );
                    Builder_Controller.ChangeSceneMode( SceneMode.Picker );
                    Builder_Controller.PickerMode_SetupGridCellSize( );
                }
                else
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
            } );
        }

        private void ConfigureLevelHeightRow( )
        {
            _levelHeightFloatField.RegisterValueChangedCallback( e =>
            {
                var absValue = Mathf.Abs( e.newValue );
                _levelHeightFloatField.SetValueWithoutNotify( absValue );

                Builder_Controller.SetGridLevelHeight( absValue );
            } );

            _levelHeightPointerToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                    Builder_Controller.ChangeSceneMode( SceneMode.Pointer );
                else
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
            } );

            _levelHeightPickerToggle.RegisterValueChangedCallback( e =>
            {
                if ( e.newValue )
                {
                    SetLevelHeightPickerToggleValue( true );
                    Builder_Controller.ChangeSceneMode( SceneMode.Picker );
                    Builder_Controller.PickerMode_SetupLevelHeight( );
                }
                else
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
            } );
        }

        private void ConfigureLevelNumberRow( )
        {
            _levelNumberIntField.RegisterValueChangedCallback( e =>
            {
                Builder_Controller.SetGridLevelNumber( e.newValue );
            } );

            _levelNumberMinusToggle.clicked += ( ) => { _levelNumberIntField.value--; };

            _levelNumberPlusToggle.clicked += ( ) => { _levelNumberIntField.value++; };
        }

        private void ConfigureIMGUIContainer( )
        {
            _gridSelectionIMGUIContainer.onGUIHandler = ( ) =>
            {
                var selectedGroupLength =
                    Builder_Controller.ShowSelectionGrid( (int)position.width, (int)position.height );

                if ( _gridSizeLinkToggle.value == true )
                {
                    if ( selectedGroupLength > -1 )
                        _gridSizeFloatField.value = selectedGroupLength;

                    _gridSizeLinkToggle.value = true;
                }
            };
        }

        #endregion


        #region UI Functions

        private void Toolbar_SetTool_Logic( ToolbarTool tool )
        {
            switch ( tool )
            {
                case ToolbarTool.WallTool:
                    _wallToolToggle.value = true;
                    break;
                case ToolbarTool.FloorTool:
                    _floorToolToggle.value = true;
                    break;
                case ToolbarTool.PlacerTool:
                    _placerToolToggle.value = true;
                    break;
            }
        }

        private void Toolbar_ChangeTool_Visual( ToolbarTool tool )
        {
            switch ( tool )
            {
                case ToolbarTool.WallTool:
                    _wallToolToggle.SetValueWithoutNotify( true );
                    _floorToolToggle.SetValueWithoutNotify( false );
                    _placerToolToggle.SetValueWithoutNotify( false );
                    break;
                case ToolbarTool.FloorTool:
                    _wallToolToggle.SetValueWithoutNotify( false );
                    _floorToolToggle.SetValueWithoutNotify( true );
                    _placerToolToggle.SetValueWithoutNotify( false );
                    break;
                case ToolbarTool.PlacerTool:
                    _wallToolToggle.SetValueWithoutNotify( false );
                    _floorToolToggle.SetValueWithoutNotify( false );
                    _placerToolToggle.SetValueWithoutNotify( true );
                    break;
            }
        }

        private void ShowCreateButton( )
        {
            var vtaEmpty =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( PathController.GetPATH_BuilderEmpty( ) );

            rootVisualElement.Add( vtaEmpty.CloneTree( ) );

            rootVisualElement.Q<Button>( ).clicked += ( ) => { Builder_Controller.Launch( ); };
        }

        #endregion


        #region External Functions

        public void SetupToolData( ToolbarTool tool, ToolData toolData )
        {
            Toolbar_ChangeTool_Visual( tool );

            _packSelectorDropdown.SetValueWithoutNotify( toolData.Pack.Name );
            _gridSizeLinkToggle.SetValueWithoutNotify( toolData.GridCellSize_LinkToggle );

            _gridSizeFloatField.value = toolData.GridCellSize;
            _levelHeightFloatField.value = toolData.GridLevelHeight;
            _levelNumberIntField.value = toolData.GridLevelNumber;

            if ( _gridSizeLinkToggle.value )
                if ( toolData.Group != null )
                    _gridSizeFloatField.value = toolData.Group.GetSize( ).x;

            _gridSizeFloatField.SetEnabled( !_gridSizeLinkToggle.value );
        }

        public void SetGridSizeValue( float newValue )
        {
            if ( _gridSizeFloatField == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.GRID_SIZE_FIELD_MISSING );
                return;
            }

            _gridSizeFloatField.SetValueWithoutNotify( newValue );
        }

        public void SetGridPickerToggleValue( bool newValue )
        {
            if ( _gridSizePickerToggle == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.GRID_SIZE_PICKET_TOGGLE_MISSING );
                return;
            }

            _gridSizePickerToggle.SetValueWithoutNotify( newValue );
        }

        public void SetGridSizeLinkToggleValue( bool newValue )
        {
            if ( _gridSizeLinkToggle == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.GRID_SIZE_LINK_TOGGLE_MISSING );
                return;
            }

            _gridSizeLinkToggle.SetValueWithoutNotify( newValue );
        }

        public void SetLevelHeightValue( float newValue )
        {
            if ( _levelHeightFloatField == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.LEVEL_HEIGHT_FIELD_MISSING );
                return;
            }

            _levelHeightFloatField.SetValueWithoutNotify( newValue );
        }

        public void SetLevelHeightPickerToggleValue( bool newValue )
        {
            if ( _levelHeightPickerToggle == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.LEVEL_HEIGHT_PICKER_TOGGLE );
                return;
            }

            _levelHeightPickerToggle.SetValueWithoutNotify( newValue );
        }

        public void SetLevelHeightPointerToggleValue( bool newValue )
        {
            if ( _levelHeightPointerToggle == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.LEVEL_HEIGHT_POINTER_TOGGLE );
                return;
            }

            _levelHeightPointerToggle.SetValueWithoutNotify( newValue );
        }

        public void SetLevelNumberValue( int newValue )
        {
            if ( _levelNumberIntField == null )
            {
                Debug.LogError( Texts.Builder.Window.Errors.LEVEL_NUMBER_INT_FIELD );
                return;
            }

            _levelNumberIntField.SetValueWithoutNotify( newValue );
        }

        #endregion
    }
}

#endif