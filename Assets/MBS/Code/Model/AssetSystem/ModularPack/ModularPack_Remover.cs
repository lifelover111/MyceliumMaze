#if UNITY_EDITOR

using System.IO;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    public  static class ModularPack_Remover
    {
        public  static bool RemoveModularPack( DescriptorData descriptorData )
        {
            if ( File.Exists( descriptorData.AssetPath ) )
                if ( !AssetDatabase.DeleteAsset( descriptorData.AssetPath ) )
                {
                    Debug.LogError( string.Format( Texts.AssetSystem.ModularPack.Remover.CANNOT_REMOVE_FOLDER,
                                                   descriptorData.AssetPath ) );
                    return false;
                }

            var info = new DirectoryInfo( descriptorData.FolderPath );
            var subfolders = info.GetDirectories( );
            var subFiles = info.GetFiles( );

            if ( subfolders.Length == 0 && subFiles.Length == 0 )
                AssetDatabase.DeleteAsset( descriptorData.FolderPath );

            return true;
        }
    }
}

#endif