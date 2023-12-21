#if UNITY_EDITOR

using System.Linq;
using MBS.Controller.Builder;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using MBS.View.Input.Physical;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public enum DecoratorMode
    {
        Placement,
        Rotation,
        Scaling
    }


    public static class BuildingProcedures_Decorator
    {
        private static DecoratorModule _module;


        private static GameObject _currentPrefab;
        private static Transform _transform;
        private static Quaternion _currentRotation;
        private static Vector3 _currentScale;

        private static MeshRenderer[ ] _meshRenderers;
        private static MeshFilter[ ] _meshFilters;
        private static Bounds _localBounds;
        private static Bounds _worldBounds;


        private static DecoratorMode _mode;


        private static Ray _worldRay;
        private static RaycastHit _rHit;


        public static void InitializeAsset( )
        {
            Clear( );

            switch ( BuilderDataController.SelectedModule )
            {
                case null:
                    Debug.LogError( Texts.Building.CANNOT_START_DRAWING_MODULE );
                    return;

                case DecoratorModule module:
                    _module = module;


                    if ( module.DefaultPrefab != null )
                    {
                        _currentPrefab = (GameObject)PrefabUtility.InstantiatePrefab( module.DefaultPrefab );
                        _currentPrefab.transform.SetParent( MBSConstruction.Current.transform );
                        var colliders = _currentPrefab.GetComponentsInChildren<Collider>( );
                        foreach ( var collider in colliders )
                            collider.enabled = false;

                        _currentRotation = _currentPrefab.transform.rotation;
                    }
                    else
                    {
                        Debug.LogError( Texts.Building.CANNOT_START_DRAWING_PREFAB );
                        return;
                    }

                    break;


                default:
                    return;
            }

            _transform = _currentPrefab.transform;
            _currentScale = _transform.localScale;
            _currentRotation = _transform.rotation;

            _meshRenderers = _currentPrefab.GetComponentsInChildren<MeshRenderer>( );
            _meshFilters = _currentPrefab.GetComponentsInChildren<MeshFilter>( );
        }

        public static void Start( )
        {
            ChangeMode( _mode );
        }

        public static void Run( )
        {
            if ( _transform == null )
                return;

            _localBounds = CalculateBoundsOfChildren( _transform, _meshFilters );
            _worldBounds = _meshRenderers[ 0 ].bounds;
            if ( _meshRenderers.Length > 1 )
                for ( var i = 1; i < _meshRenderers.Length; i++ )
                {
                    _worldBounds.Encapsulate( _meshRenderers[ i ].bounds );
                }

            _worldRay = Mouse.WorldRay;

            switch ( _mode )
            {
                case DecoratorMode.Placement:
                    Decorator_PlacementMode.Run( _transform, _module.DefaultPrefab, _currentRotation, _localBounds,
                                                 _worldBounds, _worldRay );
                    break;
                case DecoratorMode.Rotation:
                    Decorator_RotationMode.Run( _transform, _worldBounds );
                    break;
                case DecoratorMode.Scaling:
                    Decorator_ScalingMode.Run( _transform );
                    break;
            }
        }

        private static Bounds CalculateBoundsOfChildren( Transform transform, MeshFilter[ ] meshFilters )
        {
            // var totalBounds = meshFilters[ 0 ].sharedMesh.bounds;
            var totalBounds = new Bounds( Vector3.zero, Vector3.zero );

            for ( var i = 0; i < meshFilters.Length; i++ )
            {
                // if ( i == 0 )
                //     continue;

                var mf = meshFilters[ i ];
                if ( mf != null )
                {
                    var childBounds = mf.sharedMesh.bounds;
                    var childTransform = mf.transform;

                    var worldBoundCenter = (Vector3)( childTransform.localToWorldMatrix * childBounds.center );
                    var transformChildCenter =
                        transform.InverseTransformPoint( childTransform.position + worldBoundCenter );

                    totalBounds.Encapsulate( transformChildCenter + childBounds.size / 2 );
                    totalBounds.Encapsulate( transformChildCenter - childBounds.size / 2 );
                }
            }

            totalBounds.size = Matrix4x4.Scale( transform.localScale ) * totalBounds.size;

            return totalBounds;
        }

        public static void End( )
        {
            Clear( );
            Decorator_PlacementMode.Clear( );
            Decorator_RotationMode.Clear( );
        }

        public static void Clear( )
        {
            if ( _currentPrefab != null )
                Object.DestroyImmediate( _currentPrefab );

            _transform = null;
            _currentScale = default;
            _currentRotation = default;
            _worldBounds = default;
            _mode = default;
            _worldRay = default;
            _module = default;
        }

        public static void ChangeMode( DecoratorMode mode )
        {
            switch ( mode )
            {
                case DecoratorMode.Placement:
                    Decorator_PlacementMode.Start( );
                    InputDecorator_Tool.Setup_PlacementInputs( );

                    break;

                case DecoratorMode.Rotation:
                    if ( _transform != null )
                    {
                        Decorator_RotationMode.Start( _transform, _worldBounds );
                        InputDecorator_Tool.Setup_RotationInputs( );
                    }

                    break;

                case DecoratorMode.Scaling:
                    if ( _transform != null )
                    {
                        Decorator_ScalingMode.Start( _transform );
                        InputDecorator_Tool.Setup_ScalingInputs( );
                    }

                    break;
            }

            _mode = mode;
        }


        public static void MouseAction( )
        {
            switch ( _mode )
            {
                case DecoratorMode.Placement:
                    PlacerObject( );
                    if ( Decorator_PlacementMode.RandomModuleOnPlace )
                        BuilderDataController.SelectRandomModule( );
                    break;

                case DecoratorMode.Rotation:
                    _currentRotation = Decorator_RotationMode.Accept( _transform );
                    ChangeMode( DecoratorMode.Placement );
                    break;

                case DecoratorMode.Scaling:
                    _currentScale = Decorator_ScalingMode.Accept( _transform );
                    ChangeMode( DecoratorMode.Placement );
                    break;
            }
        }

        public static void CancelAction( )
        {
            switch ( _mode )
            {
                case DecoratorMode.Placement:
                    Builder_Controller.Stop( );
                    break;

                case DecoratorMode.Rotation:
                    Decorator_RotationMode.Cancel( );
                    ChangeMode( DecoratorMode.Placement );
                    break;

                case DecoratorMode.Scaling:
                    Decorator_ScalingMode.Cancel( _transform );
                    ChangeMode( DecoratorMode.Placement );
                    break;
            }
        }

        private static void PlacerObject( )
        {
            if ( _module == null )
            {
                Debug.LogError( Texts.Building.CANNOT_START_DRAWING_MODULE );
                return;
            }

            if ( _currentPrefab == null )
            {
                Debug.LogError( Texts.Building.CANNOT_START_DRAWING_PREFAB );
                return;
            }

            var placedPrefab = (GameObject)PrefabUtility.InstantiatePrefab( _module.DefaultPrefab );

            placedPrefab.transform.position = _transform.position;
            placedPrefab.transform.rotation = _transform.rotation;
            placedPrefab.transform.localScale = _transform.localScale;
            placedPrefab.transform.SetParent( MBSConstruction.Current.transform );

            var curPack = ModularPack_Manager.Singleton.ModularPacks.ToList( )
                                             .Find( i => i.DecoratorGroups.FirstOrDefault(
                                                             j => j.Guid == BuilderDataController.SelectedGroup
                                                                      .Guid ) != null );

            var curGroup = BuilderDataController.SelectedGroup as DecoratorGroup;
            var curModule = _module;

            var placerComponent = placedPrefab.AddComponent<MBSDecoratorModule>( );
            placerComponent.WhenPlaced( curPack, curGroup, curModule );

            placedPrefab.RecordCreatedUndo( );
        }


        public static void ResetRotation( )
        {
            if ( _transform != null )
            {
                _transform.rotation = _module.DefaultPrefab.transform.rotation;
                _currentRotation = _transform.rotation;
                Decorator_RotationMode.initialRotation = _currentRotation;
            }
        }

        public static void ResetScale( )
        {
            if ( _transform != null )
            {
                _transform.localScale = _module.DefaultPrefab.transform.localScale;
                _currentScale = _transform.localScale;
            }
        }

        public static void ResetAllTransforms( )
        {
            if ( _transform != null )
            {
                _transform.rotation = _module.DefaultPrefab.transform.rotation;
                _transform.localScale = _module.DefaultPrefab.transform.localScale;
            }
        }
    }
}
#endif