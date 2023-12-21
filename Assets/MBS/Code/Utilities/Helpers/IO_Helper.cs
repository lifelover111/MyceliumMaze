#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MBS.Utilities.Helpers
{
    public  static class IOHelper
    {
         
         
         
         
        public  static List<(T asset, string path)> FindAssetsWithPath<T>( string searchFilter,
                                                                          string searchInFolder,
                                                                          string searchFileExtension = null )
            where T : Object
        {
            var assetsGUIDs = AssetDatabase.FindAssets( searchFilter, new[ ] { searchInFolder } );

            var retval = new List<(T asset, string path)>( );

            foreach ( var assetGuid in assetsGUIDs )
            {
                var path = AssetDatabase.GUIDToAssetPath( assetGuid );

                if ( !string.IsNullOrEmpty( searchFileExtension ) )
                {
                    var extension = Path.GetExtension( path );

                    if ( extension == searchFileExtension )
                    {
                        var asset = (T)AssetDatabase.LoadAssetAtPath( path, typeof( T ) );
                        retval.Add( ( asset, path ) );
                    }
                }
                else
                {
                    var asset = (T)AssetDatabase.LoadAssetAtPath( path, typeof( T ) );
                    retval.Add( ( asset, path ) );
                }
            }

            return retval;
        }

         
         
         
         
         
        public  static string SearchFolder( string searchFolder )
        {
            var regEx = new Regex( @"/" + searchFolder + @"$" );

            string retval = null;

            var assetsSubfolders = AssetDatabase.GetSubFolders( "Assets" );

            foreach ( var folder in assetsSubfolders )
            {
                retval = SearchFolderRecursive( folder, regEx );

                if ( retval != null )
                    return retval;
            }

            return retval;
        }

        private static string SearchFolderRecursive( string inputFolder, Regex regex )
        {
            if ( regex.IsMatch( inputFolder ) ) return inputFolder;

            string path = null;

            var inputFolderSubfolders = AssetDatabase.GetSubFolders( inputFolder );

            foreach ( var folder in inputFolderSubfolders )
            {
                path = SearchFolderRecursive( folder, regex );

                if ( path != null )
                    return path;
            }

            return path;
        }
    }
}

#endif