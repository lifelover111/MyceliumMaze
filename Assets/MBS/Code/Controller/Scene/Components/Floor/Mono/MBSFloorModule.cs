#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Model.AssetSystem;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene.Mono
{
    public  enum FloorTileType
    {
        Square,
        Triangle
    }

    [ExecuteInEditMode]
    public  class MBSFloorModule : MonoBehaviour
    {
        [Serializable]
        public  class FloorAssetData
        {
            [SerializeField] public  string packGuid;
            [SerializeField] public  string groupGuid;
            [SerializeField] public  string moduleGuid;
            [SerializeField] public  FloorTileType tileType;
            [SerializeField] public  FloorCorner corner;
        }

        [SerializeField] private FloorAssetData _data;

        [NonSerialized] private ModularPack _origPack;
        [NonSerialized] private FloorGroup _origGroup;
        [NonSerialized] private FloorModule _origModule;


        public  FloorAssetData Data => _data;

        public  ModularPack OrigPack
        {
            get
            {
                if ( _origPack != null )
                    return _origPack;

                LoadAssetsByGUIDs( );
                return _origPack;
            }
        }

        public  FloorGroup OrigGroup
        {
            get
            {
                if ( _origGroup != null )
                    return _origGroup;

                LoadAssetsByGUIDs( );
                return _origGroup;
            }
        }

        public  FloorModule OrigModule
        {
            get
            {
                if ( _origGroup != null )
                    return _origModule;

                LoadAssetsByGUIDs( );
                return _origModule;
            }
        }


        private void LoadAssetsByGUIDs( )
        {
            _origPack = ModularPack_Manager.Singleton.ModularPacks
                                           .FirstOrDefault( i => i.Guid == _data.packGuid );
            if ( _origPack == null )
            {
                Debug.LogError( "MBS. Wall Item. Cannot initialize data, modular pack with given GUID is not found." );
                return;
            }

            _origGroup = OrigPack.FloorGroups.FirstOrDefault( i => i.Guid == _data.groupGuid );
            if ( _origGroup == null )
            {
                Debug.LogError( "MBS. Wall Item. Cannot initialize data, modular group with given GUID is not found." );
                return;
            }

            _origModule = OrigGroup.Modules.FirstOrDefault( i => i.Guid == _data.moduleGuid );
            if ( _origModule == null )
            {
                Debug.LogError( "MBS. Wall Item. Cannot initialize data, module with given GUID is not found." );
            }
        }


        public  MBSFloorModule ChangeModule( FloorModule newModule, bool isMultipleChange )
        {
            var prefabToChange = newModule.SquarePrefab;

            if ( Data.tileType == FloorTileType.Triangle )
                if ( newModule.TriangularPrefab != null )
                    prefabToChange = newModule.TriangularPrefab;

            var instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab( prefabToChange );
            instantiatedPrefab.transform.position = transform.position;
            instantiatedPrefab.transform.SetParent( transform.parent );

            var instantiatedFloorModule = instantiatedPrefab.AddComponent<MBSFloorModule>( );
            instantiatedFloorModule.WhenChanged( _data, newModule );


            if ( isMultipleChange == false )
            {
                instantiatedPrefab.RecordCreatedUndo( );
                instantiatedPrefab.SelectObject( );
            }
            else
            {
                instantiatedPrefab.RecordCreatedUndo( );
                instantiatedPrefab.AddToSelection( );
            }

            this.gameObject.DestroyImmediateUndo( );

            return instantiatedFloorModule;
        }

        public  void WhenPlaced( ModularPack pack, ModularGroup group, Module module, FloorTileType tileType,
                                  FloorCorner corner )
        {
            _origPack = pack;
            _origGroup = group as FloorGroup;
            _origModule = module as FloorModule;

            _data = new FloorAssetData
            {
                packGuid = pack.Guid,
                groupGuid = group.Guid,
                moduleGuid = module.Guid,
                tileType = tileType,
                corner = corner
            };

            InitializeChildren( );
        }

        private void WhenPlaced( FloorAssetData data )
        {
            _data = new FloorAssetData
            {
                packGuid = data.packGuid,
                groupGuid = data.groupGuid,
                moduleGuid = data.moduleGuid,
                tileType = data.tileType,
                corner = data.corner
            };

            LoadAssetsByGUIDs( );
            InitializeChildren( );
        }

        private void WhenChanged( FloorAssetData data, FloorModule newModule )
        {
            _data = new FloorAssetData
            {
                packGuid = data.packGuid,
                groupGuid = data.groupGuid,
                moduleGuid = newModule.Guid,
                tileType = data.tileType,
                corner = data.corner
            };

            if ( _data.tileType == FloorTileType.Triangle )
            {
                if ( newModule.TriangularPrefab != null )
                {
                    if ( _data.corner is FloorCorner.All )
                    {
                        Debug.LogError( "MBS. Unexpected error. If you see this, please inform the developer about this error: roman.indiedev@gmail.com" );
                        return;
                    }

                    Floor_MeshModifier.ModifyFloorCorner( this.gameObject, _data.corner );
                }
                else
                {
                    Debug.LogWarning( "MBS. Current module has no triangle prefab, so instead I used square one." );
                }
            }

            InitializeChildren( );
        }

        private void InitializeChildren( )
        {
            var children = transform.GetComponentsInChildren<MeshFilter>( );

            for ( var i = 0; i < children.Length; i++ )
            {
                if ( children[ i ].gameObject == this.gameObject )
                    continue;

                children[ i ].gameObject.AddComponent<MBSFloorModule_Child>( );
            }
        }


        public  void Rotate( bool dirRight = true, bool isMultiple = false )
        {
            

            if ( _data.tileType == FloorTileType.Square )
            {
                var angle = ( dirRight ) ? 90 : -90;
                transform.Rotate( Vector3.up, angle );
            }
            else
            {
                if ( OrigModule == null )
                {
                    Debug.LogError( "MBS. Cannot rotate the object, original module not found. " );
                    return;
                }
                
                if ( OrigModule.TriangularPrefab == null )
                {
                    var angle = ( dirRight ) ? 90 : -90;
                    transform.Rotate( Vector3.up, angle );
                    return;
                }

                int nextCornerIndex;
                var collectionSize = Enum.GetNames( typeof( FloorCorner ) ).Length - 1;

                if ( dirRight )
                    nextCornerIndex = Collections_Helper.GetNextLoopIndex( (int)_data.corner, collectionSize );
                else
                    nextCornerIndex = Collections_Helper.GetPrevLoopIndex( (int)_data.corner, collectionSize );

                var nextCorner = (FloorCorner)nextCornerIndex;

                _data.corner = nextCorner;
                
                ChangeModule( OrigModule, isMultiple );
            }
        }

        public  void SplitSquareIntoTriangles( bool isMultiple )
        {
            if ( OrigModule == null )
            {
                Debug.LogError( "MBS. Cannot split the object, original module not found." );
                return;
            }

            if ( _data.tileType == FloorTileType.Square )
            {
                if ( OrigModule.TriangularPrefab == null )
                {
                    Debug.LogError( "MBS. Cannot split the square floor object, triangle prefab not found." );
                    return;
                }

                var g1 = (GameObject)PrefabUtility.InstantiatePrefab( OrigModule.TriangularPrefab );
                var g2 = (GameObject)PrefabUtility.InstantiatePrefab( OrigModule.TriangularPrefab );


                var transform = this.transform;
                g1.transform.position = transform.position;
                g2.transform.position = transform.position;

                g1.transform.SetParent( transform.parent );
                g2.transform.SetParent( transform.parent );

                FloorAssetData assetData = new FloorAssetData
                {
                    packGuid = _data.packGuid,
                    groupGuid = _data.groupGuid,
                    moduleGuid = _data.moduleGuid,
                    tileType = FloorTileType.Triangle,
                    corner = FloorCorner.TopRight
                };

                var f1 = g1.AddComponent<MBSFloorModule>( );
                f1.WhenPlaced( assetData );
                Floor_MeshModifier.ModifyFloorCorner( f1.gameObject, f1.Data.corner );

                assetData.corner = FloorCorner.BotLeft;

                var f2 = g2.AddComponent<MBSFloorModule>( );
                f2.WhenPlaced( assetData );
                Floor_MeshModifier.ModifyFloorCorner( f2.gameObject, f2.Data.corner );

                if ( isMultiple == false )
                {
                    g1.gameObject.SelectObject( );
                    g2.gameObject.AddToSelection( );
                }
                else
                {
                    g1.gameObject.AddToSelection( );
                    g2.gameObject.AddToSelection( );
                }

                g1.RecordCreatedUndo( );
                g2.RecordCreatedUndo( );

                this.gameObject.DestroyImmediateUndo( );
            }
        }

        public  void ChangeTileType( bool isMultiple )
        {
            if ( OrigModule == null )
            {
                Debug.LogError( "MBS. Cannot split the object, original module not found." );
                return;
            }

            GameObject g1;
            FloorTileType tileType;

            if ( _data.tileType == FloorTileType.Square )
            {
                if ( OrigModule.TriangularPrefab == null )
                {
                    Debug.LogError( "MBS. Cannot change tile type, triangle prefab not found." );

                    return;
                }

                tileType = FloorTileType.Triangle;
                g1 = (GameObject)PrefabUtility.InstantiatePrefab( OrigModule.TriangularPrefab );
                Floor_MeshModifier.ModifyFloorCorner( g1, _data.corner );
            }
            else
            {
                if ( OrigModule.SquarePrefab == null )
                {
                    Debug.LogError( "MBS. Cannot change tile type, square prefab not found." );
                    return;
                }

                g1 = (GameObject)PrefabUtility.InstantiatePrefab( OrigModule.SquarePrefab );
                tileType = FloorTileType.Square;
            }

            g1.transform.position = this.transform.position;
            g1.transform.SetParent( this.transform.parent );

            FloorAssetData assetData = new FloorAssetData
            {
                packGuid = _data.packGuid,
                groupGuid = _data.groupGuid,
                moduleGuid = _data.moduleGuid,
                tileType = tileType,
                corner = _data.corner
            };

            var f1 = g1.AddComponent<MBSFloorModule>( );
            f1.WhenPlaced( assetData );

            if ( isMultiple == false )
                g1.gameObject.SelectObject( );
            else
                g1.gameObject.AddToSelection( );

            g1.RecordCreatedUndo( );
            this.gameObject.DestroyImmediateUndo( );
        }
    }
}

#endif