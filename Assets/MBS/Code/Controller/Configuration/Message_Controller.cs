#if UNITY_EDITOR

using MBS.Model.Configuration;

namespace MBS.Controller.Configuration
{
    public  static class MessageController
    {
        private const string NEXT_LINE_ELEMENT = "\n";


        public  static string TextValidation_LengthLess2( string textObjectName )
        {
            return string.Format( Texts.AssetSystem.Text.VALIDATION_LENGTH_LESS_THAN2, textObjectName ) +
                   NEXT_LINE_ELEMENT;
        }

        public  static string TextValidation_InapproppriateCharacters( string textObjectName )
        {
            return string.Format( Texts.AssetSystem.Text.VALIDATION_WRONG_CHARACTERS, textObjectName ) +
                   NEXT_LINE_ELEMENT;
        }


         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         


        public  static string GROUP_ModulesListEmpty( string windowName, string assetName )
        {
            return string.Format( Texts.AssetSystem.Group.MODULES_LIST_EMPTY, windowName, assetName ) +
                   NEXT_LINE_ELEMENT;
        }


        public  static string MODULE_Wall_Info( )
        {
            return Texts.AssetSystem.Module.WALL_INFO;
        }

        public  static string MODULE_Floor_Info( )
        {
            return Texts.AssetSystem.Module.FLOOR_INFO;
        }

        public  static string MODULE_ObjectNotPrefab( string windowName, string assetName, string fieldName )
        {
            return string.Format( Texts.AssetSystem.Module.OBJECT_NOT_PREFAB, windowName, assetName, fieldName ) +
                   NEXT_LINE_ELEMENT;
        }

        public  static string MODULE_ONSAVE_FieldValueIsNull( string windowName, string assetName, string fieldName )
        {
            return string.Format( Texts.AssetSystem.Module.FIELD_VALUE_NULL, windowName, assetName, fieldName ) +
                   NEXT_LINE_ELEMENT;
        }
    }
}
#endif