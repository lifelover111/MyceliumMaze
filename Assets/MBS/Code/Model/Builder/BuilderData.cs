#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Model.AssetSystem;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.Builder
{
    [Serializable]
    public  class BuilderData
    {
        [SerializeField] private ToolbarTool _selectedTool;
        [SerializeField] private ToolData[ ] _toolsSavedData;
        
        public  ToolbarTool SelectedTool
        {
            get => _selectedTool;
            set
            {
                _selectedTool = value;
            }
        }

        public  ToolData SelectedToolData
        {
            get => _toolsSavedData[ (int)SelectedTool ];
            set
            {
                _toolsSavedData[ (int)SelectedTool ] = value;
            }
        }

        public  void ColdInitialization( )
        {
            var toolsNumber = Enum.GetNames( typeof( ToolbarTool ) ).Length;
            _toolsSavedData = new ToolData[ toolsNumber ];

            for ( var i = 0; i < _toolsSavedData.Length; i++ )
                _toolsSavedData[ i ] = new ToolData( );
        }
        
        public  void InitializeAssetsData( ModularPack firstPack )
        {
            var fWallGroup = firstPack.WallGroups.FirstOrDefault( );
            var fFloorGroup = firstPack.FloorGroups.FirstOrDefault( );
            var fPlacerGroup = firstPack.DecoratorGroups.FirstOrDefault( );

            _toolsSavedData[ (int)ToolbarTool.WallTool ].Pack = firstPack;
            _toolsSavedData[ (int)ToolbarTool.WallTool ].Group = fWallGroup;

            _toolsSavedData[ (int)ToolbarTool.FloorTool ].Pack = firstPack;
            _toolsSavedData[ (int)ToolbarTool.FloorTool ].Group = fFloorGroup;

            _toolsSavedData[ (int)ToolbarTool.PlacerTool ].Pack = firstPack;
            _toolsSavedData[ (int)ToolbarTool.PlacerTool ].Group = fPlacerGroup;
        }
    }
}

#endif