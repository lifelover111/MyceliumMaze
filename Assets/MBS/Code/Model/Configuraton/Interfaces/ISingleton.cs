#if UNITY_EDITOR

namespace MBS.Model.Configuration
{
    public  interface ISingleton
    {
        public void ColdInitialization( );
        public void WarmInitialization( );
    }
}

#endif