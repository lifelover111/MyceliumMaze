#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Model.AssetSystem;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    public  class MBSDecoratorModule : MonoBehaviour
    {
        [Serializable]
        public  class PlacerAssetData
        {
            [SerializeField] public  string packGuid;
            [SerializeField] public  string groupGuid;
            [SerializeField] public  string moduleGuid;
        }

        [SerializeField] private PlacerAssetData _data;

        [NonSerialized] private ModularPack _origPack;
        [NonSerialized] private DecoratorGroup _origGroup;
        [NonSerialized] private DecoratorModule _origModule;


        public  PlacerAssetData Data => _data;

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

        public  DecoratorGroup OrigGroup
        {
            get
            {
                if ( _origGroup != null )
                    return _origGroup;

                LoadAssetsByGUIDs( );
                return _origGroup;
            }
        }

        public  DecoratorModule OrigModule
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

            _origGroup = OrigPack.DecoratorGroups.FirstOrDefault( i => i.Guid == _data.groupGuid );
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


        public  MBSDecoratorModule ChangeModule( DecoratorModule newModule, bool isMultipleChange )
        {
            var prefabToChange = newModule.DefaultPrefab;


            var instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab( prefabToChange );
            instantiatedPrefab.transform.position = transform.position;
            instantiatedPrefab.transform.SetParent( transform.parent );

            var instantiatedModule = instantiatedPrefab.AddComponent<MBSDecoratorModule>( );
            instantiatedModule.WhenChanged( _data, newModule );


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

            return instantiatedModule;
        }


        public  void WhenPlaced( ModularPack pack, ModularGroup group, Module module )
        {
            _origPack = pack;
            _origGroup = group as DecoratorGroup;
            _origModule = module as DecoratorModule;

            _data = new PlacerAssetData( )
            {
                packGuid = pack.Guid,
                groupGuid = group.Guid,
                moduleGuid = module.Guid
            };

            LoadAssetsByGUIDs( );
            InitializeChildren( );
        }

        public  void WhenChanged( PlacerAssetData data, DecoratorModule module )
        {
            _data = new PlacerAssetData( )
            {
                packGuid = data.packGuid,
                groupGuid = data.groupGuid,
                moduleGuid = data.moduleGuid
            };

            LoadAssetsByGUIDs( );
            InitializeChildren( );
        }


        private void InitializeChildren( )
        {
            var children = transform.GetComponentsInChildren<MeshFilter>( );

            for ( var i = 0; i < children.Length; i++ )
            {
                if ( children[ i ].gameObject == this.gameObject )
                    continue;

                var child = children[ i ].gameObject.AddComponent<MBSDecoratorModule_Child>( );
            }
        }
    }
}

#endif