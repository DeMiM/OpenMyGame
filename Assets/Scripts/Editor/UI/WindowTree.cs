#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowTree.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    internal sealed class WindowTree : UnityEditor.IMGUI.Controls.TreeView
    {
        private readonly WindowManager _windowManager = null;

        public WindowTree( UnityEditor.IMGUI.Controls.TreeViewState viewState,
            UnityEditor.IMGUI.Controls.MultiColumnHeader multiColumnHeader, WindowManager windowManager )
            : base( viewState, multiColumnHeader )
        {
            _windowManager = windowManager;

            rowHeight = UnityEditor.EditorGUIUtility.singleLineHeight * 1.4f;
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            Reload();
        }

        protected override UnityEditor.IMGUI.Controls.TreeViewItem BuildRoot()
        {
            int viewId = -1;

            UnityEditor.IMGUI.Controls.TreeViewItem root = new( viewId, -1, "Root" );
            System.Collections.Generic.List<UnityEditor.IMGUI.Controls.TreeViewItem> rows = new( 1024 );

            System.Reflection.FieldInfo fieldPositionNormal = _windowManager?.GetType().GetField( "_positionNormal",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );

            System.Reflection.FieldInfo fieldPositionPopup = _windowManager?.GetType().GetField( "_positionPopup",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );

            System.Reflection.FieldInfo fieldModalWindows = _windowManager?.GetType().GetField( "_modalWindows",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );

            if ( fieldPositionNormal?.GetValue( _windowManager ) is System.Collections.Generic.List<Window> positionNormal )
            {
                foreach ( Window window in positionNormal )
                {
                    rows.Add( new WindowTreeItem( ++viewId, 0, window ) );
                }
            }

            if ( fieldPositionPopup?.GetValue( _windowManager ) is System.Collections.Generic.List<Window> positionPopup )
            {
                foreach ( Window window in positionPopup )
                {
                    rows.Add( new WindowTreeItem( ++viewId, 0, window ) );
                }
            }

            if ( fieldModalWindows?.GetValue( _windowManager ) is System.Collections.Generic.List<Window> modalWindows )
            {
                foreach ( Window window in modalWindows )
                {
                    rows.Add( new WindowTreeItem( ++viewId, 0, window ) );
                }
            }

            SetupParentsAndChildrenFromDepths( root, rows );

            return root;
        }

        protected override void DoubleClickedItem( int id )
        {
            base.DoubleClickedItem( id );

            System.Collections.Generic.IList<UnityEditor.IMGUI.Controls.TreeViewItem> found = 
                FindRows( new System.Collections.Generic.List<int>() { id } );
            
            if ( found?.Count > 0 )
            {
                foreach ( UnityEditor.IMGUI.Controls.TreeViewItem item in found )
                {
                    if ( item is WindowTreeItem viewItem )
                    {
                        UnityEditor.EditorGUIUtility.PingObject( viewItem.instanceId );
                    }
                }
            }
        }

        protected override void RowGUI( RowGUIArgs args )
        {
            if ( args.item is WindowTreeItem item )
            {
                int columns = args.GetNumVisibleColumns();
                for ( int i = 0; i < columns; ++i )
                {
                    UnityEngine.Rect rect = args.GetCellRect( i );
                    CenterRectUsingSingleLineHeight( ref rect );

                    int column = args.GetColumn( i );
                    switch ( column )
                    {
                        case 0:
                            rect.xMin += 4f;
                            rect.xMax -= 2f;
                            UnityEngine.GUI.Label( rect, item.displayName );
                            break;

                        case 1:
                            rect.xMin += 2f;
                            rect.xMax -= 2f;
                            UnityEngine.GUI.Label( rect, item.windowType );
                            break;

                        case 2:
                            rect.xMin += 2f;
                            rect.xMax -= 2f;
                            UnityEngine.GUI.Label( rect, item.windowOrder );
                            break;

                        default:
                            args.rowRect = rect;
                            base.RowGUI( args );
                            break;
                    }
                }
            }
        }

    }

}
