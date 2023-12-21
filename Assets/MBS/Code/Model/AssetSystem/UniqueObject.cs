#if UNITY_EDITOR

using System;
using UnityEngine;

namespace MBS.Model.AssetSystem
{
    [Serializable]
    public  class UniqueObject
    {
        [SerializeField] protected string guid;


        public  UniqueObject( )
        {
            guid = System.Guid.NewGuid( ).ToString( "N" );
        }

        public  string Guid
        {
            get => guid.ToString();
            set => guid = value;
        }

        public  void GenerateNewGuid( )
        {
            guid = System.Guid.NewGuid( ).ToString( "N" );
        }
    }

    [Serializable]
    public  class UniqueNamedObject : UniqueObject
    {
        [SerializeField] public  string name;

        public  string Name
        {
            get => name;
            set => name = value;
        }
    }
}

#endif