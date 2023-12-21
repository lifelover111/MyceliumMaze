#if UNITY_EDITOR

using MBS.Controller.Configuration;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Model.Configuration
{
    public  class InternalDataManager : ScriptableObject, ISingleton
    {
        private static InternalDataManager _singleton;

        [SerializeField] private Mesh _wallGizmoMesh;
        [SerializeField] private Material _checkboardMaterial;

        public  static InternalDataManager Singleton =>
            _singleton = SingletonHelper.GetSingleton( _singleton, nameof( InternalDataManager ) );


        public  Mesh WallGizmoMesh => _wallGizmoMesh;
        
        public  Material CheckboardMaterial => _checkboardMaterial;


        public  void ColdInitialization( )
        {
            _checkboardMaterial =
                AssetDatabase.LoadAssetAtPath<Material>( PathController.GetPATH_CheckboardMaterial( ) );
            _wallGizmoMesh = AssetDatabase.LoadAssetAtPath<Mesh>( PathController.GetPATH_WallGizmoMesh( ) );
        }

        public  void WarmInitialization( )
        {
        }
    }
}
#endif