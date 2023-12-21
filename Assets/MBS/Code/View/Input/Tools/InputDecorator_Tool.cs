#if UNITY_EDITOR
using System;
using System.Linq;
using MBS.Controller.Builder;
using MBS.Controller.Scene;
using MBS.Model.Configuration;
using MBS.View.Input.GUI;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class InputDecorator_Tool
    {
        public  static void Setup_PlacementInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Decorator.PlacementMode.MOUSE_ACTION_LABEL,
                KeyAction = BuildingProcedures_Decorator.MouseAction
            } );

            InputManager.AddKeyElement(true, new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.Decorator.PlacementMode.CANCEL_ACTION_LABEL,
                KeyAction = BuildingProcedures_Decorator.CancelAction
            } );
            
            InputManager.AddKeyElement( true,new UIElement( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.NextModule,
                Label = Texts.Inputs.Decorator.PlacementMode.NEXT_MODULE,
                KeyAction = BuilderDataController.SelectNextModule
            } );

            InputManager.AddKeyElement( true,new UIDropdownElement( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.PlacementType,
                Label = Texts.Inputs.Decorator.PlacementMode.PLACEMENT_TYPE,
                Choices = Decorator_PlacementMode.PLACEMENT_TYPE_TEXT.ToList( ),
                KeyAction = Decorator_PlacementMode.ChangePlacementType_Next,
                GetValueRemote = ( ) => Decorator_PlacementMode.PLACEMENT_TYPE_TEXT[ (int)Decorator_PlacementMode.placementType ],
                OnValueChangeAction = ( s ) =>
                {
                    var list = Decorator_PlacementMode.PLACEMENT_TYPE_TEXT.ToList( );
                    var type = (PlacementType)list.FindIndex( i => i == s );
                    Decorator_PlacementMode.ChangePlacementType( type );
                }
            } );

            InputManager.AddKeyElement( true,new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.CollisionToggle,
                Label = Texts.Inputs.Decorator.PlacementMode.COLLISION,
                KeyAction = ( ) => { Decorator_PlacementMode.IsCollisionOn = !Decorator_PlacementMode.IsCollisionOn; },
                GetValueRemote = ( ) => Decorator_PlacementMode.IsCollisionOn,
                OnValueChangeAction = b => Decorator_PlacementMode.IsCollisionOn = b
            } );

            InputManager.AddKeyElement( true,new UIDropdownElement( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.PlacementFacing,
                Label = Texts.Inputs.Decorator.PlacementMode.PLACEMENT_FACING,
                Choices = Decorator_PlacementMode.PLACEMENT_FACING_TEXT.ToList( ),
                KeyAction = Decorator_PlacementMode.ChangePlacementFacing_Next,
                GetValueRemote = ( ) =>
                {
                    var index = (int)Decorator_PlacementMode.placementFacing;
                    return Decorator_PlacementMode.PLACEMENT_FACING_TEXT[ index ];
                },
                OnValueChangeAction = ( s ) =>
                {
                    var list = Decorator_PlacementMode.PLACEMENT_FACING_TEXT.ToList( );
                    var facing = (PlacementFacing)list.FindIndex( i => i == s );
                    Decorator_PlacementMode.ChangePlacementFacing( facing );
                }
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.RandomModuleToggle,
                Label = Texts.Inputs.Decorator.PlacementMode.RANDOM_MODULE,
                KeyAction = ( ) =>
                {
                    Decorator_PlacementMode.RandomModuleOnPlace = !Decorator_PlacementMode.RandomModuleOnPlace;
                },
                GetValueRemote = ( ) => Decorator_PlacementMode.RandomModuleOnPlace,
                OnValueChangeAction = b => Decorator_PlacementMode.RandomModuleOnPlace = b,
            } );


            InputManager.AddKeyElement( true,
                new UIToggleElementData( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Placement.AdjustHeightToggle,
                    Label = Texts.Inputs.Decorator.PlacementMode.ADJUST_HEIGHT,
                    KeyAction = ( ) => { Decorator_PlacementMode.DoAdjustHeight = !Decorator_PlacementMode.DoAdjustHeight; },
                    GetValueRemote = ( ) => Decorator_PlacementMode.DoAdjustHeight,
                    OnValueChangeAction = ( b ) => Decorator_PlacementMode.DoAdjustHeight = b
                },
                new UIDropdownElement( )
                {
                    Label = Texts.Inputs.Decorator.PlacementMode.ADJUST_HEIGHT_SPACE,
                    Choices = Enum.GetNames( typeof( Space ) ).ToList( ),
                    GetValueRemote = ( ) => Decorator_PlacementMode.AdjustHeight_SpaceMode.ToString( ),
                    OnValueChangeAction = ( s ) =>
                        Decorator_PlacementMode.AdjustHeight_SpaceMode = Enum.Parse<Space>( s )
                },
                new UIFloatInputData( )
                {
                    Label = Texts.Inputs.Decorator.PlacementMode.ADJUST_HEIGHT_HEIGHT,
                    GetValueRemote = ( ) => Decorator_PlacementMode.AdjustHeight,
                    OnValueChangeAction = ( f ) => Decorator_PlacementMode.AdjustHeight = f
                } );


            InputManager.AddKeyElement( true,new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.SnapToGrid,
                Label = Texts.Inputs.Decorator.PlacementMode.SNAP_TO_GRID,
                KeyAction = ( ) => Decorator_PlacementMode.DoSnapToGrid = !Decorator_PlacementMode.DoSnapToGrid,
                GetValueRemote = ( ) => Decorator_PlacementMode.DoSnapToGrid,
                OnValueChangeAction = ( b ) => Decorator_PlacementMode.DoSnapToGrid = b
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.RotationMode,
                Label = Texts.Inputs.Decorator.PlacementMode.ROTATION_MODE,
                KeyAction = ( ) => { BuildingProcedures_Decorator.ChangeMode( DecoratorMode.Rotation ); },
                GetValueRemote = ( ) => false,
                OnValueChangeAction = ( b ) => BuildingProcedures_Decorator.ResetRotation( ),
                OnText = "Reset", OffText = "Reset"
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Placement.ScalingMode,
                Label = Texts.Inputs.Decorator.PlacementMode.SCALING_MODE,
                KeyAction = ( ) => { BuildingProcedures_Decorator.ChangeMode( DecoratorMode.Scaling ); },
                GetValueRemote = ( ) => false,
                OnValueChangeAction = ( b ) => BuildingProcedures_Decorator.ResetScale( ),
                OnText = "Reset", OffText = "Reset"
            } );
        }

        public  static void Setup_RotationInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Decorator.RotationMode.MOUSE_ACTION_LABEL,
                KeyAction = ( ) =>
                {
                    BuildingProcedures_Decorator.MouseAction( );
                     
                }
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.Decorator.RotationMode.CANCEL_ACTION_LABEL,
                KeyAction = BuildingProcedures_Decorator.CancelAction
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Rotation.ResetRotation,
                Label = Texts.Inputs.Decorator.RotationMode.RESET_ROTATION_LABEL,
                KeyAction = BuildingProcedures_Decorator.ResetRotation
            } );


            InputManager.AddKeyElement( true,
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.XAxis,
                    Label = Texts.Inputs.Decorator.RotationMode.X_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_RotationMode.Axis = DecoratorRotationAxis.XAxis
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.YAxis,
                    Label = Texts.Inputs.Decorator.RotationMode.Y_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_RotationMode.Axis = DecoratorRotationAxis.YAxis
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.ZAxis,
                    Label = Texts.Inputs.Decorator.RotationMode.Z_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_RotationMode.Axis = DecoratorRotationAxis.ZAxis
                },
                new UIDropdownElement( )
                {
                    Label = Texts.Inputs.Decorator.RotationMode.ROTATE_AROUND,
                    Choices = Decorator_RotationMode.AxisTextArray.ToList( ),
                    KeyAction = Decorator_RotationMode.ChangeAxis_Next,
                    GetValueRemote = ( ) =>
                    {
                        var index = (int)Decorator_RotationMode.Axis;
                        return Decorator_RotationMode.AxisTextArray[ index ];
                    },
                    OnValueChangeAction = ( s ) =>
                    {
                        var list = Decorator_RotationMode.AxisTextArray.ToList( );
                        var index = list.FindIndex( i => i == s );
                        Decorator_RotationMode.Axis = (DecoratorRotationAxis)index;
                    }
                },
                new UIDropdownElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.ChangeRotationSpace,
                    Label = Texts.Inputs.Decorator.RotationMode.ROTATION_SPACE_LABEL,
                    Choices = Enum.GetNames( typeof( Space ) ).ToList( ),
                    KeyAction = Decorator_RotationMode.ChangeSpace_Next,
                    GetValueRemote = ( ) => Decorator_RotationMode.RotationSpace.ToString( ),
                    OnValueChangeAction = ( s ) => Decorator_RotationMode.RotationSpace = Enum.Parse<Space>( s )
                } );

            InputManager.AddKeyElement( true,
                new UIToggleElementData( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.SnapAngleToggle,
                    Label = Texts.Inputs.Decorator.RotationMode.SNAP_ANGLE_LABEL,
                    KeyAction = ( ) => { Decorator_RotationMode.DoSnapRotation = !Decorator_RotationMode.DoSnapRotation; },
                    GetValueRemote = ( ) => Decorator_RotationMode.DoSnapRotation,
                    OnValueChangeAction = ( b ) => Decorator_RotationMode.DoSnapRotation = b
                },
                new UIDropdownElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Rotation.ChangeSnapAccuracy,
                    Label = Texts.Inputs.Decorator.RotationMode.SNAP_ACCURACY_LABEL,
                    Choices = Decorator_RotationMode.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( ),
                    KeyAction = Decorator_RotationMode.ChangeSnapAccuracy_Next,
                    GetValueRemote = ( ) =>
                    {
                        var index = (int)Decorator_RotationMode.SnapStep;
                        return Decorator_RotationMode.SnapStepValueArray[ index ].ToString( );
                    },
                    OnValueChangeAction = ( s ) =>
                    {
                        var list = Decorator_RotationMode.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( );
                        var index = list.FindIndex( i => i == s );
                        Decorator_RotationMode.SnapStep = (RotationSnapStep)index;
                    }
                } );
        }

        public  static void Setup_ScalingInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Decorator.ScalingMode.MOUSE_ACTION_LABEL,
                KeyAction = BuildingProcedures_Decorator.MouseAction
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.Decorator.ScalingMode.CANCEL_ACTION_LABEL,
                KeyAction = BuildingProcedures_Decorator.CancelAction
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.PlacerToolInputs.Scaling.ResetScale,
                Label = Texts.Inputs.Decorator.ScalingMode.RESET_SCALE_LABEL,
                KeyAction = BuildingProcedures_Decorator.ResetScale
            } );


            InputManager.AddKeyElement( true,
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Scaling.AllAxis,
                    Label = Texts.Inputs.Decorator.ScalingMode.ALL_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_ScalingMode.Axis = Decorator_ScalingMode.ScalingAxis.All
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Scaling.XAxis,
                    Label = Texts.Inputs.Decorator.ScalingMode.X_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_ScalingMode.Axis = Decorator_ScalingMode.ScalingAxis.XAxis
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Scaling.YAxis,
                    Label = Texts.Inputs.Decorator.ScalingMode.Y_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_ScalingMode.Axis = Decorator_ScalingMode.ScalingAxis.YAxis
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Scaling.ZAxis,
                    Label = Texts.Inputs.Decorator.ScalingMode.Z_AXIS_LABEL,
                    KeyAction = ( ) => Decorator_ScalingMode.Axis = Decorator_ScalingMode.ScalingAxis.ZAxis
                },
                new UIDropdownElement( )
                {
                    Label = Texts.Inputs.Decorator.ScalingMode.SCALE_ALONG_LABEL,
                    Choices = Decorator_ScalingMode.ScalingAxisText_Array.ToList( ),
                    KeyAction = Decorator_ScalingMode.ChangeAxis_Next,
                    GetValueRemote = ( ) =>
                    {
                        var index = (int)Decorator_ScalingMode.Axis;
                        return Decorator_ScalingMode.ScalingAxisText_Array[ index ];
                    },
                    OnValueChangeAction = ( s ) =>
                    {
                        var list = Decorator_ScalingMode.ScalingAxisText_Array.ToList( );
                        var index = list.FindIndex( i => i == s );
                        Decorator_ScalingMode.Axis = (Decorator_ScalingMode.ScalingAxis)index;
                    }
                } );

            InputManager.AddKeyElement( true,
                new UIDropdownElement( )
                {
                    Key = Config.Sgt.Input.PlacerToolInputs.Scaling.ChangeSnapAccuracy,
                    Label = Texts.Inputs.Decorator.ScalingMode.SNAP_ACCURACY_LABEL,
                    Choices = Decorator_ScalingMode.ScalingSnapAccuracy_ValuesArray.Select( i => i.ToString( ) ).ToList( ),
                    KeyAction = Decorator_ScalingMode.ChangeAccuracy_Next,
                    GetValueRemote = ( ) =>
                    {
                        var index = (int)Decorator_ScalingMode.SnapAccuracy;
                        return Decorator_ScalingMode.ScalingSnapAccuracy_ValuesArray[ index ].ToString( );
                    },
                    OnValueChangeAction = ( s ) =>
                    {
                        var list = Decorator_ScalingMode.ScalingSnapAccuracy_ValuesArray.Select( i => i.ToString( ) )
                                                     .ToList( );
                        var index = list.FindIndex( i => i == s );
                        Decorator_ScalingMode.SnapAccuracy = (Decorator_ScalingMode.ScalingSnapAccuracy)index;
                    }
                } );
        }
    }
}
#endif