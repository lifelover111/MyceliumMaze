#if UNITY_EDITOR

using System;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    [Serializable]
    public  class ModularGroupCategory : UniqueObject
    {
        [SerializeField] private string _name;

        public  ModularGroupCategory( string name )
        {
            _name = name;
        }

        public  string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}

#endif