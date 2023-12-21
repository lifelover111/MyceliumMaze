#if UNITY_EDITOR

using MBS.Controller.Builder;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;

namespace MBS.Controller.AssetSystem
{
    public  class ModularPackPostprocessor : AssetPostprocessor
    {
        private static bool _sIsThereChanges;

        public  static bool IsAssetsChanged
        {
            get
            {
                var prevValue = _sIsThereChanges;
                _sIsThereChanges = false;

                return prevValue;
            }
        }

        private static void OnPostprocessAllAssets( string[ ] importedAssets, string[ ] deletedAssets,
                                                    string[ ] movedAssets, string[ ] movedFromAssetPaths )
        {
            var modularPacksFolder = PathsManager.Singleton.ModularPacksPath;

            var mbsPackDescriptorAsset = PredefinedNames.DESCRIPTOR_FILE_NAME;
            
            foreach ( var a in importedAssets )
                if ( a.Contains( mbsPackDescriptorAsset ) && a.Contains( modularPacksFolder ) )
                    _sIsThereChanges = true;

            foreach ( var a in deletedAssets )
                if ( a.Contains( mbsPackDescriptorAsset ) && a.Contains( modularPacksFolder ) )
                    _sIsThereChanges = true;

            foreach ( var a in movedAssets )
                if ( a.Contains( mbsPackDescriptorAsset ) && a.Contains( modularPacksFolder ) )
                    _sIsThereChanges = true;

            foreach ( var a in movedFromAssetPaths )
                if ( a.Contains( mbsPackDescriptorAsset ) && a.Contains( modularPacksFolder ) )
                    _sIsThereChanges = true;

            if ( _sIsThereChanges ) 
                UpdateWindows( );
        }

        private static void UpdateWindows( )
        {
            if ( EditorWindow.HasOpenInstances<Builder_Window>( ) ) 
                Builder_Controller.Refresh( );
        }
    }
}

#endif