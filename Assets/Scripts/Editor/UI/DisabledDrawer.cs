#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="DisabledDrawer.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Extensions.Editor
{

    [UnityEditor.CustomPropertyDrawer( typeof( DisabledAttribute ) )]
    internal sealed class DisabledDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, UnityEngine.GUIContent label ) =>
            UnityEditor.EditorGUI.GetPropertyHeight( property, label, true );

        public override void OnGUI( UnityEngine.Rect position, UnityEditor.SerializedProperty property,
            UnityEngine.GUIContent label )
        {
            UnityEditor.EditorGUI.BeginProperty( position, label, property );

            using ( new UnityEditor.EditorGUI.DisabledScope( true ) )
            {
                UnityEditor.EditorGUI.PropertyField( position, property, label, true );
            }

            UnityEditor.EditorGUI.EndProperty();
        }

    }

}
