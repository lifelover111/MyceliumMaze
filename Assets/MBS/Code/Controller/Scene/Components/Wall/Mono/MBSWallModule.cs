#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Code.Builder.Scene;
using MBS.Model.AssetSystem;
using MBS.Model.Configuration;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene.Mono
{
    [Serializable]
    public class WallSideModification
    {
        public float angle;
        public float positiveSide;
        public float negativeSide;
        public Vector3 abVector;

        public WallSideModification( float angle, float positiveSide, float negativeSide, Vector3 abVector )
        {
            this.angle = angle;
            this.positiveSide = positiveSide;
            this.negativeSide = negativeSide;
            this.abVector = abVector;
        }
    }


    [ExecuteInEditMode]
    public class MBSWallModule : EditorBehaviour
    {
        [SerializeField] public AssetData data;

        [NonSerialized] private MBSConstruction _mbsConstruction;
        [NonSerialized] private ModularPack _origPack;
        [NonSerialized] private WallGroup _origGroup;
        [NonSerialized] private WallModule _origModule;

        [SerializeField] public List<MBSWallModuleModifier> meshModifiers;

        [SerializeField] public float additionalScale;
        [SerializeField] public float fitScale;

        [SerializeField] public Vector3 frontEndPointConstructorSpace;
        [SerializeField] public Vector3 rearEndPointConstructorSpace;
        [SerializeField] public Vector3 frontEndPointLocalSpace;
        [SerializeField] public Vector3 rearEndPointLocalSpace;


        [SerializeField] public List<MBSWallModule> frontConnections;
        [SerializeField] public List<MBSWallModule> rearConnections;

        [SerializeField] public WallSideModification frontModification;
        [SerializeField] public WallSideModification rearModification;

        [SerializeField] public MBSWallModule connectedToFront;
        [SerializeField] public MBSWallModule connectedToRear;


        public WallGroup OriginalGroup
        {
            get
            {
                if ( _origGroup != null ) return _origGroup;

                LoadAssetsByGUIDs( );
                return _origGroup;
            }
        }

        public WallModule OriginalModule
        {
            get
            {
                if ( _origModule != null ) return _origModule;

                LoadAssetsByGUIDs( );
                return _origModule;
            }
        }


        public Vector3 FrontEndPointWorldSpace => transform.TransformPoint( frontEndPointLocalSpace );
        public Vector3 RearEndPointWorldSpace => transform.TransformPoint( rearEndPointLocalSpace );


        internal MBSConstruction MbsConstruction
        {
            get
            {
                if ( _mbsConstruction == null )
                    _mbsConstruction = transform.GetComponentInParent<MBSConstruction>( );

                return _mbsConstruction;
            }
        }


        private void OnEnable( )
        {
            if ( BuildPipeline.isBuildingPlayer == true )
                return;


            if ( DragAndDrop.objectReferences.Length > 0 )
                return;

            if ( data == null )
                return;


            if ( MbsConstruction != null && MbsConstruction.IsWallAlreadyAdded( this ) == false )
            {
                frontConnections = new List<MBSWallModule>( 8 );
                rearConnections = new List<MBSWallModule>( 8 );

                MbsConstruction.AddWallAndConnect( this );

                WallConnectionController.RecalculateConnectionNodes( this );

                MbsConstruction.AddAreasIfFound( this );
                MbsConstruction.UpdateEndPoints( );
                MbsConstruction.UpdateAreas( );
            }
        }

        private void OnDisable( )
        {
            if ( BuildPipeline.isBuildingPlayer == true )
                return;

            if ( DragAndDrop.objectReferences.Length > 0 )
                return;

            if ( data == null )
                return;

            if ( MbsConstruction != null )
            {
                _mbsConstruction.RemoveWallAndDisconnect( this );
                _mbsConstruction.RemoveAreasWithWall( this );

                var wallToUpdate = new List<MBSWallModule>( );

                if ( frontConnections != null && frontConnections.Count > 0 )
                    wallToUpdate.Add( frontConnections.ElementAtOrDefault( 0 ) );

                if ( rearConnections != null && rearConnections.Count > 0 )
                    wallToUpdate.Add( rearConnections.ElementAtOrDefault( 0 ) );

                for ( var i = 0; i < wallToUpdate.Count; i++ )
                    if ( wallToUpdate[ i ] != null )
                        WallConnectionController.RecalculateConnectionNodes( wallToUpdate[ i ] );

                ClearConnections( );
            }
        }

        private void OnDrawGizmosSelected( )
        {
        }

        protected override void OnOriginalDestroy( )
        {
            if ( MbsConstruction != null )
            {
                _mbsConstruction.RemoveWallAndDisconnect( this );
                _mbsConstruction.RemoveAreasWithWall( this );
                _mbsConstruction.UpdateEndPoints( );
                ClearConnections( );
            }
        }

        protected override void OnExternalDestroy( )
        {
        }


        internal void WhenPlaced( MBSConstruction mbsConstruction, float additionalScale, float fitScale,
                                  ModularPack pack, WallGroup group, WallModule module, GameObject originalPrefab )
        {
            tag = PredefinedTags.EDITING_WALL;
            _mbsConstruction = mbsConstruction;

            _origPack = pack;
            _origGroup = group;
            _origModule = module;

            data = new AssetData( );
            data.packGuid = pack.Guid;
            data.groupGuid = group.Guid;
            data.moduleGuid = module.Guid;
            data.originalPrefab = originalPrefab;
            data.originalSize = GameObjectHelper.GetSize( originalPrefab );
            data.actualSize = data.originalSize;

            frontModification = new WallSideModification( 0, 0, 0, default );
            rearModification = new WallSideModification( 0, 0, 0, default );

            frontConnections = new List<MBSWallModule>( 8 );
            rearConnections = new List<MBSWallModule>( 8 );

            WhenPlaced_Children( );

            this.additionalScale = additionalScale;
            this.fitScale = fitScale;

            if ( additionalScale != 1 || fitScale != 1 )
            {
                data.actualSize.x = data.originalSize.x * additionalScale * fitScale;
                data.actualSize.y = data.originalSize.y;
                data.actualSize.z = data.originalSize.z;
            }

            UpdateEndPoints( );
        }


        private void WhenPlaced_Children( )
        {
            meshModifiers = new List<MBSWallModuleModifier>( );

            if ( transform.TryGetComponent( out MeshFilter meshFilter ) )
                if ( !transform.TryGetComponent( out MBSWallModuleModifier meshModifier ) )
                {
                    meshModifier = transform.gameObject.AddComponent<MBSWallModuleModifier>( );
                    meshModifier.WhenPlaced( this, meshFilter );

                    meshModifiers.Add( meshModifier );
                }

            transform.DoRecursive( t =>
            {
                if ( t.TryGetComponent( out MeshFilter meshFilter ) )
                    if ( !t.TryGetComponent( out MBSWallModuleModifier meshModifier ) )
                    {
                        meshModifier = t.gameObject.AddComponent<MBSWallModuleModifier>( );
                        meshModifier.WhenPlaced( this, meshFilter );

                        meshModifiers.Add( meshModifier );
                    }
            } );
        }


        private void LoadAssetsByGUIDs( )
        {
            _origPack = ModularPack_Manager.Singleton.ModularPacks
                                           .FirstOrDefault( i => i.Guid == data.packGuid );

            if ( _origPack == null )
            {
                Debug.LogError( Texts.Component.Wall.CANNOT_LOAD_PACK );
                return;
            }

            _origGroup = _origPack.WallGroups.FirstOrDefault( i => i.Guid == data.groupGuid );

            if ( _origGroup == null )
            {
                Debug.LogError( Texts.Component.Wall.CANNOT_LOAD_GROUP );
                return;
            }

            _origModule = _origGroup.Modules.FirstOrDefault( i => i.Guid == data.moduleGuid );

            if ( _origModule == null )
            {
                Debug.LogError( Texts.Component.Wall.CANNOT_LOAD_MODULE );
            }
        }


        private void UpdateEndPoints( )
        {
            transform.localScale = Vector3.one;

            var frontWorld = transform.position + data.actualSize.x / 2 * transform.right;
            frontEndPointLocalSpace = transform.InverseTransformPoint( frontWorld );
            frontEndPointConstructorSpace = MbsConstruction.transform.InverseTransformPoint( frontWorld );

            var rearWorld = transform.position - data.actualSize.x / 2 * transform.right;
            rearEndPointLocalSpace = transform.InverseTransformPoint( rearWorld );
            rearEndPointConstructorSpace = MbsConstruction.transform.InverseTransformPoint( rearWorld );
        }


        public void ResetSideModifications( )
        {
            ResetSideModification( frontEndPointConstructorSpace );
            ResetSideModification( rearEndPointConstructorSpace );
        }

        public void ResetSideModification( Vector3 connectionPointConstr )
        {
            if ( MbsConstruction == null )
                return;

            if ( connectionPointConstr.ApxEquals( frontEndPointConstructorSpace ) )
            {
                frontModification = new WallSideModification( 0, 0, 0, default );
            }
            else if ( connectionPointConstr.ApxEquals( rearEndPointConstructorSpace ) )
            {
                rearModification = new WallSideModification( 0, 0, 0, default );
            }
            else
            {
                Debug.LogErrorFormat( Texts.Component.Wall.WRONG_CONNECTION_POINT,
                                      connectionPointConstr, frontEndPointConstructorSpace,
                                      rearEndPointConstructorSpace );
            }
        }

        public WallSideModification GetModificationAt( Vector3 connectionPointConstr )
        {
            if ( MbsConstruction == null )
                return null;

            if ( connectionPointConstr.ApxEquals( frontEndPointConstructorSpace ) ) return frontModification;

            if ( connectionPointConstr.ApxEquals( rearEndPointConstructorSpace ) )
            {
                return rearModification;
            }

            Debug.LogErrorFormat( Texts.Component.Wall.WRONG_CONNECTION_POINT,
                                  connectionPointConstr, frontEndPointConstructorSpace, rearEndPointConstructorSpace );
            return null;
        }

        public void AddFrontConnection( MBSWallModule mbsWallModule )
        {
            if ( mbsWallModule == null || mbsWallModule == this ) return;

            frontConnections.Add( mbsWallModule );
        }

        public void AddRearConnection( MBSWallModule mbsWallModule )
        {
            if ( mbsWallModule == null || mbsWallModule == this ) return;

            rearConnections.Add( mbsWallModule );
        }


        public void RemoveConnection( MBSWallModule mbsWallModule )
        {
            frontConnections.Remove( mbsWallModule );
            rearConnections.Remove( mbsWallModule );
        }

        public void ClearConnections( )
        {
            frontConnections = new List<MBSWallModule>( 8 );
            rearConnections = new List<MBSWallModule>( 8 );
        }


        public MBSWallModule ChangeModule( WallModule newModule, bool multipleSelection )
        {
            if ( MbsConstruction == null )
            {
                Debug.LogError( Texts.Component.Wall.CONSTRUCTION_MISSING );
                return null;
            }

            var chosen = WallPrefabContoller.ChangeModule_ChosePrefab(
                OriginalModule,
                newModule,
                data.originalPrefab,
                additionalScale );

            var chosenPrefab = chosen.chosenPrefab;


            var newName = chosenPrefab.name;
            if ( transform.parent != null )
            {
                var existedNames = transform.parent.GetComponentsInChildren<Transform>( )
                                            .Select( i => i.gameObject.name );
                existedNames = existedNames.Where( i => i != gameObject.name );
                newName = ObjectNames.GetUniqueName( existedNames.ToArray( ), chosenPrefab.name );
            }


            var changedPrefab = (GameObject)PrefabUtility.InstantiatePrefab( chosenPrefab );
            changedPrefab.transform.SetParent( transform.parent );
            changedPrefab.transform.localPosition = transform.localPosition;
            changedPrefab.transform.localRotation = transform.localRotation;

            var totalScale = chosen.chosenPrefab.transform.localScale;
            totalScale = totalScale.MultiplyByVector3_XXYYZZ( chosen.additionalScale );
            totalScale = totalScale.MultiplyByVector3_XXYYZZ( new Vector3( fitScale, 1, 1 ) );
            changedPrefab.transform.localScale = totalScale;

            Undo.IncrementCurrentGroup( );
            Undo.SetCurrentGroupName( Texts.Component.Wall.MODULE_CHANGED_UNDO_NAME );
            changedPrefab.RecordCreatedUndo( );

            var changedWall = changedPrefab.AddComponent<MBSWallModule>( );
            changedWall.tag = tag;
            changedWall.name = newName;

            changedWall.WhenPlaced( MbsConstruction, chosen.additionalScale.x, fitScale, _origPack, _origGroup,
                                    newModule,
                                    chosenPrefab );

            if ( multipleSelection )
            {
                MbsConstruction.AddWallAndConnect( changedWall );
                MbsConstruction.AddAreasIfFound( changedWall );
                changedPrefab.AddToSelection( );
                gameObject.DestroyImmediateUndo( );
            }
            else
            {
                _mbsConstruction.RemoveWallAndDisconnect( this );
                _mbsConstruction.RemoveAreasWithWall( this );
                _mbsConstruction.UpdateEndPoints( );

                MbsConstruction.AddWallAndConnect( changedWall );

                MbsConstruction.AddAreasIfFound( changedWall );
                MbsConstruction.UpdateAreas( );

                changedPrefab.AddToSelection( );

                gameObject.DestroyImmediateUndo( );
            }

            return changedWall;
        }


        public void LockConnectionWith( MBSWallModule mbsWallModule )
        {
            if ( frontConnections.Contains( mbsWallModule ) )
            {
                if ( connectedToFront != null )
                {
                    connectedToFront.UnlockConnectionWith( this );
                    connectedToFront = mbsWallModule;
                }
                else
                {
                    connectedToFront = mbsWallModule;
                }
            }
            else if ( rearConnections.Contains( mbsWallModule ) )
            {
                if ( connectedToRear != null )
                {
                    connectedToRear.UnlockConnectionWith( this );
                    connectedToRear = mbsWallModule;
                }
                else
                {
                    connectedToRear = mbsWallModule;
                }
            }
        }

        public void UnlockConnectionWith( MBSWallModule mbsWallModule )
        {
            if ( connectedToFront == mbsWallModule )
                connectedToFront = null;
            else if ( connectedToRear == mbsWallModule )
                connectedToRear = null;
        }

        public void ResetLockedConnections( )
        {
            ResetFrontLockedConnection( );
            ResetRearLockedConnection( );
            WallConnectionController.RecalculateConnectionNodes( this );
        }

        public void ResetFrontLockedConnection( )
        {
            if ( connectedToFront != null )
            {
                if ( connectedToFront.connectedToFront == this )
                {
                    connectedToFront.connectedToFront = null;
                    connectedToFront = null;
                }
                else if ( connectedToFront.connectedToRear == this )
                {
                    connectedToFront.connectedToRear = null;
                    connectedToFront = null;
                }

                WallConnectionController.RecalculateConnectionNodes( this );
            }
        }

        public void ResetRearLockedConnection( )
        {
            if ( connectedToRear != null )
            {
                if ( connectedToRear.connectedToFront == this )
                {
                    connectedToRear.connectedToFront = null;
                    connectedToRear = null;
                }
                else if ( connectedToRear.connectedToRear == this )
                {
                    connectedToRear.connectedToRear = null;
                    connectedToRear = null;
                }

                WallConnectionController.RecalculateConnectionNodes( this );
            }
        }


        public void FlipFace( )
        {
            MbsConstruction.RemoveWallAndDisconnect( this );
            ClearConnections( );

            transform.Rotate( 0, 180, 0 );

            UpdateEndPoints( );
            MbsConstruction.AddWallAndConnect( this );

            ( connectedToFront, connectedToRear ) = ( connectedToRear, connectedToFront );

            WallConnectionController.RecalculateConnectionNodes( this );
        }

        public void TurnWallInsideArea( RoomArea area, bool turnInside )
        {
            var checkDistance = 0.2f;
            var facing = transform.position - transform.forward * checkDistance;
            var facedFromArea = area.IsPointInsideArea( MbsConstruction.transform.InverseTransformPoint( facing ) );

            if ( turnInside )
            {
                if ( facedFromArea == true )
                {
                    transform.Rotate( 0, 180, 0 );
                    UpdateEndPoints( );
                }
            }
            else
            {
                if ( facedFromArea == false)
                {
                    transform.Rotate( 0, 180, 0 );
                    UpdateEndPoints( );
                }
            }
        }

        public bool IsFacesInsideArea( RoomArea area )
        {
            var checkDistance = 0.2f;
            var ofFacePos = transform.position - transform.forward * checkDistance;
            var isRearInsideArea = area.IsPointInsideArea( MbsConstruction.transform.InverseTransformPoint( ofFacePos ) );
            return !isRearInsideArea;
        }

        public bool IsThereLockedConnectionAt( Vector3 connectionPointConstr )
        {
            if ( connectionPointConstr.ApxEquals( frontEndPointConstructorSpace ) )
                return connectedToFront != null;

            if ( connectionPointConstr.ApxEquals( rearEndPointConstructorSpace ) )
                return connectedToRear != null;

            var frontDist = ( connectionPointConstr - frontEndPointConstructorSpace ).sqrMagnitude;
            var rearDist = ( connectionPointConstr - rearEndPointConstructorSpace ).sqrMagnitude;

            if ( frontDist < rearDist )
                return connectedToFront != null;
            if ( rearDist < frontDist )
                return connectedToRear != null;

            return false;
        }

        [Serializable]
        public class AssetData
        {
            [SerializeField] public string packGuid;
            [SerializeField] public string groupGuid;
            [SerializeField] public string moduleGuid;
            [SerializeField] public GameObject originalPrefab;
            [SerializeField] public Vector3 originalSize;
            [SerializeField] public Vector3 actualSize;
        }
    }
}
#endif