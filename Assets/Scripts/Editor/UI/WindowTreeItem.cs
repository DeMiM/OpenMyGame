#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowTreeItem.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    internal sealed class WindowTreeItem : UnityEditor.IMGUI.Controls.TreeViewItem
    {
        private readonly UnityEngine.GameObject _rootObject;
        private readonly string _windowName;
        private readonly string _windowType;
        private readonly string _windowOrder;

        public int instanceId => GetInstanceId( _rootObject );
        public string windowName => _windowName;
        public string windowType => _windowType;
        public string windowOrder => _windowOrder;

        private static int GetInstanceId( UnityEngine.GameObject rootObject )
        {
            if ( rootObject != null )
            {
                return rootObject.GetInstanceID();
            }

            return 0;
        }

        public WindowTreeItem( int id, int depth, Window window )
            : base( id, depth )
        {
            _rootObject = window.gameObject;
            _windowName = window.windowId;
            _windowType = $"{window.windowType}";
            _windowOrder = $"{window.windowOrder}";

            displayName = $"[{window.windowId:D6}] {_windowName}";
        }

    }

}
