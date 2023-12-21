#if UNITY_EDITOR

namespace MBS.Code.Utilities.Helpers
{
    public  static class Collections_Helper
    {
        public  static int GetNextLoopIndex( int currentIndex, int collectionSize )
        {
            var nextIndex = currentIndex + 1;

            if ( nextIndex > collectionSize - 1 || nextIndex < 0 )
                return 0;

            return nextIndex;
        }

        public  static int GetPrevLoopIndex( int currentIndex, int collectionSize )
        {
            var prevIndex = currentIndex - 1;

            if ( prevIndex < 0 || prevIndex > collectionSize - 1 )
                return collectionSize - 1;

            return prevIndex;
        }
    }
}

#endif