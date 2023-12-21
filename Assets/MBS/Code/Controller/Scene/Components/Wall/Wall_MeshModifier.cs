#if UNITY_EDITOR

using System;
using System.Linq;
using MBS.Controller.Scene.Mono;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBS.Controller.Scene
{
    [Serializable]
    public  class Wall_MeshModifier
    {
        private static Color _noAffectColor = Color.black;


        public  static Mesh ModifyMesh( MBSWallModule mbsWallModule, MBSWallModuleModifier moduleModifier )
        {
            var originalMesh = moduleModifier.originalMesh;

            var frontMod = mbsWallModule.frontModification;
            var rearMod = mbsWallModule.rearModification;
            var frontEndPointWallLocal = mbsWallModule.frontEndPointLocalSpace;
            var rearEndPointWallLocal = mbsWallModule.rearEndPointLocalSpace;

            var rootObject = mbsWallModule.transform;
            var curObject = moduleModifier.transform;

            var doRequireToScale = !mbsWallModule.additionalScale.ApxEquals( 1 )
                                || !mbsWallModule.fitScale.ApxEquals( 1 )
                                || !mbsWallModule.data.originalPrefab.transform.localScale.x.ApxEquals( 1 );


            var hasVertexColors = originalMesh.colors != null && originalMesh.colors.Length > 0;

            if ( originalMesh == null )
            {
                Debug.LogError( Texts.Building.WallMeshModifier.ORIGINAL_MESH_MISSING );
                return null;
            }

            if ( originalMesh.vertices == null || originalMesh.vertices.Length == 0 )
            {
                Debug.LogError( Texts.Building.WallMeshModifier.ORIGINAL_MESH_VERTICES_EMPTY );
                return null;
            }

            if ( originalMesh.colors == null || originalMesh.colors.Length == 0 ) hasVertexColors = false;

            if ( !doRequireToScale && frontMod.angle == 0 && rearMod.angle == 0 ) return originalMesh;

            Mesh currentMesh;

            if ( doRequireToScale )
            {
                var totalScale = mbsWallModule.data.originalPrefab.transform.lossyScale;
                totalScale = totalScale.MultiplyByVector3_XXYYZZ( new Vector3( mbsWallModule.additionalScale, 1, 1 ) );
                totalScale = totalScale.MultiplyByVector3_XXYYZZ( new Vector3( mbsWallModule.fitScale, 1, 1 ) );

                currentMesh = ScaleMeshWithoutCheck( originalMesh, hasVertexColors, totalScale );
            }
            else
            {
                currentMesh = Object.Instantiate( originalMesh );
            }


            var vertices = currentMesh.vertices;
            var colors = currentMesh.colors;


            if ( hasVertexColors )
            {
                if ( rootObject != curObject )
                {
                    vertices = vertices.Select( i => curObject.transform.TransformPoint( i ) ).ToArray( );
                    vertices = vertices.Select( i => rootObject.transform.InverseTransformPoint( i ) ).ToArray( );
                }

                var sidesColors = GetNearVertColor( frontEndPointWallLocal, rearEndPointWallLocal, vertices, colors );
                float connectionAngle = 0;
                float sign = 0;

                Vector3 curVector = default;

                var frontVector = GetConjuctionVector( rootObject, frontMod, frontEndPointWallLocal );
                var rearVector = GetConjuctionVector( rootObject, rearMod, rearEndPointWallLocal );

                for ( var v = 0; v < vertices.Length; v++ )
                {
                    if ( colors[ v ] == Color.white || colors[ v ] == _noAffectColor ) continue;

                     
                     
                    if ( colors[ v ] == sidesColors.frontColor )
                    {
                        connectionAngle = frontMod.angle;

                        if ( vertices[ v ].z > 0 )
                            sign = frontMod.positiveSide;
                        else if ( vertices[ v ].z < 0 )
                            sign = frontMod.negativeSide;
                        else if ( vertices[ v ].z == 0 )
                            sign = 0;

                        curVector = frontVector;
                    }
                    else if ( colors[ v ] == sidesColors.rearColor )
                    {
                        connectionAngle = rearMod.angle;
                        if ( vertices[ v ].z > 0 )
                            sign = -rearMod.positiveSide;
                        else if ( vertices[ v ].z < 0 )
                            sign = -rearMod.negativeSide;
                        else if ( vertices[ v ].z == 0 )
                            sign = 0;

                        curVector = rearVector;
                    }

                    if ( connectionAngle == 0 )
                        continue;

                    var adjV = Vector3.forward * Mathf.Abs( vertices[ v ].z );
                    var angle = Vector3.Angle( adjV, curVector );
                    var hyp = adjV.magnitude / Mathf.Cos( angle * Mathf.Deg2Rad );
                    var opp = Mathf.Sin( angle * Mathf.Deg2Rad ) * hyp;

                    vertices[ v ].x += opp * sign;
                }

                if ( rootObject != curObject )
                {
                    vertices = vertices.Select( i => rootObject.transform.TransformPoint( i ) ).ToArray( );
                    vertices = vertices.Select( i => curObject.transform.InverseTransformPoint( i ) ).ToArray( );
                }
            }

            currentMesh.name = "MBSWallModifiedMesh";
            currentMesh.SetVertices( vertices );
            currentMesh.RecalculateBounds( );
            return currentMesh;
        }

        private static Vector3 GetConjuctionVector( Transform rootObject, WallSideModification sideMod,
                                                    Vector3 localEndPoint )
        {
            var transform = rootObject.transform;
            var curEndPointWorld = transform.TransformPoint( localEndPoint );
            var curAdjecentDirWorld = sideMod.abVector;
            var hyp1Direction = curEndPointWorld - transform.position;

            var angle = Vector3.Angle( hyp1Direction, curAdjecentDirWorld );

            var adj1Magn = MathF.Cos( angle * Mathf.Deg2Rad ) * hyp1Direction.magnitude;
            var adj1Direction = curAdjecentDirWorld.normalized * adj1Magn;
            var adj1PointWorld = transform.position + adj1Direction;

            var opp1Vector = curEndPointWorld - adj1PointWorld;

            opp1Vector = rootObject.InverseTransformDirection( opp1Vector.normalized );

            Vector3 retval = new Vector3( Mathf.Abs( opp1Vector.normalized.x ), Mathf.Abs( opp1Vector.normalized.y ),
                                          Mathf.Abs( opp1Vector.normalized.z ) );
            return retval;
        }


         
         
         
        private static (Color frontColor, Color rearColor) GetNearVertColor( Vector3 localPoint1, Vector3 localPoint2,
                                                                             Vector3[ ] vertecies, Color[ ] colors )
        {
            var minDistance1 = float.MaxValue;
            var minDistance2 = float.MaxValue;
            var retval1 = Color.black;
            var retval2 = Color.black;

            for ( var i = 0; i < vertecies.Length; i++ )
            {
                if ( colors[ i ] == Color.white )
                    continue;
                if ( colors[ i ] == _noAffectColor )
                    continue;

                var distnace1 = ( localPoint1 - vertecies[ i ] ).sqrMagnitude;
                if ( distnace1 < minDistance1 )
                {
                    minDistance1 = distnace1;
                    retval1 = colors[ i ];
                }

                var distnace2 = ( localPoint2 - vertecies[ i ] ).sqrMagnitude;
                if ( distnace2 < minDistance2 )
                {
                    minDistance2 = distnace2;
                    retval2 = colors[ i ];
                }
            }

            return ( retval1, retval2 );
        }


        public  static void ScaleWall( MBSWallModule mbsWallModule, Vector3 scaleVector )
        {
            for ( var i = 0; i < mbsWallModule.meshModifiers.Count; i++ )
            {
                var modifier = mbsWallModule.meshModifiers[ i ];

                if ( !modifier.doModify )
                {
                    modifier.SetupMesh( );
                }
                else
                {
                    var scaledMesh = ScaleMesh( modifier.originalMesh, scaleVector );
                    modifier.SetupMesh( scaledMesh );
                }
            }
        }

        public  static Mesh ScaleMesh( Mesh originalMesh, Vector3 scaleVector )
        {
            if ( originalMesh == null )
            {
                Debug.LogError( Texts.Building.WallMeshModifier.ORIGINAL_MESH_MISSING );
                return null;
            }

            if ( originalMesh.vertices == null || originalMesh.vertices.Length == 0 )
            {
                Debug.LogError( Texts.Building.WallMeshModifier.ORIGINAL_MESH_VERTICES_EMPTY );
                return null;
            }

            var hasVertexColors = !( originalMesh.colors == null || originalMesh.colors.Length == 0 );

            return ScaleMeshWithoutCheck( originalMesh, hasVertexColors, scaleVector );
        }


        private static Mesh ScaleMeshWithoutCheck( Mesh originalMesh, bool hasVertexColors, Vector3 scaleVector )
        {
            var scaledVertices = originalMesh.vertices.ToArray( );


            if ( hasVertexColors )
                for ( var i = 0; i < scaledVertices.Length; i++ )
                {
                    if ( originalMesh.colors[ i ] == _noAffectColor )
                        continue;

                    scaledVertices[ i ] = new Vector3( scaledVertices[ i ].x * scaleVector.x,
                                                       scaledVertices[ i ].y * scaleVector.y,
                                                       scaledVertices[ i ].z * scaleVector.z );
                }
            else
                for ( var i = 0; i < scaledVertices.Length; i++ )
                    scaledVertices[ i ] = new Vector3( scaledVertices[ i ].x * scaleVector.x,
                                                       scaledVertices[ i ].y * scaleVector.y,
                                                       scaledVertices[ i ].z * scaleVector.z );

            var scaledMesh = Object.Instantiate( originalMesh );
            scaledMesh.name = "MBSScaledMesh";
            scaledMesh.SetVertices( scaledVertices );
            scaledMesh.RecalculateBounds( );

            return scaledMesh;
        }
    }
}

#endif