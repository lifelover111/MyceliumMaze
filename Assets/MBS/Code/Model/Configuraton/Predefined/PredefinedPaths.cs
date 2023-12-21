#if UNITY_EDITOR

namespace MBS.Model.Configuration
{
    public  static class PredefinedNames
    {
         
        public  const string MBS_FOLDER = "MBS";
        public  const string MODULAR_PACKS_FOLDER = "ModularPacks";
        public  const string HIDDEN_PACKS_FOLDER = "__HiddenPacks__";
        public  const string CODE_FOLDER = "Code";
        public  const string TEMP_DATA_FOLDER = "TempData";
        public  const string INTERNAL_DATA_PATH = "InternalData";

        public  const string COMBINER_MODULAR_PACK_GUID = "CombinedModularPackGUID";

        public  const string DESCRIPTOR_NAME = "mbs-pack-descriptor";
        public  const string DESCRIPTOR_FILE_NAME = "mbs-pack-descriptor.asset";
    }

    public  static class PredefinedPaths
    {
         
        public  static readonly string MODULAR_PACKS_PATH = "/" + PredefinedNames.MODULAR_PACKS_FOLDER;
        public  static readonly string HIDDEN_PACKS_PATH = "/" + PredefinedNames.HIDDEN_PACKS_FOLDER;
        public  static readonly string CODE_PATH = "/" + PredefinedNames.CODE_FOLDER;
        public  static readonly string TEMP_DATA_PATH = "/" + PredefinedNames.TEMP_DATA_FOLDER;
        public  static readonly string INTERNAL_DATA_PATH = "/" + PredefinedNames.INTERNAL_DATA_PATH;

         
        private const string UIToolkit = "/UIToolkit";

         
        public  const string MANAGER_WINDOW = UIToolkit + "/PackManager/PackManager_Window.uxml";

        public  const string WALL_GROUP_EDITOR_WINDOW = UIToolkit + "/PackManager/WallGroupEditor_Window.uxml";
        public  const string WALL_MODULE_EDITOR_WINDOW = UIToolkit + "/PackManager/WallModuleEditor_Window.uxml";

        public  const string FLOOR_GROUP_EDITOR_WINDOW = UIToolkit + "/PackManager/FloorGroupEditor_Window.uxml";
        public  const string FLOOR_MODULE_EDITOR_WINDOW = UIToolkit + "/PackManager/FloorModuleEditor_Window.uxml";

        public  const string PLACER_GROUP_EDITOR_WINDOW = UIToolkit + "/PackManager/PlacerGroupEditor_Window.uxml";

        public  const string PLACER_MODULE_EDITOR_WINDOW = UIToolkit + "/PackManager/PlacerModuleEditor_Window.uxml";

        public  const string ASSET_CATEGORIES_EDITOR_WINDOW = UIToolkit + "/PackManager/AssetCategoriesEditor_Window.uxml";

        public  const string LIST_ITEM_ICON_AND_LABEL = UIToolkit + "/PackManager/ListItem_IconAndLabel.uxml";

        public  const string DESCRIPTOR_SUFFIX = "/" + PredefinedNames.DESCRIPTOR_FILE_NAME;
        
         
        public  const string BUILDER_WINDOW_LIGHT = UIToolkit + "/Builder/builder-window_light.uxml";
        public  const string BUILDER_WINDOW_DARK = UIToolkit + "/Builder/builder-window_dark.uxml";
        
        public  const string BUILDER_WINDOW_EMPTY = UIToolkit + "/Builder/Builder_Window_Empty.uxml";
        
         
        public  const string CONSTRUCTION_INSPECTOR = UIToolkit + "/Construction/Construction.uxml";
        public  const string CONSTRUCTION_INSPECTOR_LIST_ITEM = UIToolkit + "/Construction/Construction_ListView_Item.uxml";

         
        public  const string HELPBAR_STYLE_SHEET = UIToolkit + "/Helpbar/Uss/Helpbar_Styles.uss";
        public  const string HELPBAR_PICKER_BLOCK_DARK = UIToolkit + "/Helpbar/float-field-picker-block_dark.uxml";
        public  const string HELPBAR_PICKER_BLOCK_LIGHT = UIToolkit + "/Helpbar/float-field-picker-block_light.uxml";

         
        public  const string BUILDER_DATA = "/BuilderData.asset";
        public  const string INPUT_CONFIG_ASSET = "/InputConfig.asset";

         
        public  const string ARROW_GIZMO_MESH = "/Meshes/MBSGizmo.fbx";
        public  const string CHECKBOARD_MATERIAL = "/Materials/MBSGeneretedMeshCheckboard.mat";

         
        public  const string ICON_EMPTY = "/Icons/icon_empty.png";

         
         
         
         
         
         
         
         
    }
}
#endif