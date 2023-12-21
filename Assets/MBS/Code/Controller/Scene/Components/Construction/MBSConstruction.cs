#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using MBS.Controller.Builder;
using MBS.Controller.Scene.Mono;
using MBS.Model.Builder;
using MBS.Utilities.Extensions;
using MBS.Utilities.Helpers;
using MBS.View.Input;
using MBS.View.Scene;
using UnityEditor;
using UnityEngine;

namespace MBS.Controller.Scene
{
    [ExecuteInEditMode]
    internal  class MBSConstruction : MonoBehaviour
    {
        private static MBSConstruction _sCurrent;
        public ConstructionEditor mEditor;

        [SerializeReference] public BuilderData builderData;
        
        [SerializeField] private List<MBSWallModule> _walls;
        [SerializeField] private List<RoomArea> _areas;
        [SerializeField] private List<Vector3> _allEndPoints;
        [SerializeField] private List<Vector3> _allEndPointsForSnaps;
        [SerializeField] private GameObject _placerPreviewObject;

        [NonSerialized] public  Vector3[ ] additionalPointsToDraw;

        public  static MBSConstruction Current
        {
            get
            {
                if ( _sCurrent == null )
                    Builder_Controller.Stop( );

                return _sCurrent;
            }
            set => _sCurrent = value;
        }

        public  List<RoomArea> Areas => _areas;

        private void OnEnable( )
        {
            if ( _placerPreviewObject != null )
                _placerPreviewObject.DestroyImmediateUndo( );
        }

        private void OnDisable( )
        {
            if ( _placerPreviewObject != null )
                _placerPreviewObject.DestroyImmediateUndo( );

            if ( _sCurrent == this )
                Builder_Controller.Stop( );
        }


        public  void Initialize( )
        {
            _walls ??= new List<MBSWallModule>( );
            _areas ??= new List<RoomArea>( );
            _allEndPoints ??= new List<Vector3>( );
            _allEndPointsForSnaps ??= new List<Vector3>( );
        }

        public  void StartEditObject( )
        {
            Initialize( );
            Builder_Controller.Launch( this );
        }

        public  bool IsWallAlreadyAdded( MBSWallModule wall )
        {
            if ( _walls is { Count: > 0 } )
            {
                return _walls.Contains( wall );
            }

            return false;
        }

        public  void AddWallAndConnect( MBSWallModule mbsWallModule )
        {
            var wallsToConnect = IsThereWallToConnect(
                mbsWallModule,
                mbsWallModule.frontEndPointConstructorSpace,
                mbsWallModule.rearEndPointConstructorSpace );

            if ( wallsToConnect.Count > 0 )
                for ( var i = 0; i < wallsToConnect.Count; i++ )
                {
                    if ( mbsWallModule.rearEndPointConstructorSpace ==
                         wallsToConnect[ i ].rearEndPointConstructorSpace )
                    {
                        mbsWallModule.AddRearConnection( wallsToConnect[ i ] );
                        wallsToConnect[ i ].AddRearConnection( mbsWallModule );
                    }
                    else if ( mbsWallModule.rearEndPointConstructorSpace ==
                              wallsToConnect[ i ].frontEndPointConstructorSpace )
                    {
                        mbsWallModule.AddRearConnection( wallsToConnect[ i ] );
                        wallsToConnect[ i ].AddFrontConnection( mbsWallModule );
                    }

                    if ( mbsWallModule.frontEndPointConstructorSpace ==
                         wallsToConnect[ i ].frontEndPointConstructorSpace )
                    {
                        mbsWallModule.AddFrontConnection( wallsToConnect[ i ] );
                        wallsToConnect[ i ].AddFrontConnection( mbsWallModule );
                    }
                    else if ( mbsWallModule.frontEndPointConstructorSpace ==
                              wallsToConnect[ i ].rearEndPointConstructorSpace )
                    {
                        mbsWallModule.AddFrontConnection( wallsToConnect[ i ] );
                        wallsToConnect[ i ].AddRearConnection( mbsWallModule );
                    }
                }

            _walls.Add( mbsWallModule );
        }

        public  void RemoveWallAndDisconnect( MBSWallModule mbsWallModule, bool doUpdateMesh = false )
        {
            if ( mbsWallModule != null && _walls != null ) 
                _walls.Remove( mbsWallModule );

            if ( mbsWallModule.frontConnections != null && mbsWallModule.frontConnections.Count > 0 )
                for ( var i = 0; i < mbsWallModule.frontConnections.Count; i++ )
                    mbsWallModule.frontConnections[ i ].RemoveConnection( mbsWallModule );

            if ( mbsWallModule.rearConnections != null && mbsWallModule.rearConnections.Count > 0 )
                for ( var i = 0; i < mbsWallModule.rearConnections.Count; i++ )
                    mbsWallModule.rearConnections[ i ].RemoveConnection( mbsWallModule );

            if ( doUpdateMesh )
            {
                List<MBSWallModule> wallToUpdate = new List<MBSWallModule>( );

                if ( mbsWallModule.frontConnections != null && mbsWallModule.frontConnections.Count > 0 )
                    wallToUpdate.Add( mbsWallModule.frontConnections.ElementAtOrDefault( 0 ) );

                if ( mbsWallModule.rearConnections != null && mbsWallModule.rearConnections.Count > 0 )
                    wallToUpdate.Add( mbsWallModule.rearConnections.ElementAtOrDefault( 0 ) );

                for ( var i = 0; i < wallToUpdate.Count; i++ )
                    if ( wallToUpdate[ i ] != null )
                        WallConnectionController.RecalculateConnectionNodes( wallToUpdate[ i ] );
            }

            UpdateEndPoints( );
        }


        public  void UpdateAreas( )
        {
            var listToRemove = _areas.ToList( );

            List<int> visitedIndexes = new List<int>( );

            for ( var i = 0; i < _areas.Count; i++ )
            {
                var curArea = _areas[ i ];

                for ( var j = i; j < _areas.Count; j++ )
                {
                    if ( i == j ) continue;

                    if ( visitedIndexes.Contains( i ) ) continue;

                    if ( curArea.Equals( _areas[ j ] ) )
                    {
                        listToRemove.Remove( _areas[ j ] );
                        visitedIndexes.Add( j );
                    }
                }
            }

            _areas = listToRemove.ToList( );

            RemoveIntersectedAreas( );
            
            mEditor?.UpdateAreaListView(  );
            
        }

        public void AddArea( RoomArea area )
        {
            _areas.Add( area );
        }

        public  void RemoveAreasWithWall( MBSWallModule mbsWallModule )
        {
            if ( _areas == null ) return;

            if ( _areas.Count == 0 ) return;

            if ( mbsWallModule == null ) return;

            _areas.RemoveAll( i => i.Walls.Contains( mbsWallModule ) );
        }

        private void RemoveIntersectedAreas( )
        {
            List<RoomArea> toRemove = new List<RoomArea>( );

            for ( var i = 0; i < _areas.Count; i++ )
            {
                var curArea = _areas[ i ];

                for ( var j = 0; j < _areas.Count; j++ )
                {
                    if ( i == j ) continue;

                    var checkArea = _areas[ j ];

                    if ( curArea.area <= checkArea.area ) continue;

                    for ( var k = 0; k < checkArea.Walls.Count; k++ )
                    {
                        var checkWallPosition =
                            transform.InverseTransformPoint( checkArea.Walls[ k ].transform.position );

                        var (inside, edge) = curArea.IsPointInsideAreaWithEdge( checkWallPosition );

                        if ( inside && !edge ) toRemove.Add( curArea );
                    }
                }
            }

            toRemove = toRemove.Distinct( ).ToList( );

            foreach ( var a in toRemove ) _areas.Remove( a );
        }

        public  void AddAreasIfFound( MBSWallModule mbsWallModule )
        {
            var foundAreas = Wall_LoopFinder.FindLoops( mbsWallModule );

            foreach ( var a in foundAreas )
                if ( a.essentialPoints is { Length: > 2 } )
                    AddArea( a );
        }

        public  RoomArea[ ] FindAreasWith( MBSWallModule mbsWallModule )
        {
            RoomArea[ ] areas;

            if ( _areas == null || _areas.Count == 0 )
                areas = new RoomArea[ 0 ];
            else
                areas = _areas.Where( i => i.Walls.Contains( mbsWallModule ) ).ToArray( );

            return areas;
        }


        public  void UpdateEndPoints( )
        {
            if ( _walls == null || _walls.Count == 0 ) return;

            _walls = new List<MBSWallModule>( _walls.Distinct( ) );
            _walls = new List<MBSWallModule>( _walls.Where( i => i != null ) );

            _allEndPoints = new List<Vector3>( _walls.Count * 2 );

            for ( var i = 0; i < _walls.Count; i++ )
            {
                _allEndPoints.Add( _walls[ i ].frontEndPointConstructorSpace );
                _allEndPoints.Add( _walls[ i ].rearEndPointConstructorSpace );
            }

            _allEndPoints = _allEndPoints.DistinctVectors( );
            _allEndPointsForSnaps = new List<Vector3>( _allEndPoints );

            if ( !_allEndPoints.Contains( Vector3.zero ) ) _allEndPointsForSnaps.Add( Vector3.zero );

            for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
            {
                var toNullYAxis = _allEndPointsForSnaps[ i ];
                toNullYAxis.y = 0;
                _allEndPointsForSnaps[ i ] = toNullYAxis;
            }
        }


        public  bool TryFindEndPointToSnap( Vector3 constrPointNotSnapped, float snapDistance, out Vector3 outPosition )
        {
            outPosition = default;

            var minDistance = float.MaxValue;
            var minDistIndex = -1;

            constrPointNotSnapped.y = 0;

            _ = Vector3.zero;

            for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
            {
                var distance = Vector3.Distance( constrPointNotSnapped, _allEndPointsForSnaps[ i ] );
                if ( distance <= snapDistance && distance < minDistance )
                {
                    minDistance = distance;
                    minDistIndex = i;
                }
            }

            if ( minDistIndex == -1 ) return false;

            outPosition = _allEndPointsForSnaps[ minDistIndex ];
            return true;
        }


        public  bool TryFindEndPointToSnap( Vector3 constrPointNotSnapped, Vector3 constrPointSnapped, float radius,
                                             out Vector3 outPosition )
        {
            outPosition = default;

            var minDistance = float.MaxValue;
            var minDistIndex = -1;

            constrPointNotSnapped.y = 0;

            var retval = false;

            for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
            {
                var distance = Vector3.Distance( constrPointNotSnapped, _allEndPointsForSnaps[ i ] );
                if ( distance <= radius && distance < minDistance )
                {
                    minDistance = distance;
                    minDistIndex = i;
                    retval = true;
                }
            }

            if ( minDistIndex == -1 )
            {
                for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
                {
                    var distance = Vector3.Distance( constrPointSnapped, _allEndPointsForSnaps[ i ] );
                    if ( distance <= radius && distance < minDistance )
                    {
                        minDistance = distance;
                        minDistIndex = i;
                        retval = true;
                    }
                }
            }

            if ( retval ) outPosition = _allEndPointsForSnaps[ minDistIndex ];

            return retval;
        }

        public  bool TryFindEndPointToSnap( Vector3 constrPointNotSnapped, Vector3 constrPointSnapped, float radius,
                                             out Vector3 outPosition, Vector3 excludePos )
        {
            outPosition = default;

            var minDistance = float.MaxValue;
            var minDistIndex = -1;

            constrPointNotSnapped.y = 0;

            var retval = false;

            for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
            {
                if ( _allEndPointsForSnaps[ i ].ApxEquals( excludePos ) ) continue;

                var distance = Vector3.Distance( constrPointNotSnapped, _allEndPointsForSnaps[ i ] );
                if ( distance <= radius && distance < minDistance )
                {
                    minDistance = distance;
                    minDistIndex = i;
                    retval = true;
                }
            }

            if ( minDistIndex == -1 )
                for ( var i = 0; i < _allEndPointsForSnaps.Count; i++ )
                {
                    var distance = Vector3.Distance( constrPointSnapped, _allEndPointsForSnaps[ i ] );
                    if ( distance <= radius && distance < minDistance )
                    {
                        minDistance = distance;
                        minDistIndex = i;
                        retval = true;
                    }
                }

            if ( retval ) outPosition = _allEndPointsForSnaps[ minDistIndex ];

            return retval;
        }

        private List<MBSWallModule> IsThereWallToConnect( MBSWallModule mbsWallModule, Vector3 position1,
                                                          Vector3 position2 )
        {
            List<MBSWallModule> retval = new List<MBSWallModule>( );

            if ( _walls != null && _walls.Count > 0 )
                for ( var i = 0; i < _walls.Count; i++ )
                {
                    if ( _walls[ i ].frontEndPointConstructorSpace == position1 ||
                         _walls[ i ].rearEndPointConstructorSpace == position1 )
                        retval.Add( _walls[ i ] );

                    if ( _walls[ i ].frontEndPointConstructorSpace == position2 ||
                         _walls[ i ].rearEndPointConstructorSpace == position2 )
                        retval.Add( _walls[ i ] );
                }

            return retval;
        }

        public  RoomArea GetAreaAtPoint( Vector3 point_Constr )
        {
            if ( _areas == null || _areas.Count == 0 ) return null;

            RoomArea area = null;
            var minArea = float.MaxValue;

            for ( var i = 0; i < _areas.Count; i++ )
                if ( _areas[ i ].IsPointInsideArea( point_Constr ) )
                    if ( _areas[ i ].area < minArea )
                    {
                        area = _areas[ i ];
                        minArea = area.area;
                    }

            return area;
        }

        public  void AddWallEndPoints( MBSWallModule mbsWallModule )
        {
            if ( mbsWallModule == null ) return;

            _allEndPoints ??= new List<Vector3>( );

            _allEndPointsForSnaps ??= new List<Vector3>( );

            if ( !_allEndPoints.Any( i => i.ApxEquals( mbsWallModule.frontEndPointConstructorSpace ) ) )
                _allEndPoints.Add( mbsWallModule.frontEndPointConstructorSpace );

            if ( !_allEndPoints.Any( i => i.ApxEquals( mbsWallModule.rearEndPointConstructorSpace ) ) )
                _allEndPoints.Add( mbsWallModule.rearEndPointConstructorSpace );

            var front = mbsWallModule.frontEndPointConstructorSpace;
            front.y = 0;

            if ( !_allEndPointsForSnaps.Any( i => i.ApxEquals( front ) ) ) _allEndPointsForSnaps.Add( front );

            var rear = mbsWallModule.rearEndPointConstructorSpace;
            rear.y = 0;

            if ( !_allEndPointsForSnaps.Any( i => i.ApxEquals( rear ) ) ) _allEndPointsForSnaps.Add( rear );

            _allEndPoints = _allEndPoints.DistinctVectors( );
            _allEndPointsForSnaps = _allEndPointsForSnaps.DistinctVectors( );
        }

         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
        public  void OnEditorDisable( )
        {
            if ( this == _sCurrent )
                Builder_Controller.Stop( );
        }


        #region DebugGizmos

        private int _iter;
        private int _i;

        private void OnDrawGizmosSelected( )
        {
            if ( additionalPointsToDraw != null && additionalPointsToDraw.Length > 0 )
            {
                var matrix = Handles.matrix;
                Handles.matrix = this.transform.localToWorldMatrix;
                Handles.color = Color.yellow;
                for ( var i = 0; i < additionalPointsToDraw.Length; i++ )
                {
                    Handles.SphereHandleCap( 0, additionalPointsToDraw[ i ], Quaternion.identity, 0.4f,
                                             EventType.Repaint );
                }

                Handles.matrix = matrix;
            }
        }

        private void OnDrawGizmos( )
        {
            if ( this == _sCurrent )
            {
                MbsGrid.Draw( );
                Gizmos_Controller.Draw( );
            }
        }

        #endregion
    }
}

#endif