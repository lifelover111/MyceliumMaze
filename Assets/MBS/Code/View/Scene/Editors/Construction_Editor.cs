#if UNITY_EDITOR

using System.Linq;
using MBS.Controller.Configuration;
using MBS.Controller.Scene;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine.UIElements;

namespace MBS.View.Scene
{
    [CustomEditor( typeof( MBSConstruction ) )]
    public class ConstructionEditor : Editor
    {
        private MBSConstruction _mbsConstruction;
        private ListView _areasListView;

        public override VisualElement CreateInspectorGUI( )
        {
            if ( _mbsConstruction != null )
                _mbsConstruction.mEditor = this;

            var windowPath = PathController.GetPATH_ConstructionInspector( );
            var listItemPath = PathController.GetPATH_ConstructionInspectorListViewItem( );

            var windowLoaded = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( windowPath );
            var listItemLoaded = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>( listItemPath );

            var window = windowLoaded.CloneTree( );

            var editButton = window.Q<Button>( "edit-button" );
            editButton.clicked += ( ) => { ( target as MBSConstruction )?.StartEditObject( ); };

            _areasListView = window.Q<ListView>( "areas-listview" );
            _areasListView.itemsSource = ( (MBSConstruction)target ).Areas;
            _areasListView.makeItem = ( ) => listItemLoaded.Instantiate( );
            _areasListView.bindItem = ( visualElement, index ) =>
            {
                visualElement.RegisterCallback<PointerEnterEvent>( ( f ) =>
                {
                    var area = ( target as MBSConstruction )?.Areas[ index ];
                    var points = area.essentialPoints;
                    ( (MBSConstruction)target ).additionalPointsToDraw = points.ToArray( );
                    SceneView.RepaintAll( );
                } );

                visualElement.RegisterCallback<PointerLeaveEvent>( ( f ) =>
                {
                    ( target as MBSConstruction ).additionalPointsToDraw = null;
                    SceneView.RepaintAll( );
                } );

                visualElement.Q<Label>( "area-name_label" ).text = $"Area ({index.ToString( )})";

                visualElement.Q<Button>( "generate-mesh_button" ).clicked += ( ) =>
                {
                    var area = ( (MBSConstruction)target ).Areas[ index ];
                    var areaFloor = MeshCreator.CreatePlaneMesh( area.essentialPoints, area.pointsWindingOrder );
                    areaFloor.name = $"Generated Mesh. Area ({index.ToString( )}).";
                    areaFloor.transform.SetParent( ( target as MBSConstruction ).transform );
                };

                visualElement.Q<Button>( "turn-inside_button" ).clicked += ( ) =>
                {
                    var area = ( (MBSConstruction)target ).Areas[ index ];
                    area.TurnWalls( _mbsConstruction, true );
                };

                visualElement.Q<Button>( "turn-outside_button" ).clicked += ( ) =>
                {
                    var area = ( (MBSConstruction)target ).Areas[ index ];
                    area.TurnWalls( _mbsConstruction, false );
                };
            };

            return window;
        }

        public void UpdateAreaListView( )
        {
            if ( _areasListView != null )
            {
                _areasListView.itemsSource = ( (MBSConstruction)target ).Areas;
                _areasListView.Rebuild( );
            }
        }


        private void OnEnable( )
        {
            _mbsConstruction = target as MBSConstruction;
        }

        private void OnDisable( )
        {
            _mbsConstruction.OnEditorDisable( );
        }
    }
}
#endif