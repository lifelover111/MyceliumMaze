#if UNITY_EDITOR

using System;
using MBS.Controller.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.Configuration
{
    [Serializable]
    public  class Config : ScriptableObject, ISingleton
    {
        private static Config _singleton;


        [SerializeField] public  int GetAssetPreviewMaxAttempts = 10;
        [SerializeField] public  int SceneValueCheckEveryMS = 300;
        [SerializeField] public  float GridCellSizeMinLimit = 0.1f;
        [SerializeField] public  bool LimitPlacementHeightByCurrentLevel = true;
        [SerializeField] public  int DecoratorRaycastOffsetCoefficient = 5;

        [SerializeField] public Color GridPrimeLineColor = new Color( .3f, .3f, .3f, 1f );
        [SerializeField] public Color GridSecondLineColor = new Color( .6f, .6f, .6f, 0.5f );
        [SerializeField] public int GridPrimeLineWidth = 3;
        [SerializeField] public int GridSecondLineWidth = 1;


        public  static Config Sgt => _singleton = SingletonHelper.GetSingleton( _singleton, nameof( Config ) );
        public  InputConfig Input { get; private set; }


        public  void ColdInitialization( )
        {
            SceneValueCheckEveryMS = 100;
            AddTagIfNotExist( PredefinedTags.EDITING_WALL );
            AddTagIfNotExist( PredefinedTags.EDITING_FLOOR );
            LoadInputConfig( );
            var pm = PathsManager.Singleton;
        }

        public  void WarmInitialization( )
        {
            LoadInputConfig( );
        }

        public  static void AddTagIfNotExist( string tag )
        {
            var asset = AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/TagManager.asset" );

            if ( asset != null && asset.Length > 0 )
            {
                var tagManagerSerializedObject = new SerializedObject( asset[ 0 ] );
                {
                    var tags = tagManagerSerializedObject.FindProperty( "tags" );

                    for ( var i = 0; i < tags.arraySize; ++i )
                        if ( tags.GetArrayElementAtIndex( i ).stringValue == tag )
                            return; 
                     
                    tags.InsertArrayElementAtIndex( 0 );
                    tags.GetArrayElementAtIndex( 0 ).stringValue = tag;
                }

                tagManagerSerializedObject.ApplyModifiedProperties( );
                tagManagerSerializedObject.Update( );
            }
            else
            {
                Debug.LogError( Texts.Configuration.CANNOT_ADD_TAGS );
            }
        }


         
         
        private void LoadInputConfig( )
        {
            var inputConfigPath = PathController.GetPATH_InputConfigAsset( );
            Input = AssetDatabase.LoadAssetAtPath<InputConfig>( inputConfigPath );
            if ( Input == null )
            {
                var newInputConfig = CreateInstance<InputConfig>( );
                AssetDatabase.CreateAsset( newInputConfig, inputConfigPath );
                Input = newInputConfig;
            }
        }
    }
}
#endif