#if UNITY_EDITOR

using MBS.Controller.Scene.Mono;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    [ExecuteInEditMode]
    public  class MBSWallModuleModifier : EditorBehaviour
    {
        [SerializeField] private MBSWallModule _root;

        [SerializeField] public  Mesh originalMesh;
        [SerializeField] public  bool doModify = true;
        [SerializeField] public  MeshFilter meshFilter;
        [SerializeField] public  MeshCollider meshCollider;


        public  MBSWallModule Root
        {
            get
            {
                if ( _root == null )
                    _root = GetComponentInParent<MBSWallModule>( );
                return _root;
            }
            set => _root = value;
        }

        public  void SetupMesh( Mesh mesh = null )
        {
            if ( mesh == null )
                mesh = originalMesh;

            if ( meshFilter != null )
                meshFilter.mesh = mesh;

            if ( meshCollider != null )
                meshCollider.sharedMesh = mesh;
        }

        public  void WhenPlaced( MBSWallModule root, MeshFilter meshFilter )
        {
            Root = root;
            originalMesh = meshFilter.sharedMesh;

            this.meshFilter = GetComponent<MeshFilter>( );
            meshCollider = GetComponent<MeshCollider>( );
            if ( meshCollider == null )
                meshCollider = gameObject.AddComponent<MeshCollider>( );

            EditorUtility.SetDirty( this );
        }
    }
}
#endif