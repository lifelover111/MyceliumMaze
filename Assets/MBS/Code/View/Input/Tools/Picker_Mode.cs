#if UNITY_EDITOR


using System;
using System.Linq;
using MBS.Code.Utilities.Helpers;
using MBS.Controller.Builder;
using MBS.Model.Configuration;
using MBS.Model.Scene;
using MBS.Utilities.Extensions;
using MBS.View.Builder;
using MBS.View.Input.GUI;
using UnityEditor;
using UnityEngine;

namespace MBS.View.Input.Physical
{
    public  static class Picker_Mode
    {
        private static LeadAxis _leadAxis;
        private static Space _spaceMode;
        private static bool _returnSelfScaled = true;
        private static float _value;

        public  static SceneFuncData DoPickerMode( Event e )
        {
            SceneFuncData retval = new SceneFuncData( );

            var ray = Mouse.WorldRay;

            if ( Physics.Raycast( ray, out var hit, 1000 ) )
            {
                var transform = hit.collider.transform;

                ObjectData hitObjectData;

                switch ( _spaceMode )
                {
                    case Space.Self:
                        hitObjectData = GetLocalSize( transform );
                        if ( _returnSelfScaled )
                            hitObjectData.size = Matrix4x4.Scale( transform.localScale ) * hitObjectData.size;
                        break;
                    case Space.World:
                        hitObjectData = GetWorldSize( transform );
                        break;
                    default:
                        return default;
                }

                if ( hitObjectData.isOk == false ) return default;

                switch ( _leadAxis )
                {
                    case LeadAxis.XAxis:
                        _value = hitObjectData.size.x;
                        break;
                    case LeadAxis.YAxis:
                        _value = hitObjectData.size.y;
                        break;
                    case LeadAxis.ZAxis:
                        _value = hitObjectData.size.z;
                        break;
                    default:
                        _value = 1;
                        break;
                }

                _value = _value.RoundDecimals( );

                
                var rotation = Quaternion.identity;
                if ( _spaceMode == Space.Self )
                    rotation = transform.rotation;

                retval.isOk = true;
                retval.matrix = Matrix4x4.TRS( hitObjectData.center, rotation, Vector3.one );
                retval.size = hitObjectData.size;
                retval.leadAxis = _leadAxis;
                
                
                Handles.BeginGUI( );
                {
                    var style = new GUIStyle( "button" );
                    style.alignment = TextAnchor.MiddleLeft;
                    style.richText = true;
                    var text = _value.ToString();
                    text = "<b>" + text + "</b>";

                    var rectSize = style.CalcSize( new GUIContent( text ) ) * 1.2f;
                    var screenPos = HandleUtility.WorldToGUIPoint( hitObjectData.center ) + Vector2.right;
                    var mouseOffset = new Vector2( 30, 0 );
                    var rect = new Rect( screenPos.x + mouseOffset.x, screenPos.y, rectSize.x, rectSize.y );

                    UnityEngine.GUI.backgroundColor = new Color( 1.0f, 1.0f, 1.0f, 0.85f );
                    UnityEngine.GUI.Box( rect, text, style );
                    UnityEngine.GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                _value = 1;
            }

            
            Handles.EndGUI( );

            return retval;
        }

        private static ObjectData GetLocalSize( Transform transform )
        {
            ObjectData objData = new ObjectData( );

            if ( transform.TryGetComponent<MeshFilter>( out var mf ) )
            {
                var bounds = mf.sharedMesh.bounds;
                objData.size = bounds.size;
                objData.center = transform.TransformPoint( bounds.center );
                objData.isOk = true;
            }

            return objData;
        }

        private static ObjectData GetWorldSize( Transform transform )
        {
            ObjectData objData = new ObjectData( );

            if ( transform.TryGetComponent<MeshRenderer>( out var mr ) )
            {
                var bounds = mr.bounds;
                objData.size = bounds.size;
                objData.center = bounds.center;
                objData.isOk = true;
            }
            else
            {
                objData.isOk = false;
            }

            return objData;
        }

        public  static void Setup_Inputs( Action<float> mouseClickAction )
        {
            InputManager.ClearInputData( );

            InputManager.AddMouseElement( new UIElement( )
            {
                Label = Texts.Inputs.Picker.MOUSE_ACTION_LABEL,
                KeyAction = ( ) =>
                {
                    SceneData.IsMouseRecalcNeeded = true;
                    mouseClickAction?.Invoke( _value );
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
                }
            } );

            InputManager.AddKeyElement( true, new UIElement( )
            {
                Key = Config.Sgt.Input.CancelKey,
                Label = Texts.Inputs.CANCEL_ACTION,
                KeyAction = ( ) =>
                {
                    SceneData.IsMouseRecalcNeeded = true;
                    Builder_Controller.ChangeSceneMode( SceneMode.Builder );
                }
            } );

            InputManager.AddKeyElement( true, new UIDropdownElement( )
            {
                Key = Config.Sgt.Input.PickerToolInputs.BoundsType,
                Label = Texts.Inputs.Picker.BOUNDS_TYPE_LABEL,
                Choices = Enum.GetNames( typeof( Space ) ).ToList( ),
                KeyAction = ( ) =>
                {
                    var names = Enum.GetNames( typeof( Space ) );

                    var curIndex = (int)_spaceMode;
                    var nextIndex =
                        Collections_Helper.GetNextLoopIndex( curIndex, names.Length );

                    _spaceMode = (Space)nextIndex;
                },
                GetValueRemote = ( ) => _spaceMode.ToString( ),
                OnValueChangeAction = s =>
                {
                    if ( Enum.TryParse( s, true, out Space parsedSpaceMode ) )
                        _spaceMode = parsedSpaceMode;
                }
            } );

            InputManager.AddKeyElement( true, new UIDropdownElement( )
            {
                Key = Config.Sgt.Input.PickerToolInputs.AxisParameter,
                Label = Texts.Inputs.Picker.AXIS_PARAMETER_LABEL,
                Choices = Enum.GetNames( typeof( LeadAxis ) ).ToList( ),
                KeyAction = ( ) =>
                {
                    var names = Enum.GetNames( typeof( LeadAxis ) );

                    var curIndex = (int)_leadAxis;
                    var nextIndex =
                        Collections_Helper.GetNextLoopIndex( curIndex, names.Length );

                    _leadAxis = (LeadAxis)nextIndex;
                },
                GetValueRemote = ( ) => _leadAxis.ToString( ),
                OnValueChangeAction = s =>
                {
                    if ( Enum.TryParse( s, true, out LeadAxis parsed ) )
                        _leadAxis = parsed;
                }
            } );
        }

        public  static void Clear( )
        {
            _leadAxis = default;
            _spaceMode = default;
            _returnSelfScaled = default;
            _value = default;
        }

        public  static void SetParameter( LeadAxis param )
        {
            _leadAxis = param;
        }

        private struct ObjectData
        {
            public  bool isOk;
            public  Vector3 size;
            public  Vector3 center;
        }
    }
}

#endif