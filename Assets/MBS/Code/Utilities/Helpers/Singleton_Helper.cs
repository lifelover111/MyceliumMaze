#if UNITY_EDITOR

using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.Utilities.Helpers
{
    public  class SingletonHelper
    {
        public  static T GetSingleton<T>( T singleton, string fileName ) where T : ScriptableObject, ISingleton
        {
            if ( singleton != null )
            {
                singleton.WarmInitialization( );
                return singleton;
            }

            T retval = null;

            var pathsManager = PathsManager.Singleton;
           
            PathsManager.Start(  );

            var guids = AssetDatabase.FindAssets( "t: " + nameof( T ), new[ ] { pathsManager.TempDataPath } );

            if ( guids.Length == 0 )
            {
                retval = ScriptableObject.CreateInstance<T>( );

                retval.ColdInitialization( );

                var fullPathForNewAsset = pathsManager.TempDataPath + "/" + fileName + ".asset";
                
                AssetDatabase.CreateAsset( retval, fullPathForNewAsset );
            }
            else if ( guids.Length == 1 )
            {
                var loadedAsset = AssetDatabase.LoadAssetAtPath<T>( AssetDatabase.GUIDToAssetPath( guids[ 0 ] ) );

                if ( loadedAsset != null )
                {
                    loadedAsset.WarmInitialization( );

                    retval = loadedAsset;
                }
            }

            return retval;
        }
    }
}

#endif