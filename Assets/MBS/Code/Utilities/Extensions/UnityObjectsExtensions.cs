#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MBS.Utilities.Extensions
{
    public  static class UnityObjExtensions
    {
        public  static void SelectObject( this Object obj )
        {
            Selection.objects = new[ ] { obj };
        }

        public  static void AddToSelection( this Object obj )
        {
            var selected = Selection.objects.ToList( );
            selected.Add( obj );
            Selection.objects = selected.ToArray( );
        }

        public  static void DoRecursive( this Transform root, Action<Transform> function )
        {
            foreach ( Transform child in root )
            {
                function.Invoke( child );
                child.DoRecursive( function );
            }
        }
    }
}

#endif