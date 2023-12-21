#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Controller.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    [Serializable]
    public  class Module : UniqueNamedObject
    {
        [SerializeField] protected GameObject[ ] prefabs;
        [NonSerialized] protected Mesh[ ] CombinedMeshes;


        public  GameObject[ ] Prefabs
        {
            get => prefabs;
            set => prefabs = value;
        }


        public  Texture2D GetPreviewOrEmptyIcon( )
        {
            return GetPreviewOrEmptyIcon( this );
        }

        private Texture2D GetPreviewTexture( )
        {
            if ( prefabs == null || prefabs.Length == 0 )
                return null;

            Texture2D retval = null;

            var firstPrefab = prefabs.ElementAtOrDefault( 0 );

            if ( firstPrefab != null )
            {
                var texture = GameObjectHelper.GetPreviewTexture( firstPrefab );

                if ( texture != null )
                    retval = texture;
            }

            return retval;
        }

        public  virtual Vector3 GetSize( int index = 0 )
        {
            var mesh = GetPrefabCombinedMeshAt( index );

            if ( mesh == null )
                return Vector3.one;

            return mesh.bounds.size;
        }

        public  static Vector3 GetSize( Module module, int index = 0 )
        {
            if ( module == null )
                return Vector3.one;

            var mesh = module.GetPrefabCombinedMeshAt( index );

            if ( mesh == null )
                return Vector3.one;

            return mesh.bounds.size;
        }

        public  Mesh GetPrefabCombinedMeshAt( int prefabIndex )
        {
            if ( prefabs == null || prefabs.Length == 0 )
                return null;

            if ( CombinedMeshes == null || CombinedMeshes.Length != prefabs.Length )
                CombinedMeshes = new Mesh[ prefabs.Length ];
            else if ( CombinedMeshes[ prefabIndex ] != null )
                return CombinedMeshes[ prefabIndex ];

            var prefab = prefabs.ElementAtOrDefault( prefabIndex );
            
            var combinedMesh = GameObjectHelper.GetPrefabCombinedMesh( prefab );

            return combinedMesh;
        }

        public  static Texture2D GetPreviewOrEmptyIcon( Module asset )
        {
            if ( asset == null )
                return AssetDatabase.LoadAssetAtPath<Texture2D>( PathController.GetPATH_AssetEmptyIconPreview( ) );

            var assetPreview = asset.GetPreviewTexture( );

            if ( assetPreview == null )
                assetPreview =
                    AssetDatabase.LoadAssetAtPath<Texture2D>( PathController.GetPATH_AssetEmptyIconPreview( ) );

            return assetPreview;
        }
    }

    [Serializable]
    public  class WallModule : Module
    {
        public  WallModule( )
        {
            prefabs = new GameObject[ 2 ];
        }

        public  GameObject DefaultPrefab
        {
            get => prefabs[ 0 ];
            set => prefabs[ 0 ] = value;
        }

        public  GameObject ExtendedPrefab
        {
            get => prefabs[ 1 ];
            set => prefabs[ 1 ] = value;
        }

        public  enum PrefabType
        {
            Default = 0,
            Stretched = 1
        }
    }

    [Serializable]
    public  class FloorModule : Module
    {
        public  FloorModule( )
        {
            prefabs = new GameObject[ 2 ];
        }

        public  GameObject SquarePrefab
        {
            get => prefabs[ 0 ];
            set => prefabs[ 0 ] = value;
        }

        public  GameObject TriangularPrefab
        {
            get => prefabs[ 1 ];
            set => prefabs[ 1 ] = value;
        }
    }

    [Serializable]
    public  class DecoratorModule : Module
    {
        public  DecoratorModule( )
        {
            prefabs = new GameObject[ 1 ];
        }

        public  GameObject DefaultPrefab
        {
            get => prefabs[ 0 ];
            set => prefabs[ 0 ] = value;
        }
    }
}


#endif