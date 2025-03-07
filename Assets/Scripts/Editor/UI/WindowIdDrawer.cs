#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowIdDrawer.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{
    [UnityEditor.CustomPropertyDrawer( typeof( WindowIdAttribute ) )]
    internal sealed class WindowIdDrawer : UnityEditor.PropertyDrawer
    {
        private UnityEngine.GUIContent _windowName = null;
        private string _lastName = string.Empty;
        private int _selected = -2;
        private bool _isEdited = false;

        public override bool CanCacheInspectorGUI( UnityEditor.SerializedProperty property )
        {
            return false;
        }

        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, UnityEngine.GUIContent label )
        {
            if ( string.IsNullOrEmpty( property.stringValue ) || ( _isEdited == true ) )
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight + 2f;
            }

            if ( ( attribute as WindowIdAttribute )?.editable ?? false )
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight * 2f + 4f;
            }

            return UnityEditor.EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI( UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label )
        {
            if ( property.propertyType != UnityEditor.SerializedPropertyType.String )
            {
                UnityEditor.EditorGUI.LabelField( position, label, InternalContent.wrongType );

                return;
            }

            if ( attribute is WindowIdAttribute attr )
            {
                UnityEditor.EditorGUI.BeginProperty( position, label, property );

                if ( attr.editable == true )
                {
                    if ( string.IsNullOrEmpty( property.stringValue ) || ( _isEdited == true ) )
                    {
                        if ( _isEdited == true )
                        {
                            UnityEngine.Event @event = UnityEngine.Event.current;

                            int controlID = UnityEngine.GUIUtility.GetControlID( UnityEngine.FocusType.Passive );

                            if ( @event.GetTypeForControl( controlID ) == UnityEngine.EventType.KeyDown )
                            {
                                if ( @event.keyCode == UnityEngine.KeyCode.Escape )
                                {
                                    _lastName = string.Empty;
                                    _isEdited = false;

                                    @event.Use();

                                    UnityEngine.GUIUtility.ExitGUI();
                                }
                            }
                        }

                        using ( UnityEditor.EditorGUI.ChangeCheckScope checker = new() )
                        {
                            string newName = UnityEditor.EditorGUI.DelayedTextField( position, label, _lastName );

                            if ( checker.changed == true )
                            {
                                if ( string.IsNullOrEmpty( newName ) == false )
                                {
                                    if ( WindowBuilder.TryAddWindowName( newName, _lastName, out string windowId ) )
                                    {
                                        property.stringValue = windowId;

                                        _windowName = null;
                                        _lastName = string.Empty;
                                        _selected = -2;
                                        _isEdited = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if ( _windowName == null )
                        {
                            _windowName = new UnityEngine.GUIContent( property.stringValue );
                        }

                        UnityEngine.Rect rc0 = position;
                        rc0.xMin += UnityEditor.EditorGUIUtility.labelWidth;

                        float buttonWidth = rc0.width * 0.33f;

                        UnityEngine.Rect rc1 = position;
                        rc1.height = UnityEditor.EditorGUIUtility.singleLineHeight + 2f;

                        UnityEditor.EditorGUI.LabelField( rc1, label, _windowName, UnityEditor.EditorStyles.boldLabel );

                        UnityEngine.Rect rc2 = position;
                        rc2.yMin = rc1.yMax;
                        rc2.xMin = rc0.xMin;
                        rc2.xMax = rc2.xMin + buttonWidth;

                        if ( UnityEngine.GUI.Button( rc2, InternalContent.edit, UnityEditor.EditorStyles.miniButton ) == true )
                        {
                            _lastName = _windowName.text;
                            _isEdited = true;
                        }

                        UnityEngine.Rect rc3 = position;
                        rc3.yMin = rc1.yMax;
                        rc3.xMin = rc2.xMax + 1f;
                        rc3.xMax = rc3.xMin + buttonWidth;

                        if ( UnityEngine.GUI.Button( rc3, InternalContent.remove, UnityEditor.EditorStyles.miniButton ) )
                        {
                            bool shouldRemoved = UnityEditor.EditorUtility.DisplayDialog( "Remove...",
                                "Do you really want to remove the window id?", "Yes", "No" );

                            if ( shouldRemoved == true )
                            {
                                WindowBuilder.RemoveWindowId( property.stringValue );

                                property.stringValue = string.Empty;

                                _windowName = null;
                                _lastName = string.Empty;
                                _selected = -1;
                            }
                        }
                    }
                }
                else
                {
                    if ( _selected == -2 )
                    {
                        _selected = WindowBuilder.GetIndexByWindowId( property.stringValue );
                    }

                    int selected = UnityEditor.EditorGUI.Popup( position, label.text, _selected,
                        WindowBuilder.availableWindowNames );

                    if ( _selected != selected )
                    {
                        property.stringValue = WindowBuilder.GetWindowIdByIndex( selected );

                        _windowName = null;
                        _selected = selected;
                    }
                }

                UnityEditor.EditorGUI.EndProperty();
            }
        }

        private static class InternalContent
        {
            public static readonly UnityEngine.GUIContent wrongType = new( "Use WindowId attribute with STRING field!" );
            public static readonly UnityEngine.GUIContent windowId = new( "Window Id" );
            public static readonly UnityEngine.GUIContent edit = new( "Edit" );
            public static readonly UnityEngine.GUIContent remove = new( "Remove" );
        }

    }

}
