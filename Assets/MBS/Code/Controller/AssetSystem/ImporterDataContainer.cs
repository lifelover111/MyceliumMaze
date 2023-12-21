#if UNITY_EDITOR

using MBS.Model.AssetSystem;

namespace MBS.Controller.AssetSystem
{
    public  class ImporterDataContainer
    {
        public  bool IsHidden;
        public  bool IsUnsaved;
        public  ModularPack Pack;

        public  string GetName( )
        {
            if ( IsUnsaved )
                return "[Unsaved] " + Pack.Name;

            if ( IsHidden )
                return "[Hidden] " + Pack.Name;

            return Pack.Name;
        }
    }
}

#endif