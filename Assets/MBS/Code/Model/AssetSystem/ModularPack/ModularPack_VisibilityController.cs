#if UNITY_EDITOR

using System.IO;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    public  static class ModularPack_VisibilityController
    {
        public  static bool HideModularPack( DescriptorData descriptorData )
        {
            if ( !AssetDatabase.IsValidFolder( descriptorData.FolderPath ) )
            {
                Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Visibility.CANNOT_FIND_FOLDER,
                                               descriptorData.FolderPath ) );
                return false;
            }

            if ( File.Exists( descriptorData.AssetPath ) )
            {
                var newHiddenPath = PathsManager.Singleton.HiddenPacksPath + "/" + descriptorData.Pack.Name;

                var result = AssetDatabase.MoveAsset( descriptorData.FolderPath, newHiddenPath );

                if ( !string.IsNullOrEmpty( result ) )
                {
                    Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Visibility.CANNOT_MOVE_FOLDER,
                                                   descriptorData.FolderPath, newHiddenPath ) );
                    return false;
                }

                return true;
            }


            return false;
        }

        public  static bool UnhideModularPack( DescriptorData descriptorData )
        {
            if ( !AssetDatabase.IsValidFolder( descriptorData.FolderPath ) )
            {
                Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Visibility.CANNOT_FIND_FOLDER,
                                               descriptorData.FolderPath ) );
                return false;
            }

            if ( File.Exists( descriptorData.AssetPath ) )
            {
                var newUnhiddenPath = PathsManager.Singleton.ModularPacksPath + "/" + descriptorData.Pack.Name;

                var result = AssetDatabase.MoveAsset( descriptorData.FolderPath, newUnhiddenPath );

                if ( !string.IsNullOrEmpty( result ) )
                {
                    Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Visibility.CANNOT_MOVE_FOLDER,
                                                   descriptorData.FolderPath, newUnhiddenPath ) );
                    return false;
                }

                return true;
            }


            return false;
        }
    }
}

#endif