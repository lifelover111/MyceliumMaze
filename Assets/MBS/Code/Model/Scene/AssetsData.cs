#if UNITY_EDITOR

using UnityEngine;

namespace MBS.Model.Scene
{
    public  static class AssetsData
    {
        public  static class Floor
        {
            public  static GameObject SquarePrefab;
            public  static GameObject TriangularPrefab;

            public  static Mesh SquareMesh;
            public  static Mesh TriangularMesh;

            public  static bool IsTriangularExist;

            public  static void Clear( )
            {
                SquarePrefab = null;
                TriangularPrefab = null;
                IsTriangularExist = false;
            }
        }
    }
}

#endif