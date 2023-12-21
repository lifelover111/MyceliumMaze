#if UNITY_EDITOR

using System;
using MBS.Model.AssetSystem;
using UnityEngine;

namespace MBS.Model.Builder
{
    [Serializable]
    public  class ToolData
    {
         
        [SerializeField] public  string PackGUID;
        [SerializeField] public  string GroupGuid;
        [SerializeField] public  string CategoryGUID;

         
        [SerializeField] public  float GridCellSize;
        [SerializeField] public  bool GridCellSize_LinkToggle;
        [SerializeField] public  float GridLevelHeight;

        [SerializeField] public  int GridLevelNumber;

        [SerializeField] public  Vector2 SelectionGrid_ScrollPos;

        [NonSerialized] public  ModularGroupCategory Category;
        [NonSerialized] public  ModularGroup Group;

        [NonSerialized] public  ModularPack Pack;


        public  ToolData( )
        {
            PackGUID = null;
            GroupGuid = null;
            CategoryGUID = null;

            GridCellSize = 1;
            GridCellSize_LinkToggle = true;

            GridLevelHeight = 1;

            GridLevelNumber = 0;

            SelectionGrid_ScrollPos = Vector2.zero;
        }
    }
}

#endif