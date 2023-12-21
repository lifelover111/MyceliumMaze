#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Controller.Configuration;
using MBS.Controller.Scene;
using MBS.Model.AssetSystem;
using MBS.Model.Builder;
using MBS.Model.Configuration;
using MBS.View.Builder;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Builder
{
    internal static class BuilderDataController
    {
        private static BuilderData _data;

        public static ToolbarTool Tool => _data.SelectedTool;
        public static ToolData ToolData => _data.SelectedToolData;

        public static ModularPack[ ] AllModularPacks { get; private set; }


        public static ModularPack SelectedModularPack { get; private set; }
        public static int SelectedModularPackIndex { get; private set; }
        public static ModularGroup[ ] SelectedModularPackGroups { get; private set; }


        public static ModularGroup SelectedGroup { get; private set; }
        public static Vector3 SelectedGroupSize { get; private set; }
        public static int SelectedGroupIndex { get; private set; }


        public static Module SelectedModule { get; private set; }
        private static Vector3 SelectedModuleSize { get; set; }
        private static int SelectedModuleIndex { get; set; }


        #region INITIALIZERS

        public static void Initialize( MBSConstruction construction )
        {
            InitializeBuilderData( construction );
            CombinePacks( );
        }


        private static void InitializeBuilderData( MBSConstruction mbsConstruction )
        {
            // var builderDataPath = PathController.GetPATH_BuilderDataAsset( );
            if ( mbsConstruction.builderData == null )
            {
                mbsConstruction.builderData = new BuilderData( );
                mbsConstruction.builderData.ColdInitialization( );
            }

            _data = mbsConstruction.builderData;
        }

        private static void CombinePacks( )
        {
            var packs = ModularPack_Manager.Singleton.DescriptorsData.Where( i => !i.IsHidden )
                                           .Select( i => i.Pack ).ToArray( );
            AllModularPacks = ModularPack_Combiner.GetCombinedArray( packs );
        }

        private static void LoadToolsAssets( )
        {
            var loadedModularPack = GetModularPackByGuid( _data.SelectedToolData.PackGUID );

            if ( loadedModularPack == null )
            {
                loadedModularPack = GetModularPackByIndex( 0 );

                if ( loadedModularPack == null )
                {
                    Debug.LogError( Texts.Builder.Data.CANNOT_LOAD_PACK );
                    ResetSelectedPackData( );
                    return;
                }
            }

            SetupModularPack( loadedModularPack, AllModularPacks );

            var loadedModularGroup = GetModularGroupByGuid( _data.SelectedToolData.GroupGuid );
            if ( loadedModularGroup == null )
            {
                loadedModularGroup = GetAssetGroupByIndex( 0 );

                if ( loadedModularGroup == null )
                {
                    Debug.LogWarning( Texts.Builder.Data.CANNOT_LOAD_GROUP );
                    ResetSelectedGroupData( );
                    return;
                }
            }


            SetupGroup( loadedModularGroup, SelectedModularPackGroups );
        }

        // public static void ClearData( )
        // {
        //     ResetSelectedModuleData( );
        //     ResetSelectedGroupData( );
        //     ResetSelectedModuleData( );
        //
        //     AllModularPacks = null;
        //     _data = null;
        // }

        #endregion


        #region EXTERNAL_Interface_Functions

        // public static void SaveBuilderData( )
        // {
        //     // if ( _data != null )
        //     //     AssetDatabase.SaveAssetIfDirty( _data );
        // }

        public static void SelectTool( ToolbarTool tool )
        {
            _data.SelectedTool = tool;
            LoadToolsAssets( );
        }

        public static bool SelectModularPackAt( int index )
        {
            var pack = GetModularPackByIndex( index );

            if ( pack == null )
            {
                if ( index > 0 )
                {
                    pack = GetModularPackByIndex( 0 );

                    if ( pack == null )
                    {
                        ResetSelectedPackData( );
                        Debug.LogErrorFormat( Texts.Builder.Data.CANNOT_SELECT_MODULAR_PACK_AT, index );
                        return false;
                    }
                }
                else
                {
                    ResetSelectedPackData( );
                    Debug.LogErrorFormat( Texts.Builder.Data.CANNOT_SELECT_MODULAR_PACK_AT, index );
                    return false;
                }
            }

            SetupModularPack( pack, AllModularPacks );
            SelectAssetGroupAt( 0 );
            return true;
        }

        public static bool SelectAssetGroupAt( int index )
        {
            var group = GetAssetGroupByIndex( index );

            if ( group == null )
            {
                Debug.LogWarning( $"MBS. BuilderData_Controller. Cannot select group at index {index}" );

                if ( index > 0 )
                {
                    index = 0;
                    group = GetAssetGroupByIndex( index );

                    if ( group == null )
                    {
                        ResetSelectedGroupData( );
                        Debug.LogError( $"MBS. Cannot select modular pack at index {index}" );
                        return false;
                    }
                }
                else
                {
                    ResetSelectedGroupData( );
                    return false;
                }
            }

            SetupGroup( group, SelectedModularPackGroups );

            return true;
        }

        #endregion


        #region public _Setters

        private static void SetupModularPack( ModularPack pack, ModularPack[ ] allPacks )
        {
            SelectedModularPack = pack;
            SelectedModularPackIndex = allPacks.ToList( ).FindIndex( i => i.Guid == pack.Guid );
            SelectedModularPackGroups = pack.GetGroupsByToolIndex( (int)_data.SelectedTool ).ToArray( );

            _data.SelectedToolData.PackGUID = pack.Guid;
            _data.SelectedToolData.Pack = pack;

            _onPackChange.ForEach( i => i.Invoke( SelectedModularPack ) );
        }

        private static void SetupGroup( ModularGroup group, ModularGroup[ ] toolGroups )
        {
            if ( group == null || toolGroups == null || toolGroups.Length == 0 )
            {
                ResetSelectedGroupData( );
                return;
            }

            SelectedGroup = group;
            SelectedGroupSize = ModularGroup.GetSize( group );
            SelectedGroupIndex = toolGroups.ToList( ).FindIndex( i => i.Guid == SelectedGroup.Guid );

            _data.SelectedToolData.Group = group;
            _data.SelectedToolData.GroupGuid = group.Guid;

            _onGroupChange.ForEach( i => i.Invoke( SelectedGroup, SelectedGroupSize ) );

            SetupModule( 0, SelectedGroup );
        }

        private static void SetupModule( int index, ModularGroup group )
        {
            if ( group == null )
            {
                ResetSelectedModuleData( );
                return;
            }

            SelectedModule = group.CastedModules.ElementAtOrDefault( index );

            if ( SelectedModule == null )
            {
                Debug.LogWarning( Texts.Builder.Data.CANNOT_LOAD_MODULE );
                ResetSelectedModuleData( );
                return;
            }

            SelectedModuleSize = Module.GetSize( SelectedModule );
            SelectedModuleIndex = index;

            _onModuleChange.ForEach( i => i?.Invoke( SelectedModule, SelectedModuleSize ) );
        }


        private static void ResetSelectedPackData( )
        {
            SelectedModularPack = null;
            SelectedModularPackGroups = null;
            SelectedModularPackIndex = -1;

            if ( _data != null && _data.SelectedToolData != null )
            {
                _data.SelectedToolData.Pack = null;
                _data.SelectedToolData.PackGUID = null;
            }

            ResetSelectedGroupData( );
        }

        private static void ResetSelectedGroupData( )
        {
            SelectedGroup = null;
            SelectedGroupSize = Vector3.zero;
            SelectedGroupIndex = 0;

            if ( _data != null && _data.SelectedToolData != null )
            {
                _data.SelectedToolData.Group = null;
                _data.SelectedToolData.GroupGuid = null;
            }

            ResetSelectedModuleData( );
        }

        private static void ResetSelectedModuleData( )
        {
            SelectedModule = null;
            SelectedModuleSize = Vector3.zero;
            SelectedModuleIndex = 0;
        }

        #endregion


        #region public _Getters

        private static ModularPack GetModularPackByGuid( string guid )
        {
            if ( string.IsNullOrEmpty( guid ) )
                return null;

            var modularPack = AllModularPacks.FirstOrDefault( i => i.Guid == guid );

            return modularPack;
        }

        private static ModularPack GetModularPackByIndex( int index )
        {
            if ( index < 0 )
            {
                return null;
            }


            var modularPack = AllModularPacks.ElementAtOrDefault( index );

            if ( modularPack == null ) Debug.LogError( $"Can't get modular pack at index {index}" );

            return modularPack;
        }

        private static ModularGroup GetModularGroupByGuid( string guid )
        {
            if ( string.IsNullOrEmpty( guid ) )
                return null;

            var assetGroup = SelectedModularPackGroups.FirstOrDefault( i => i.Guid == guid );

            return assetGroup;
        }

        private static ModularGroup GetAssetGroupByIndex( int index )
        {
            if ( index < 0 ) return null;

            var modularGroup = SelectedModularPackGroups?.ElementAtOrDefault( index );

            return modularGroup;
        }

        #endregion

        #region CALLBACKS

        public delegate void PackChangeCallback( ModularPack pack );

        private static readonly List<PackChangeCallback> _onPackChange = new List<PackChangeCallback>( );

        public delegate void GroupChangeCallback( ModularGroup group, Vector3 size );

        private static readonly List<GroupChangeCallback> _onGroupChange = new List<GroupChangeCallback>( );

        public delegate void ModuleChangeCallback( Module module, Vector3 size );

        private static readonly List<ModuleChangeCallback> _onModuleChange = new List<ModuleChangeCallback>( );


        public static void AddPackChangeCallback( PackChangeCallback callback )
        {
            _onPackChange.Add( callback );
        }

        public static void AddGroupChangeCallback( GroupChangeCallback callback )
        {
            _onGroupChange.Add( callback );
        }

        public static void AddModuleChangeCallback( ModuleChangeCallback callback )
        {
            _onModuleChange.Add( callback );
        }

        public static void ClearCallbacks( )
        {
            _onPackChange.Clear( );
            _onGroupChange.Clear( );
            _onModuleChange.Clear( );
        }

        #endregion

        public static void SelectRandomModule( )
        {
            if ( SelectedGroup == null )
                return;

            if ( SelectedGroup.CastedModules is null || SelectedGroup.CastedModules.Length == 0 ||
                 SelectedGroup.CastedModules.Length == 1 )
                return;

            if ( SelectedGroup.CastedModules.Length is 2 )
            {
                var nextIndex =
                    Collections_Helper.GetNextLoopIndex( SelectedModuleIndex, SelectedGroup.CastedModules.Length );

                SetupModule( nextIndex, SelectedGroup );
                return;
            }

            var currentIndex = SelectedModuleIndex;
            var minIndex = 0;
            var maxIndex = SelectedGroup.CastedModules.Length;

            var randomIndex = Random.Range( minIndex, maxIndex );

            if ( randomIndex == currentIndex )
                while ( randomIndex == currentIndex )
                {
                    randomIndex = Random.Range( minIndex, maxIndex );
                }

            SetupModule( randomIndex, SelectedGroup );
        }

        public static void SelectNextModule( )
        {
            if ( SelectedGroup == null )
                return;

            if ( SelectedGroup.CastedModules is null || SelectedGroup.CastedModules.Length == 0 ||
                 SelectedGroup.CastedModules.Length == 1 )
                return;


            var currentIndex = SelectedModuleIndex;
            var nextIndex = Collections_Helper.GetNextLoopIndex( currentIndex, SelectedGroup.CastedModules.Length );

            SetupModule( nextIndex, SelectedGroup );
        }
    }
}

#endif