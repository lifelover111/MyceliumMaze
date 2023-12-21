#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using MBS.Model.Configuration;
using UnityEditor;
using UnityEngine;

namespace MBS.Utilities.Helpers
{
    public  static class GameObjectHelper
    {
        public  static void DestroyImmediate( this GameObject gameObject )
        {
            Object.DestroyImmediate( gameObject );
        }

        public  static void DestroyImmediateUndo( this GameObject gameObject )
        {
            Undo.DestroyObjectImmediate( gameObject );
        }

        public  static void RecordCreatedUndo( this GameObject gameObject, string undoText = null )
        {
            string text = ( string.IsNullOrEmpty( undoText ) ) ? gameObject.name : undoText;
            Undo.RegisterCreatedObjectUndo( gameObject, text );
        }
        
        public  static bool IsPrefab( GameObject gameObject )
        {
            return gameObject.scene.rootCount == 0;
        }

         
         
         
         
         
         
        public  static Texture2D GetPreviewTexture( GameObject prefab )
        {
             
            if ( prefab == null ) return null;

            var maxIterations = Config.Sgt.GetAssetPreviewMaxAttempts;


            var retvalTexture = AssetPreview.GetAssetPreview( prefab );


            if ( retvalTexture == null )
            {
                var instanceID = prefab.GetInstanceID( );


                 
                for ( var iteration = 0; iteration < maxIterations; iteration++ )
                {
                    if ( AssetPreview.IsLoadingAssetPreview( instanceID ) )
                        continue;

                    retvalTexture = AssetPreview.GetAssetPreview( prefab );
                    break;
                }
            }

            return retvalTexture;
        }

        public  static string GetUniqueName( string name )
        {
            var allGameObjectNames = Object.FindObjectsOfType<GameObject>( true ).Select( i => i.name ).ToArray( );
            var uniqueName = ObjectNames.GetUniqueName( allGameObjectNames, name );
            return uniqueName;
        }

        public  static Vector3 GetSize( GameObject prefab )
        {
            var mesh = GetPrefabCombinedMesh( prefab );

            if ( mesh == null )
            {
                Debug.LogError( "MBS. GameObject Helper. Cannot get prefab mesh size. \n. " +
                                "This error can occur if your prefab doesn't have a MeshFilter or Mesh attached to it." );
                return Vector3.zero;
            }

            return mesh.bounds.size;
        }

        public  static Mesh GetPrefabCombinedMesh( GameObject prefab )
        {
            if ( prefab == null )
                return null;

            var childrenMF = prefab.GetComponentsInChildren<MeshFilter>( );
            Mesh combinedMesh = null;

            var tempFilters = childrenMF.ToList( );

            tempFilters = tempFilters.Where( i => i != null && i.sharedMesh != null ).ToList( );

            if ( tempFilters.Count > 0 )
            {
                var combineInstances = new List<CombineInstance>( tempFilters.Count * 5 );

                for ( var i = 0; i < tempFilters.Count; i++ )
                {
                    var sharedMesh = tempFilters[ i ].sharedMesh;
                    var transformMatrix = tempFilters[ i ].transform.localToWorldMatrix;
                    for ( int y = 0; y < sharedMesh.subMeshCount; y++ )
                    {
                        var ci = new CombineInstance
                        {
                            mesh = sharedMesh,
                            subMeshIndex = y,
                            transform = transformMatrix
                        };
                        combineInstances.Add( ci );
                    }
                }

                combinedMesh = new Mesh( );
                combinedMesh.CombineMeshes( combineInstances.ToArray( ) );
            }

            return combinedMesh;
        }
    }
}

#endif