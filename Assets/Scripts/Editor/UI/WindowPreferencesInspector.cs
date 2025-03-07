#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowPreferencesInspector.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    [UnityEditor.CustomEditor( typeof( WindowPreferences ) )]
    internal sealed class WindowPreferencesInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DoInspector();

            if ( UnityEngine.GUI.changed )
            {
                serializedObject.ApplyModifiedProperties();
                UnityEditor.EditorUtility.SetDirty( target );
            }
        }

        private void DoInspector()
        {
            UnityEditor.SerializedProperty propPopupBaseOrder = serializedObject.FindProperty( "_popupBaseOrder" );
            UnityEditor.EditorGUILayout.PropertyField( propPopupBaseOrder, InternalContent.popupBaseOrder );
            UnityEditor.SerializedProperty propModalBaseOrder = serializedObject.FindProperty( "_modalBaseOrder" );
            UnityEditor.EditorGUILayout.PropertyField( propModalBaseOrder, InternalContent.modalBaseOrder );

            UnityEditor.EditorGUILayout.Separator();

            UnityEngine.Object selected = UnityEditor.EditorGUILayout.ObjectField( InternalContent.enumAsset,
                WindowBuilder.enumAsset, typeof( UnityEngine.TextAsset ), false );
            if ( selected is UnityEngine.TextAsset newAsset && ( newAsset != WindowBuilder.enumAsset ) )
            {
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath( newAsset );

                if ( System.IO.Path.GetExtension( assetPath ) == ".cs" )
                {
                    WindowBuilder.enumAsset = newAsset;

                    UnityEngine.Debug.LogFormat( "[Windows]: Change enum windows on \"{0}\"",
                        assetPath );
                }
                else
                {
                    UnityEngine.Debug.LogWarningFormat( "[Windows]: Asset \"{0}\" is not C# file!",
                       assetPath );
                }
            }
        }

        private static class Integration
        {
            [UnityEditor.MenuItem( "Test/Windows/Preferences", false, 1299 )]
            private static void ShowWindowPreferences()
            {
                string assetPath = $"Assets/Resources/{WindowManager.kPreferencesName}.asset";

                WindowPreferences prefs = ScriptableObjectUtility.FindInstance<WindowPreferences>( assetPath, false );

                if ( prefs != null )
                {
                    UnityEditor.Selection.SetActiveObjectWithContext( prefs, null );
                }
            }

        }

        private static class InternalContent
        {
            public static readonly UnityEngine.GUIContent popupBaseOrder = new( "Popup Base Order" );
            public static readonly UnityEngine.GUIContent modalBaseOrder = new( "Modal Base Order" );
            public static readonly UnityEngine.GUIContent enumAsset = new( "Enum Windows File" );

        }

    }

}
