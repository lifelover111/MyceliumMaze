#if UNITY_EDITOR

using System;
using MBS.Utilities.Helpers;
using UnityEditor;

namespace MBS.Model.Configuration
{
    public  class PathsManager
    {
        private static PathsManager _singleton;

        private string _codePath;
        private string _hiddenPacksPath;
        private string _internalDataPath;
        private string _modularPackPath;


        private string _rootPath;
        private string _tempDataPath;


        public  static PathsManager Singleton
        {
            get
            {
                if ( _singleton == null )
                {
                    _singleton = new PathsManager( );
                    _singleton.Initialize( );
                }

                return _singleton;
            }
        }

        public  string RootPath => _rootPath;
        public  string ModularPacksPath => _modularPackPath;
        public  string HiddenPacksPath => _hiddenPacksPath;
        public  string CodePath => _codePath;
        public  string TempDataPath => _tempDataPath;
        public  string InternalDataPath => _internalDataPath;

        public  static void Start( )
        {
            var pm = PathsManager.Singleton;
            pm.Initialize(  );
        }
        
        private void Initialize( )
        {
            _rootPath = IOHelper.SearchFolder( PredefinedNames.MBS_FOLDER );

            _modularPackPath = _rootPath + PredefinedPaths.MODULAR_PACKS_PATH;
            _hiddenPacksPath = _modularPackPath + PredefinedPaths.HIDDEN_PACKS_PATH;
            _codePath = _rootPath + PredefinedPaths.CODE_PATH;
            _tempDataPath = _rootPath + PredefinedPaths.TEMP_DATA_PATH;
            _internalDataPath = _rootPath + PredefinedPaths.INTERNAL_DATA_PATH;

            PathsHealthCheck( );
        }

        private void PathsHealthCheck( )
        {
            var isRootValid = AssetDatabase.IsValidFolder( _rootPath );
            var isPacksValid = AssetDatabase.IsValidFolder( _modularPackPath );
            var isHiddenValid = AssetDatabase.IsValidFolder( _hiddenPacksPath );
            var isCodeValid = AssetDatabase.IsValidFolder( _codePath );
            var isTempValid = AssetDatabase.IsValidFolder( _tempDataPath );
            var isInternalDataValid = AssetDatabase.IsValidFolder( _internalDataPath );

            if ( !isRootValid )
                throw new NullReferenceException(
                    string.Format( Texts.Configuration.PathsManager.CANT_FIND_FOLDER, _rootPath ) );

            if ( !isCodeValid )
                throw new NullReferenceException(
                    string.Format( Texts.Configuration.PathsManager.CANT_FIND_FOLDER, _modularPackPath ) );

            if ( !isInternalDataValid )
                throw new NullReferenceException(
                    string.Format( Texts.Configuration.PathsManager.CANT_FIND_FOLDER, _internalDataPath ) );


            if ( !isPacksValid )
            {
                var result = AssetDatabase.CreateFolder( _rootPath, PredefinedNames.MODULAR_PACKS_FOLDER );
                if ( string.IsNullOrEmpty( result ) )
                    throw new Exception( string.Format( Texts.Configuration.PathsManager.CANT_CREATE_FOLDER, _modularPackPath ) );
            }

            if ( !isHiddenValid )
            {
                var result = AssetDatabase.CreateFolder( _modularPackPath, PredefinedNames.HIDDEN_PACKS_FOLDER );
                if ( string.IsNullOrEmpty( result ) )
                    throw new Exception( string.Format( Texts.Configuration.PathsManager.CANT_CREATE_FOLDER, _hiddenPacksPath ) );
            }

            if ( !isTempValid )
            {
                var result = AssetDatabase.CreateFolder( _rootPath, PredefinedNames.TEMP_DATA_FOLDER );
                if ( string.IsNullOrEmpty( result ) )
                    throw new Exception( string.Format( Texts.Configuration.PathsManager.CANT_CREATE_FOLDER, _tempDataPath ) );
            }
        }
    }
}

#endif