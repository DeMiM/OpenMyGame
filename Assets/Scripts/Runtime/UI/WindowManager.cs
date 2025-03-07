#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowManager.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{

    public sealed class WindowManager
    {
        public const string kPreferencesName = "Windows Preferences";

        private static WindowManager s_instance = null;

        private readonly WindowPreferences _preferences;
        private readonly System.Collections.Generic.Dictionary<string, Window> _registry = null;
        private readonly System.Collections.Generic.List<Window> _positionNormal = null;
        private readonly System.Collections.Generic.List<Window> _positionPopup = null;
        private readonly System.Collections.Generic.List<Window> _modalWindows = null;

#if UNITY_EDITOR
        public bool changed { get; set; }
        public bool dirty { get; set; }
#endif

        [UnityEngine.RuntimeInitializeOnLoadMethod( UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration )]
        private static void Initialize()
        {
            s_instance = new WindowManager();
        }

        internal static void Register( Window window )
        {
            s_instance.RegisterWindow( window );
        }

        internal static void Unregister( Window window )
        {
            s_instance.UnregisterWindow( window );
        }

        internal static void Restore( string windowId )
        {
            s_instance.ChangeWindowVisible( windowId, null, true, true );
        }

        public static void Show( string windowId, string args )
        {
            s_instance.ChangeWindowVisible( windowId, args, true, false );
        }

        public static void Hide( string windowId )
        {
            s_instance.ChangeWindowVisible( windowId, null, false, false );
        }

        public static void HideAll()
        {
            s_instance.CloseAllWindows();
        }

        public static bool Find( string windowId, out Window window )
        {
            return s_instance.FindWindow( windowId, out window );
        }

        internal void Focus( Window window )
        {
            switch ( window.windowType )
            {
                case WindowType.Normal:
                    ChangeOrderForNormal( window );
                    break;

                case WindowType.Popup:
                    ChangeOrderForPopup( window );
                    break;

                case WindowType.Modal:
                    ChangeOrderForModal( window );
                    break;
#if UNITY_EDITOR
                default:
                    UnityEngine.Debug.LogWarning( $"Window \"{window.windowId}\" declared as none type!" );
                    break;
#endif
            }
        }

        internal void Release( Window window )
        {
            switch ( window.windowType )
            {
                case WindowType.Normal:
                    RemoveFromNormal( window );
                    break;

                case WindowType.Popup:
                    RemoveFromPopup( window );
                    break;

                case WindowType.Modal:
                    RemoveFromModal( window );
                    break;
#if UNITY_EDITOR
                default:
                    UnityEngine.Debug.LogWarning( $"Window \"{window.windowId}\" declared as none type!" );
                    break;
#endif
            }
        }

        private WindowManager()
        {
            // Load input mobile preferences
            _preferences = UnityEngine.Resources.Load<WindowPreferences>( kPreferencesName );

#if !WIGRO_BUILD_RETAIL
            UnityEngine.Assertions.Assert.IsNotNull( _preferences,
                "[WindowManager] Preferences not found!" );
#endif
            _registry = new System.Collections.Generic.Dictionary<string, Window>( 64 );
            _positionNormal = new System.Collections.Generic.List<Window>( 64 );
            _positionPopup = new System.Collections.Generic.List<Window>( 64 );
            _modalWindows = new System.Collections.Generic.List<Window>( 8 );
        }

        private void RegisterWindow( Window window )
        {
#if UNITY_EDITOR
            if ( _registry.ContainsKey( window.windowId ) )
            {
                UnityEngine.Debug.LogAssertion( $"Re-registering a window \"{window.windowId}\" in the registry!" );
                return;
            }
#endif
            _registry.Add( window.windowId, window );

#if UNITY_EDITOR
            changed = true;
#endif
        }

        private void UnregisterWindow( Window window )
        {
#if UNITY_EDITOR
            if ( _registry.ContainsKey( window.windowId ) == false )
            {
                UnityEngine.Debug.LogAssertion( $"Window \"{window.windowId}\" not found in the registry!" );
                return;
            }
#endif
            _registry.Remove( window.windowId );

#if UNITY_EDITOR
            changed = true;
#endif
        }

        private bool FindWindow( string windowId, out Window window )
        {
            return _registry.TryGetValue( windowId, out window );
        }

        private void CloseAllWindows()
        {
            foreach ( Window window in _modalWindows )
            {
                window.InterruptShow();
                window.ChangeOrder( -1 );
                window.position = -1;
            }

            _modalWindows.Clear();

            foreach ( Window window in _positionPopup )
            {
                window.InterruptShow();
                window.ChangeOrder( -1 );
                window.position = -1;
            }

            _positionPopup.Clear();

            foreach ( Window window in _positionNormal )
            {
                window.InterruptShow();
                window.ChangeOrder( -1 );
                window.position = -1;
            }

            _positionNormal.Clear();

#if UNITY_EDITOR
            dirty = true;
#endif
        }

        private void ChangeWindowVisible( string windowId, string args, bool visible, bool restore )
        {
            if ( FindWindow( windowId, out Window window ) )
            {
                window.ChangeVisible( this, args, visible, restore );

                return;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning( $"Unknown window \"{windowId}\" detect!" );
#endif
        }

        private void UpdateWindowPosition( System.Collections.Generic.List<Window> list, int baseOrder, int startIndex )
        {
            int length = list.Count;

            for ( int index = startIndex; index < length; ++index )
            {
                Window window = list[ index ];

                window.ChangeOrder( baseOrder + index );
                window.position = index;
            }

#if UNITY_EDITOR
            dirty = true;
#endif
        }

        private void ChangeOrderForNormal( Window window )
        {
            int startIndex = _positionNormal.Count;

            if ( window.position != -1 )
            {
                _positionNormal.RemoveAt( window.position );
                startIndex = window.position;
            }

            _positionNormal.Add( window );

            UpdateWindowPosition( _positionNormal, 0, startIndex );
        }

        private void RemoveFromNormal( Window window )
        {
            if ( window.position != -1 )
            {
                _positionNormal.RemoveAt( window.position );

                UpdateWindowPosition( _positionNormal, 0, window.position );

                window.ChangeOrder( -1 );
                window.position = -1;
            }
        }

        private void ChangeOrderForPopup( Window window )
        {
            int startIndex = _positionPopup.Count;

            if ( window.position != -1 )
            {
                _positionPopup.RemoveAt( window.position );
                startIndex = window.position;
            }

            _positionPopup.Add( window );

            UpdateWindowPosition( _positionPopup, _preferences.popupBaseOrder, startIndex );
        }

        private void RemoveFromPopup( Window window )
        {
            if ( window.position != -1 )
            {
                _positionPopup.RemoveAt( window.position );

                UpdateWindowPosition( _positionPopup, _preferences.popupBaseOrder, window.position );

                window.ChangeOrder( -1 );
                window.position = -1;
            }
        }

        private void ChangeOrderForModal( Window window )
        {
            int startIndex = _modalWindows.Count;

            if ( window.position != -1 )
            {
                _modalWindows.RemoveAt( window.position );
                startIndex = window.position;
            }

            _modalWindows.Add( window );

            UpdateWindowPosition( _modalWindows, _preferences.modalBaseOrder, startIndex );
        }

        private void RemoveFromModal( Window window )
        {
            if ( window.position != -1 )
            {
                _modalWindows.RemoveAt( window.position );

                UpdateWindowPosition( _modalWindows, _preferences.modalBaseOrder, window.position );

                window.ChangeOrder( -1 );
                window.position = -1;
            }
        }

    }

}