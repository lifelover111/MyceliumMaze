#if UNITY_EDITOR

using MBS.Controller.Builder;
using MBS.Controller.Scene;
using MBS.Model.Configuration;
using MBS.Model.Scene;
using MBS.Utilities.Extensions;
using MBS.View.Builder;
using MBS.View.Input.GUI;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class Input_Floors_Tool
    {
        public  static void IdleMode( )
        {
            Mouse.CustomSnappingFunction = ( ) =>
            {
                Mouse.IsSnappedToEnd = false;
                var construction = MBSConstruction.Current;

                var mouseGridSnappedPosition = Mouse.FreeConstrPos;

                var gridSize = MbsGrid.CellSize;
                var assetSize = BuilderDataController.SelectedGroupSize;

                var x = assetSize.x / 2;
                var z = assetSize.z / 2;

                if ( SceneData.DoSnapToGrid )
                {
                    Vector3 topRight, botRight, topLeft, botLeft;
                    topRight = mouseGridSnappedPosition + new Vector3( x, 0, z );
                    botRight = mouseGridSnappedPosition + new Vector3( x, 0, -z );
                    botLeft = mouseGridSnappedPosition + new Vector3( -x, 0, -z );
                    topLeft = mouseGridSnappedPosition + new Vector3( -x, 0, z );


                    Vector3 tr, br, bl, tl;
                    bool isTopRight, isBotRight, isBotLeft, isTopLeft;

                    isTopRight = construction.TryFindEndPointToSnap( topRight, 0.5f, out tr );
                    isBotRight = construction.TryFindEndPointToSnap( botRight, 0.45f, out br );
                    isBotLeft = construction.TryFindEndPointToSnap( botLeft, 0.5f, out bl );
                    isTopLeft = construction.TryFindEndPointToSnap( topLeft, 0.45f, out tl );

                    if ( isTopRight || isBotRight || isBotLeft || isTopLeft )
                    {
                        var maxD = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
                        var trDif = isTopRight ? tr - topRight : maxD;
                        var brDif = isBotRight ? br - botRight : maxD;
                        var blDif = isBotLeft ? bl - botLeft : maxD;
                        var tlDif = isTopLeft ? tl - topLeft : maxD;

                        var aa = trDif.magnitude <= brDif.magnitude ? trDif : brDif;
                        var bb = blDif.magnitude <= tlDif.magnitude ? blDif : tlDif;
                        var cc = aa.magnitude <= bb.magnitude ? aa : bb;

                        mouseGridSnappedPosition += cc;
                        mouseGridSnappedPosition.y = MbsGrid.Position_Local.y;

                        Mouse.IsSnappedToEnd = true;
                    }
                    else
                    {
                        mouseGridSnappedPosition = MbsGrid.Snap_ToCenter( mouseGridSnappedPosition );
                    }
                }
                else
                {
                    mouseGridSnappedPosition = MbsGrid.Snap_ToCenter( mouseGridSnappedPosition );
                }


                return mouseGridSnappedPosition;
            };
        }

        public  static void DrawingMode( )
        {
             
            Mouse.CustomSnappingFunction = ( ) =>
            {
                var gridSize = MbsGrid.CellSize;
                var halfGrid = ( gridSize / 2 ).RoundDecimals( );

                var mouseGridSnappedPosition = Mouse.FreeConstrPos;


                mouseGridSnappedPosition = new Vector3(
                    Mathf.Floor( mouseGridSnappedPosition.x / gridSize ) * gridSize + halfGrid,
                    mouseGridSnappedPosition.y,
                    Mathf.Floor( mouseGridSnappedPosition.z / gridSize ) * gridSize + halfGrid
                );

                return mouseGridSnappedPosition;
            };
        }


        public  static void Setup_IdleInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Floor.IDLE_MOUSE_ACTION_LABEL,
                KeyAction = ( ) =>
                {
                    SceneData.StartPositionConstr = Mouse.SnappedConstrPos;

                    if ( BuildingProcedures_Floors.Start( ) )
                    {
                        SceneData.BuilderMode = BuilderMode.Drawing;
                        Setup_DrawingInputs( );
                        Mouse.IsSnappedToEnd = false;
                    }
                },
            } );

            InputManager.AddKeyElement( true,new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = Builder_Controller.Stop
            } );

            InputManager.AddKeyElement( true, new UIToggleElementData( )
            {
                Label = Texts.Inputs.MOVE_GRID_ORIGIN,
                Key = Config.Sgt.Input.FloorToolInputs.FloorMoveGridOrigin,
                KeyAction = ( ) => MbsGrid.Position_Local = Mouse.SnappedConstrPos,
                GetValueRemote = ( ) => false,
                OnValueChangeAction = ( b ) => MbsGrid.Position_Local = Vector3.zero,
                OnText = "Reset", OffText = "Reset",
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.FloorToolInputs.FloorAutofill,
                Label = Texts.Inputs.Floor.AUTOFILL_LABEL,
                KeyAction = BuildingProcedures_Floors.Autofill
            } );
        }


        private static void Setup_DrawingInputs( )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Floor.DRAWING_MOUSE_ACTION_LABEL,
                KeyAction = ( ) =>
                {
                    SceneData.StartPositionConstr = Mouse.SnappedConstrPos;

                    SceneData.IsMouseRecalcNeeded = true;
                    SceneData.BuilderMode = BuilderMode.Idle;
                    BuildingProcedures_Floors.End( );
                    Setup_IdleInputs( );
                }
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = ( ) =>
                {
                    SceneData.IsMouseRecalcNeeded = true;
                    SceneData.BuilderMode = BuilderMode.Idle;
                    BuildingProcedures_Floors.EndWithoutSaving( );
                    Setup_IdleInputs( );
                }
            } );
        }
    }
}
#endif