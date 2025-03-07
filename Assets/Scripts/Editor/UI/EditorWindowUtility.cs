#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="EditorWindowUtility.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    public static class EditorWindowUtility
    {
        public static void ShowWindow<T>( string windowTitle, UnityEngine.Vector2 minSize, bool createIfNotExists )
            where T : UnityEditor.EditorWindow
        {
            T[] objects = UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if ( objects.Length == 0 )
            {
                if ( createIfNotExists )
                {
                    T editorWindow = UnityEngine.ScriptableObject.CreateInstance<T>();
                    editorWindow.titleContent = new UnityEngine.GUIContent( windowTitle );
                    editorWindow.minSize = minSize;
                    editorWindow.Show( true );
                }
            }
            else
            {
                objects[ 0 ].Focus();
            }
        }

        public static void ShowWindow<T>( string windowTitle, bool createIfNotExists )
            where T : UnityEditor.EditorWindow
        {
            ShowWindow<T>( windowTitle, UnityEngine.Vector2.zero, createIfNotExists );
        }

        public static T GetWindow<T>() where T : UnityEditor.EditorWindow
        {
            T[] objects = UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if ( objects.Length > 0 )
            {
                return objects[ 0 ];
            }

            return null;
        }

    }

}
