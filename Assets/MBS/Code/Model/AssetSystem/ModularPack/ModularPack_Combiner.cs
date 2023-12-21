#if UNITY_EDITOR

using System.Collections.Generic;
using MBS.Model.Configuration;

namespace MBS.Model.AssetSystem
{
    public  static class ModularPack_Combiner
    {
        public  static ModularPack[ ] GetCombinedArray( ModularPack[ ] assetPacks )
        {
            var combinedPack = CreateCombinedPack( assetPacks );

            var combinedList = new List<ModularPack>( );
            combinedList.Add( combinedPack );
            combinedList.AddRange( assetPacks );

            return combinedList.ToArray( );
        }

        private static ModularPack CreateCombinedPack( ModularPack[ ] assetPacks )
        {
            var wallGroups = new List<WallGroup>( );
            var floorGroups = new List<FloorGroup>( );
            var placerGroups = new List<DecoratorGroup>( );

            for ( var i = 0; i < assetPacks.Length; i++ )
            {
                var pack = assetPacks[ i ];

                wallGroups.AddRange( pack.WallGroups );
                floorGroups.AddRange( pack.FloorGroups );
                placerGroups.AddRange( pack.DecoratorGroups );
            }

            ModularPack combinedPack = new ModularPack
            {
                Name = "All",
                WallGroups = wallGroups.ToArray( ),
                FloorGroups = floorGroups.ToArray( ),
                DecoratorGroups = placerGroups.ToArray( ),
                Guid = PredefinedNames.COMBINER_MODULAR_PACK_GUID
            };

            return combinedPack;
        }
    }
}
#endif