#if UNITY_EDITOR

using System.IO;
using System.Linq;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    public  class ModularPack_Saver
    {
        public  static bool SaveCreatedModularPack( ModularPack createdPack )
        {
             

            var folderPath = PathsManager.Singleton.ModularPacksPath + "/" + createdPack.Name;


            if ( AssetDatabase.IsValidFolder( folderPath ) == false )
            {
                 
                 
                var createdFolderPath = CreateModularPackFolder( createdPack.Name );

                if ( string.IsNullOrEmpty( createdFolderPath ) )
                {
                    Debug.LogFormat(
                        Texts.AssetSystem.ModularPack.Saver.CANNOT_CREATE_FOLDER,
                        folderPath );
                    return false;
                }

                var assetSavePath = createdFolderPath + PredefinedPaths.DESCRIPTOR_SUFFIX;

                CreateNewAssetAndSave( createdPack, assetSavePath );
            }
            else
            {
                 
                 

                var prevName = createdPack.Name;

                var subfolders = AssetDatabase.GetSubFolders( PathsManager.Singleton.ModularPacksPath );
                subfolders = subfolders.Select( i => Path.GetFileName( i ) ).ToArray( );

                var uniquePackName = ObjectNames.GetUniqueName( subfolders, createdPack.Name );

                createdPack.Name = uniquePackName;

                var result = SaveCreatedModularPack( createdPack );

                if ( result )
                    Debug.LogWarningFormat( Texts.AssetSystem.ModularPack.Saver.packNameHasChanged, prevName,
                                            uniquePackName );

                return result;
            }

            return true;
        }

        public  static bool SaveEditedModularPack( ModularPack editedPack, DescriptorData descriptorData )
        {
             
            if ( File.Exists( descriptorData.AssetPath ) )
            {
                 
                 
                 

                AssetDatabase.DeleteAsset( descriptorData.AssetPath );

                 
                 

                CreateNewAssetAndSave( editedPack, descriptorData.AssetPath );

                if ( descriptorData.FolderName != editedPack.Name )
                {
                    var renameResult = AssetDatabase.RenameAsset( descriptorData.FolderPath, editedPack.Name );

                    if ( !string.IsNullOrEmpty( renameResult ) ) Debug.LogError( renameResult );
                }
            }
            else
            {
                 
                 

                var newFolderPath = CreateModularPackFolder( editedPack.Name );
                var newAssetPath = newFolderPath + PredefinedPaths.DESCRIPTOR_SUFFIX;
                CreateNewAssetAndSave( editedPack, newAssetPath );
            }

            return true;
        }

        public  static void CreateNewAssetAndSave( ModularPack modularPack, string assetPath )
        {
            var newAsset = ConvertToJsonAsset( modularPack );
            AssetDatabase.CreateAsset( newAsset, assetPath );
        }

        public  static TextAsset ConvertToJsonAsset( ModularPack modularPack )
        {
            var contentToString = EditorJsonUtility.ToJson( modularPack, true );
            var textAsset = new TextAsset( contentToString );
            return textAsset;
        }

        public  static string CreateModularPackFolder( string folderName )
        {
            var createdFolderGuid = AssetDatabase.CreateFolder( PathsManager.Singleton.ModularPacksPath, folderName );
            var createdFolderPath = AssetDatabase.GUIDToAssetPath( createdFolderGuid );
            return createdFolderPath;
        }
    }
}

#endif