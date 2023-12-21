#if UNITY_EDITOR

using System;
using UnityEngine;

namespace MBS.Model.Configuration
{
    [Serializable]
    public  class InputConfig : ScriptableObject
    {
        public  KeyCode CancelKey = KeyCode.Escape;

        public  WallTool WallToolInputs;
        public  FloorTool FloorToolInputs;
        public  PlacerTool PlacerToolInputs;
        public  PickerTool PickerToolInputs;


        [Serializable]
        public  class WallTool
        {
            [SerializeField] public  KeyCode WallMoveGridOrigin = KeyCode.Z;
            [SerializeField] public  KeyCode WallSnapToGrid = KeyCode.X;
            [SerializeField] public  KeyCode WallSnapToEnds = KeyCode.C;
            [SerializeField] public  KeyCode ChangeSnapAccuracy = KeyCode.V;
            [SerializeField] public  KeyCode IncreaseLevelNumber = KeyCode.B;
            [SerializeField] public  KeyCode DecreaseLevelNumber = KeyCode.N;
        }

        [Serializable]
        public  class FloorTool
        {
            [SerializeField] public  KeyCode FloorMoveGridOrigin = KeyCode.Z;
            [SerializeField] public  KeyCode FloorAutofill = KeyCode.X;
        }

        [Serializable]
        public  class PlacerTool
        {
            public  PlacementMode Placement;
            public  RotationMode Rotation;
            public  ScalingMode Scaling;
            
            public  KeyCode PlacerMoveGridOrigin = KeyCode.Z;

            [Serializable]
            public  class PlacementMode
            {
                [SerializeField] public  KeyCode PlacementType = KeyCode.Z;
                [SerializeField] public  KeyCode PlacementFacing = KeyCode.X;
                [SerializeField] public  KeyCode CollisionToggle = KeyCode.C;
                [SerializeField] public  KeyCode RandomModuleToggle = KeyCode.V;
                [SerializeField] public  KeyCode AdjustHeightToggle = KeyCode.B;
                [SerializeField] public  KeyCode SnapToGrid = KeyCode.N;
                [SerializeField] public  KeyCode RotationMode = KeyCode.R;
                [SerializeField] public  KeyCode ScalingMode = KeyCode.T;
                [SerializeField] public  KeyCode NextModule = KeyCode.E;
            }
            
            [Serializable]
            public  class RotationMode
            {
                [SerializeField] public  KeyCode ResetRotation = KeyCode.C;
                [SerializeField] public  KeyCode XAxis = KeyCode.X;
                [SerializeField] public  KeyCode YAxis = KeyCode.Y;
                [SerializeField] public  KeyCode ZAxis = KeyCode.Z;
                [SerializeField] public  KeyCode ChangeRotationSpace = KeyCode.V;
                [SerializeField] public  KeyCode SnapAngleToggle = KeyCode.B;
                [SerializeField] public  KeyCode ChangeSnapAccuracy = KeyCode.N;
            }
            
            [Serializable]
            public  class ScalingMode
            {
                [SerializeField] public  KeyCode ResetScale = KeyCode.C;
                [SerializeField] public  KeyCode AllAxis = KeyCode.Q;
                [SerializeField] public  KeyCode XAxis = KeyCode.X;
                [SerializeField] public  KeyCode YAxis = KeyCode.Y;
                [SerializeField] public  KeyCode ZAxis = KeyCode.Z;
                [SerializeField] public  KeyCode ChangeSnapAccuracy = KeyCode.N;
            }

        }

        [Serializable]
        public  class PickerTool
        {
            [SerializeField] public  KeyCode BoundsType = KeyCode.Z;
            [SerializeField] public  KeyCode AxisParameter = KeyCode.X;
        }
    }
}

#endif