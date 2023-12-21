#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Controller.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    public  abstract class ModularGroup : UniqueNamedObject
    {
        public  abstract Module[ ] CastedModules { get; }

        private Texture2D GetPreviewTexture( )
        {
            Texture2D retval = null;

            var firstModule = CastedModules?.FirstOrDefault( );

            if ( firstModule != null )
            {
                var firstPrefab = firstModule.Prefabs.ElementAtOrDefault( 0 );

                if ( firstPrefab != null )
                {
                    var texture = GameObjectHelper.GetPreviewTexture( firstPrefab );

                    if ( texture != null )
                        retval = texture;
                }
            }


            return retval;
        }

        public  Vector3 GetSize( int moduleIndex = 0 )
        {
            if ( CastedModules == null || CastedModules.Length <= moduleIndex )
                return Vector3.one;

            var firstModule = CastedModules.ElementAtOrDefault( moduleIndex );

            if ( firstModule == null )
                return Vector3.one;

            return firstModule.GetSize( );
        }

        public  static Vector3 GetSize( ModularGroup group )
        {
            if ( group == null )
                return Vector3.one;

            if ( group.CastedModules == null || group.CastedModules.Length == 0 )
                return Vector3.one;

            var firstModule = group.CastedModules.ElementAtOrDefault( 0 );

            return Module.GetSize( firstModule );
        }

        public  static Texture2D GetPreviewOrEmptyIcon( ModularGroup group )
        {
            if ( group == null )
                return AssetDatabase.LoadAssetAtPath<Texture2D>( PathController.GetPATH_AssetEmptyIconPreview( ) );

            var assetPreview = group.GetPreviewTexture( );

            if ( assetPreview == null )
                assetPreview =
                    AssetDatabase.LoadAssetAtPath<Texture2D>( PathController.GetPATH_AssetEmptyIconPreview( ) );

            return assetPreview;
        }

        public  bool IsEmpty( )
        {
            if ( CastedModules == null || CastedModules.Length == 0 )
                return true;

            return false;
        }
    }

    [Serializable]
    public  class WallGroup : ModularGroup
    {
        [SerializeField] private WallModule[ ] _modules;

        public  WallGroup( )
        {
            _modules = new WallModule[ 0 ];
        }

        public  WallModule[ ] Modules
        {
            get => _modules;
            set => _modules = value;
        }

        public  override Module[ ] CastedModules => _modules;
    }

    [Serializable]
    public  class FloorGroup : ModularGroup
    {
        [SerializeField] private FloorModule[ ] _modules;

        public  FloorGroup( )
        {
            _modules = new FloorModule[ 0 ];
        }

        public  FloorModule[ ] Modules
        {
            get => _modules;
            set => _modules = value;
        }

        public  override Module[ ] CastedModules => _modules;
    }

    [Serializable]
    public  class DecoratorGroup : ModularGroup
    {
        [SerializeField] private DecoratorModule[ ] _modules;
        [SerializeField] private string _assetCategoryGuid;


        public  DecoratorGroup( )
        {
            _modules = new DecoratorModule[ 0 ];
        }

        public  DecoratorModule[ ] Modules
        {
            get => _modules;
            set => _modules = value;
        }

        public  override Module[ ] CastedModules => _modules;

        public  string CategoryGuid
        {
            get => _assetCategoryGuid;
            set => _assetCategoryGuid = value;
        }
    }
}
#endif