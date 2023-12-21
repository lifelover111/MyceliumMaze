#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MBS.Model.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    public  class DescriptorData
    {
        public  TextAsset Asset;
        public  string AssetPath;
        public  string FolderName;
        public  string FolderPath;
        public  bool IsHidden;
        public  ModularPack Pack;
    }

    public  static class ModularPack_Importer
    {
        public  static List<DescriptorData> ImportAll( )
        {
            var hiddenPacksFolderName = PredefinedPaths.HIDDEN_PACKS_PATH.Remove( 0, 1 );

            var assetsWithPaths = IOHelper.FindAssetsWithPath<TextAsset>(
                PredefinedNames.DESCRIPTOR_NAME + " t: TextAsset",
                PathsManager.Singleton.ModularPacksPath );

            var loaderData = new List<DescriptorData>( assetsWithPaths.Count );

            foreach ( var a in assetsWithPaths )
            {
                var folderPath = Path.GetDirectoryName( a.path );
                var folderName = Path.GetFileName( folderPath );

                var isHidden = a.path.Contains( hiddenPacksFolderName );

                var d = new DescriptorData
                {
                    Asset = a.asset,
                    IsHidden = isHidden,
                    AssetPath = a.path,
                    FolderPath = folderPath,
                    FolderName = folderName
                };

                loaderData.Add( d );
            }


            loaderData = ParseJsonAssets( loaderData );
            loaderData = loaderData.Where( i => i != null ).ToList( );

            DataHealthCheck( loaderData );

            return loaderData;
        }

        private static List<DescriptorData> ParseJsonAssets( List<DescriptorData> dataCells )
        {
            var retval = dataCells.ToList( );

            for ( var i = 0; i < retval.Count; i++ )
                retval[ i ].Pack = ParseJsonAsset( retval[ i ].Asset );

            return retval;
        }

        private static ModularPack ParseJsonAsset( TextAsset asset )
        {
            if ( string.IsNullOrEmpty( asset.text ) )
            {
                Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Importer.PARSE_FAILED_JSON_EMPTY,
                                               AssetDatabase.GetAssetPath( asset ) ) );
                return null;
            }

             
            var deserializedPack = new ModularPack( );
            EditorJsonUtility.FromJsonOverwrite( asset.text, deserializedPack );
            return deserializedPack;
        }


        private static void DataHealthCheck( List<DescriptorData> dataCells )
        {
            for ( var i = 0; i < dataCells.Count; i++ )
            {
                var dd1 = dataCells.ElementAt( i );

                for ( var j = i + 1; j < dataCells.Count; j++ )
                {
                    var dd2 = dataCells.ElementAt( j );

                    if ( dd1.Pack.Guid == dd2.Pack.Guid )
                        Debug.LogErrorFormat(
                            Texts.AssetSystem.ModularPack.Importer.GUID_IS_EQUAL,
                            dd1.FolderPath, dd2.FolderPath );

                    if ( dd1.Pack.Name == dd2.Pack.Name )
                        Debug.LogErrorFormat(
                            Texts.AssetSystem.ModularPack.Importer.NAME_IS_EQUAL,
                            dd1.FolderPath, dd2.FolderPath );
                }
            }
        }
    }
}

#endif