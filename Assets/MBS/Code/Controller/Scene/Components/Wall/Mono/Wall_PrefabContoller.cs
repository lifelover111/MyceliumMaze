#if UNITY_EDITOR

using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using UnityEngine;

namespace MBS.Code.Builder.Scene
{
    public  class WallPrefabContoller
    {
        private const float MIN_SCALE_LIMIT = 0.795f;
        private const float MAX_SCALE_LIMIT = 1.205f;
        private const float STRETCHED_SCALE = 1.4142135623730950488016887242f;


         

        public  static (GameObject chosenPrefab, Vector3 additionalScale) ChangeModule_ChosePrefab(
            WallModule prevModule, WallModule newModule, GameObject prevOrigPrefab, float additionalScale )
        {
            if ( prevOrigPrefab == prevModule.ExtendedPrefab )
            {
                if ( newModule.ExtendedPrefab != null )
                    return ( newModule.ExtendedPrefab, Vector3.one );
                return ( newModule.DefaultPrefab, new Vector3( STRETCHED_SCALE, 1, 1 ) );
            }

            if ( prevOrigPrefab == prevModule.DefaultPrefab )
            {
                if ( additionalScale > MAX_SCALE_LIMIT )
                {
                     
                     
                    if ( newModule.ExtendedPrefab != null )
                    {
                        var scaleDifference = STRETCHED_SCALE - additionalScale;
                        var newScale = new Vector3( 1 - scaleDifference, 1, 1 );
                        return ( newModule.ExtendedPrefab, newScale );
                    }

                    return ( newModule.DefaultPrefab, new Vector3( additionalScale, 0, 0 ) );
                }

                return ( newModule.DefaultPrefab, Vector3.one );
            }

            Debug.LogError( Texts.Component.Wall.PREFAB_CONTROLLER_PREV_PREFAB_NOT_MATCH );
            return ( newModule.DefaultPrefab, Vector3.one );
        }
    }
}
#endif