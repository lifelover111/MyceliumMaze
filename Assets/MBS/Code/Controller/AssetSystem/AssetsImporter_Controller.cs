#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Model.AssetSystem;

namespace MBS.Controller.AssetSystem
{
    public  static class AssetsImporterController
    {
        public  static void RefreshAssetsData( )
        {
            ModularPack_Manager.Singleton.ImportAssets( );
        }

        public  static List<ImporterDataContainer> GetImporterData( bool includeHidden )
        {
            var descriptorsData = ModularPack_Manager.Singleton.DescriptorsData;
            if ( !includeHidden )
                descriptorsData = descriptorsData.Where( i => i.IsHidden != true ).ToArray( );

            var containers = new List<ImporterDataContainer>( );

            for ( var i = 0; i < descriptorsData.Length; i++ )
            {
                var container = new ImporterDataContainer
                {
                    Pack = descriptorsData[ i ].Pack,
                    IsHidden = descriptorsData[ i ].IsHidden,
                    IsUnsaved = false
                };

                containers.Add( container );
            }

            return containers;
        }
    }
}

#endif