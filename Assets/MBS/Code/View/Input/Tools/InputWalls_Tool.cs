#if UNITY_EDITOR

using System.Linq;
using MBS.Builder.Scene;
using MBS.Controller.Builder;
using MBS.Controller.Scene;
using MBS.Model.Configuration;
using MBS.Model.Scene;
using MBS.Utilities.Extensions;
using MBS.View.Input.GUI;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class InputWalls_Tool
    {
        public  static void IdleMode( )
        {
             
            Mouse.CustomSnappingFunction = ( ) =>
            {
                var mouseGridSnappedPosition = Mouse.FreeConstrPos;

                 
                if ( SceneData.DoSnapToGrid )
                     
                    mouseGridSnappedPosition = MbsGrid.Snap_ToBeginning( mouseGridSnappedPosition );

                 
                if ( SceneData.DoSnapToEnds )
                {
                     
                    if ( MBSConstruction.Current.TryFindEndPointToSnap( Mouse.FreeConstrPos,
                                                                        mouseGridSnappedPosition,
                                                                        0.5f,
                                                                        out var nearestEndPoint ) )
                    {
                         
                        mouseGridSnappedPosition = nearestEndPoint;
                        mouseGridSnappedPosition.y = MbsGrid.Position_Local.y;
                        Mouse.IsSnappedToEnd = true;
                    }
                    else
                    {
                        Mouse.IsSnappedToEnd = false;
                    }
                }
                else
                {
                    Mouse.IsSnappedToEnd = false;
                }

                return mouseGridSnappedPosition;
            };
        }

        public  static void DrawingMode( )
        {
             
             
             
             
            Mouse.CustomSnappingFunction = ( ) =>
            {
                var mouseGridSnappedPosition = Mouse.FreeConstrPos;


                if ( !SceneData.DoSnapToGrid )
                {
                     
                    if ( SceneData.DoSnapToEnds )
                    {
                         
                        if ( MBSConstruction.Current.TryFindEndPointToSnap( Mouse.FreeConstrPos,
                                                                            mouseGridSnappedPosition,
                                                                            0.5f,
                                                                            out var nearestEndPoint,
                                                                            SceneData.StartPositionConstr ) )
                        {
                             
                            if ( nearestEndPoint.ApxEquals( SceneData.StartPositionConstr ) )
                            {
                                Mouse.IsSnappedToEnd = false;
                            }
                            else
                            {
                                mouseGridSnappedPosition =
                                    new Vector3( nearestEndPoint.x, mouseGridSnappedPosition.y, nearestEndPoint.z );
                                Mouse.IsSnappedToEnd = true;
                            }
                        }
                        else
                        {
                            Mouse.IsSnappedToEnd = false;
                        }
                    }
                    else
                    {
                        Mouse.IsSnappedToEnd = false;
                    }

                    return mouseGridSnappedPosition;
                }

                var moduleSize = BuilderDataController.SelectedGroupSize;

                var diff = Mouse.FreeConstrPos - SceneData.StartPositionConstr;

                var absDifX = Mathf.Abs( diff.x );
                var absDifZ = Mathf.Abs( diff.z );

                if ( !absDifX.ApxEquals( absDifZ ) )
                {
                    if ( absDifX.ApxEquals( Mathf.Min( absDifX, absDifZ ) ) )
                    {
                        if ( !absDifX.ApxEquals( 0 ) && !absDifZ.ApxEquals( 0 ) )
                        {
                            if ( absDifX > absDifZ / 2 )
                                mouseGridSnappedPosition.x =
                                    SceneData.StartPositionConstr.x + absDifZ * Mathf.Sign( diff.x );
                            else
                                mouseGridSnappedPosition.x = SceneData.StartPositionConstr.x;
                        }
                    }
                    else
                    {
                        if ( absDifX != 0 && absDifZ != 0 )
                        {
                            if ( absDifZ > absDifX / 2 )
                                mouseGridSnappedPosition.z =
                                    SceneData.StartPositionConstr.z + absDifX * Mathf.Sign( diff.z );
                            else
                                mouseGridSnappedPosition.z = SceneData.StartPositionConstr.z;
                        }
                    }
                }

                diff = mouseGridSnappedPosition - SceneData.StartPositionConstr;

                mouseGridSnappedPosition = new Vector3(
                    SceneData.StartPositionConstr.x + Mathf.Round( diff.x / moduleSize.x ) * moduleSize.x,
                    Mouse.FreeConstrPos.y,
                    SceneData.StartPositionConstr.z + Mathf.Round( diff.z / moduleSize.x ) * moduleSize.x
                );

                return mouseGridSnappedPosition;
            };
        }

        public  static void Setup_IdleInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Wall.IDLE_MOUSE_ACTION_LABEL,
                KeyAction = BuildingProcedures_Walls.StartDrawing
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = KeyCode.Escape,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = Builder_Controller.Stop
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Label = Texts.Inputs.MOVE_GRID_ORIGIN,
                Key = Config.Sgt.Input.WallToolInputs.WallMoveGridOrigin,
                KeyAction = ( ) => MbsGrid.Position_Local = Mouse.SnappedConstrPos,
                GetValueRemote = ( ) => false,
                OnValueChangeAction = ( b ) => MbsGrid.Position_Local = Vector3.zero,
                OnText = "Reset", OffText = "Reset",
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.WallToolInputs.WallSnapToGrid,
                Label = Texts.Inputs.SNAP_TO_GRID,
                KeyAction = ( ) => SceneData.DoSnapToGrid = !SceneData.DoSnapToGrid,
                GetValueRemote = ( ) => SceneData.DoSnapToGrid,
                OnValueChangeAction = ( b ) => SceneData.DoSnapToGrid = b
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.WallToolInputs.WallSnapToEnds,
                Label = Texts.Inputs.SNAP_TO_ENDS,
                KeyAction = ( ) => SceneData.DoSnapToEnds = !SceneData.DoSnapToEnds,
                GetValueRemote = ( ) => SceneData.DoSnapToEnds,
                OnValueChangeAction = ( b ) => SceneData.DoSnapToEnds = b
            } );

            InputManager.AddKeyElement( true,
                new UIDropdownElement( )
                {
                    Key = Config.Sgt.Input.WallToolInputs.ChangeSnapAccuracy,
                    Label = Texts.Inputs.Wall.SNAP_ACCURACY_LABEL,
                    Choices = BuildingProcedures_Walls.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( ),
                    KeyAction = BuildingProcedures_Walls.ChangeSnapAccuracy_Next,
                    GetValueRemote = ( ) =>
                    {
                        var index = (int)BuildingProcedures_Walls.SnapStep;
                        return BuildingProcedures_Walls.SnapStepValueArray[ index ].ToString( );
                    },
                    OnValueChangeAction = ( s ) =>
                    {
                        var list = BuildingProcedures_Walls.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( );
                        var index = list.FindIndex( i => i == s );
                        BuildingProcedures_Walls.SnapStep = (WallAngleSnapStep)index;
                    }
                } );

            InputManager.AddKeyElement( false,
                new UIElement( )
                {
                    Key = Config.Sgt.Input.WallToolInputs.IncreaseLevelNumber,
                    Label = Texts.Inputs.Wall.INCREASE_LEVEL_NUMBER,
                    KeyAction = ( ) =>
                        BuildingProcedures_Walls.levelsNumber =
                            Mathf.Clamp( BuildingProcedures_Walls.levelsNumber + 1, 1, 999 ),
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.WallToolInputs.DecreaseLevelNumber,
                    Label = Texts.Inputs.Wall.DECREASE_LEVEL_NUMBER,
                    KeyAction = ( ) =>
                        BuildingProcedures_Walls.levelsNumber =
                            Mathf.Clamp( BuildingProcedures_Walls.levelsNumber - 1, 1, 999 ),
                },
                new UIIntegerInputData( )
                {
                     
                    Label = Texts.Inputs.Wall.LEVEL_NUMBER,
                    KeyAction = ( ) => { },
                    GetValueRemote = ( ) => BuildingProcedures_Walls.levelsNumber,
                    OnValueChangeAction = ( b ) => BuildingProcedures_Walls.levelsNumber = Mathf.Clamp( b, 1, 999 )
                },
                new UIFloatInputData( )
                {
                    Label = Texts.Inputs.Wall.LEVEL_HEIGHT,
                    KeyAction = ( ) => { },
                    GetValueRemote = ( ) => BuildingProcedures_Walls.levelHeight,
                    OnValueChangeAction = ( value ) => BuildingProcedures_Walls.levelHeight = Mathf.Clamp( value, 0.01f, 999f ),
                    AddValuePointer = true,
                    GetLinkedValue = ( ) =>
                    {
                        if ( BuilderDataController.SelectedModule == null )
                            return 1f;

                        return Mathf.Clamp( BuilderDataController.SelectedModule.GetSize( ).y, 0.1f, 999f );
                    }
                } );
        }

        public  static void Setup_DrawingInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Wall.DRAWING_MOUSE_ACTION_LABEL,
                KeyAction = BuildingProcedures_Walls.EndDrawing
            } );


            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = KeyCode.Escape,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = BuildingProcedures_Walls.EndDrawingWithoutSaving
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.WallToolInputs.WallSnapToGrid,
                Label = Texts.Inputs.SNAP_TO_GRID,
                KeyAction = ( ) => SceneData.DoSnapToGrid = !SceneData.DoSnapToGrid,
                GetValueRemote = ( ) => SceneData.DoSnapToGrid,
                OnValueChangeAction = ( b ) => SceneData.DoSnapToGrid = b
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Key = Config.Sgt.Input.WallToolInputs.WallSnapToEnds,
                Label = Texts.Inputs.SNAP_TO_ENDS,
                KeyAction = ( ) => SceneData.DoSnapToEnds = !SceneData.DoSnapToEnds,
                GetValueRemote = ( ) => SceneData.DoSnapToEnds,
                OnValueChangeAction = ( b ) => SceneData.DoSnapToEnds = b
            } );

            InputManager.AddKeyElement( true, new UIDropdownElement( )
            {
                Key = Config.Sgt.Input.WallToolInputs.ChangeSnapAccuracy,
                Label = Texts.Inputs.Wall.SNAP_ACCURACY_LABEL,
                Choices = BuildingProcedures_Walls.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( ),
                KeyAction = BuildingProcedures_Walls.ChangeSnapAccuracy_Next,
                GetValueRemote = ( ) =>
                {
                    var index = (int)BuildingProcedures_Walls.SnapStep;
                    return BuildingProcedures_Walls.SnapStepValueArray[ index ].ToString( );
                },
                OnValueChangeAction = ( s ) =>
                {
                    var list = BuildingProcedures_Walls.SnapStepValueArray.Select( i => i.ToString( ) ).ToList( );
                    var index = list.FindIndex( i => i == s );
                    BuildingProcedures_Walls.SnapStep = (WallAngleSnapStep)index;
                }
            } );

            InputManager.AddKeyElement( true, new UIElement( )
                {
                    Key = Config.Sgt.Input.WallToolInputs.IncreaseLevelNumber,
                    Label = Texts.Inputs.Wall.INCREASE_LEVEL_NUMBER,
                    KeyAction = ( ) =>
                        BuildingProcedures_Walls.levelsNumber =
                            Mathf.Clamp( BuildingProcedures_Walls.levelsNumber + 1, 1, 999 ),
                },
                new UIElement( )
                {
                    Key = Config.Sgt.Input.WallToolInputs.DecreaseLevelNumber,
                    Label = Texts.Inputs.Wall.DECREASE_LEVEL_NUMBER,
                    KeyAction = ( ) =>
                        BuildingProcedures_Walls.levelsNumber =
                            Mathf.Clamp( BuildingProcedures_Walls.levelsNumber - 1, 1, 999 ),
                },
                new UIIntegerInputData( )
                {
                     
                    Label = Texts.Inputs.Wall.LEVEL_NUMBER,
                    KeyAction = ( ) => { },
                    GetValueRemote = ( ) => BuildingProcedures_Walls.levelsNumber,
                    OnValueChangeAction = ( b ) =>
                        BuildingProcedures_Walls.levelsNumber = Mathf.Clamp( b, 1, 999 )
                },
                new UIFloatInputData( )
                {
                    Label = Texts.Inputs.Wall.LEVEL_HEIGHT,
                    KeyAction = ( ) => { },
                    GetValueRemote = ( ) => BuildingProcedures_Walls.levelHeight,
                    Disabled = true
                } );
        }
    }
}
#endif