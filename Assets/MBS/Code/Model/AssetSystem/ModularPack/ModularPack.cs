#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MBS.Model.AssetSystem
{
    [Serializable]
    public  class ModularPack : UniqueObject
    {
        [SerializeField] private string _name;
        [SerializeField] private WallGroup[ ] _wallGroups;
        [SerializeField] private FloorGroup[ ] _floorGroups;
        [FormerlySerializedAs( "_placerGroups" )] [SerializeField] private DecoratorGroup[ ] _decoratorGroups;
        [FormerlySerializedAs( "_placerCategories" )] [SerializeField] private ModularGroupCategory[ ] _decoratorCategories;

         
        public  ModularPack( ModularPack copyFrom )
        {
            _name = copyFrom._name;
            guid = copyFrom.guid;
            _wallGroups = copyFrom._wallGroups.ToArray( );
            _floorGroups = copyFrom._floorGroups.ToArray( );
            _decoratorGroups = copyFrom._decoratorGroups.ToArray( );
            _decoratorCategories = copyFrom._decoratorCategories.ToArray( );
        }


        public  ModularPack( )
        {
            _wallGroups = new WallGroup[ 0 ];
            _floorGroups = new FloorGroup[ 0 ];
            _decoratorGroups = new DecoratorGroup[ 0 ];
            _decoratorCategories = new[ ] { new ModularGroupCategory( "All" ) };
        }


        public  string Name
        {
            get => _name;
            set => _name = value;
        }

        public  WallGroup[ ] WallGroups
        {
            get => _wallGroups;
            set => _wallGroups = value;
        }

        public  FloorGroup[ ] FloorGroups
        {
            get => _floorGroups;
            set => _floorGroups = value;
        }

        public  DecoratorGroup[ ] DecoratorGroups
        {
            get => _decoratorGroups;
            set => _decoratorGroups = value;
        }

        public  ModularGroupCategory[ ] DecoratorCategories
        {
            get => _decoratorCategories;
            set => _decoratorCategories = value;
        }

        public  ICollection<ModularGroup> GetGroupsByToolIndex( int toolIndex )
        {
            switch ( toolIndex )
            {
                case 0:
                    return _wallGroups;
                case 1:
                    return _floorGroups;
                case 2:
                    return _decoratorGroups;
                default:
                    return null;
            }
        }
    }
}
#endif