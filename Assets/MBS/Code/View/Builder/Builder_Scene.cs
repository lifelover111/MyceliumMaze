#if UNITY_EDITOR

using MBS.Builder.Scene;
using MBS.Controller.Builder;
using MBS.Controller.Scene;
using MBS.Controller.Scene.Mono;
using MBS.Model.Scene;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Builder
{
    public  enum SceneMode
    {
        Builder,
        Pointer,
        Picker
    }

    public  enum ChangingGridParam
    {
        GridCellSize,
        LevelHeight
    }

    public  enum BuilderMode
    {
        Idle,
        Drawing
    }

    public  struct SceneFuncData
    {
        public  bool defaultOrAccent;

        public  bool isOk;

         
        public  LeadAxis leadAxis;

         
        public  Matrix4x4 matrix;
        public  Vector3 position;

        public  Quaternion rotation;

         
        public  Vector3 size;
    }


    public  static class Builder_Scene
    {
        public  static void SceneGUI( SceneView sceneView )
        {
             
            HandleUtility.AddDefaultControl( GUIUtility.GetControlID( FocusType.Keyboard ) );
            Tools.current = Tool.None;

             
            var current = Event.current;

            if ( current.type == EventType.MouseEnterWindow )
            {
                Mouse.IsMouseInScene = true;
                sceneView.Focus( );
            }
            else if ( current.type == EventType.MouseLeaveWindow )
            {
                Mouse.IsMouseInScene = false;
            }

            var funcData = new SceneFuncData( );
            switch ( SceneData.SceneMode )
            {
                case SceneMode.Builder:
                    switch ( BuilderDataController.Tool )
                    {
                        case ToolbarTool.WallTool:
                            funcData = Builder_WallTool( );
                            break;
                        case ToolbarTool.FloorTool:
                            funcData = Builder_FloorTool( );
                            break;
                        case ToolbarTool.PlacerTool:
                            funcData = Builder_PlacerTool( );
                            funcData.rotation = Quaternion.identity;
                            break;
                    }

                    break;
                case SceneMode.Pointer:
                    funcData = Pointer_Mode.DoPointerMode( current );
                    break;
                case SceneMode.Picker:
                    funcData = Picker_Mode.DoPickerMode( current );
                    break;
                default:
                    funcData = new SceneFuncData( );
                    break;
            }

            if ( funcData.isOk )
            {
                Gizmos_Controller.Matrix = funcData.matrix;
                Gizmos_Controller.Position = funcData.position;
                Gizmos_Controller.Rotation = funcData.rotation;
                Gizmos_Controller.DefaultOrAccent = funcData.defaultOrAccent;
                Gizmos_Controller.LearAxis = funcData.leadAxis;
                Gizmos_Controller.ObjBoundsSize = funcData.size;
            }

            sceneView.Repaint( );
        }


        private static SceneFuncData _wallToolData;
        private static SceneFuncData Builder_WallTool( )
        {
            _wallToolData = new SceneFuncData( );
            _wallToolData.isOk = Mouse.IsMouseInScene;
            _wallToolData.matrix = MBSConstruction.Current.transform.localToWorldMatrix;
            _wallToolData.defaultOrAccent = Mouse.IsSnappedToEnd;
            _wallToolData.rotation = Quaternion.identity;
            
            switch ( SceneData.BuilderMode )
            {
                case BuilderMode.Idle:
                    _wallToolData.position = Mouse.SnappedConstrPos;
                    InputWalls_Tool.IdleMode( );
                    break;
                
                case BuilderMode.Drawing:
                    _wallToolData.position = BuildingProcedures_Walls.Building( Mouse.SnappedConstrPos );
                    InputWalls_Tool.DrawingMode( );
                    break;
            }
            
            return _wallToolData;
        }

        
        private static SceneFuncData _floorToolData;
        private static SceneFuncData Builder_FloorTool( )
        {
            _floorToolData = new SceneFuncData( );
            _floorToolData.isOk = Mouse.IsMouseInScene;
            _floorToolData.position = Mouse.SnappedConstrPos;
            _floorToolData.matrix = MBSConstruction.Current.transform.localToWorldMatrix;
            _floorToolData.defaultOrAccent = Mouse.IsSnappedToEnd;
            _floorToolData.rotation = Quaternion.identity;

            if ( AssetsData.Floor.IsTriangularExist )
            {
                if ( BuildingProcedures_Floors.FloorGizmoSupervisor( out var outFloorType, out var outRotation ) )
                {
                    if ( outFloorType == FloorTileType.Square )
                    {
                        Gizmos_Controller.SetupMesh( AssetsData.Floor.SquareMesh );
                    }
                    else
                    {
                        Gizmos_Controller.SetupMesh( AssetsData.Floor.TriangularMesh );
                        _floorToolData.rotation = Quaternion.Euler( outRotation );
                    }
                }
                else
                {
                    Gizmos_Controller.SetupMesh( AssetsData.Floor.SquareMesh );
                }
            }
            
            switch ( SceneData.BuilderMode )
            {
                case BuilderMode.Idle:
                    Input_Floors_Tool.IdleMode( );
                    break;
                
                case BuilderMode.Drawing:
                    BuildingProcedures_Floors.Draw( );
                    Input_Floors_Tool.DrawingMode( );
                    break;
            }

            return _floorToolData;
        }

        private static SceneFuncData Builder_PlacerTool( )
        {
            switch ( SceneData.BuilderMode )
            {
                case BuilderMode.Idle:
                    BuildingProcedures_Decorator.Run( );
                    break;
            }

            return default;
        }
    }
}

#endif