#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="Window.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{

    [UnityEngine.RequireComponent( typeof( UnityEngine.Canvas ) ), UnityEngine.DisallowMultipleComponent]
    public sealed class Window : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField, Extensions.Disabled]
        private WindowAdapter _adapter = null;

        [UnityEngine.SerializeField, Extensions.Disabled]
        private UnityEngine.Canvas _canvas = null;

        [UnityEngine.SerializeField, WindowId( true )]
        private string _windowId = string.Empty;

        [UnityEngine.SerializeField]
        private string _primaryArgs = null;

        [UnityEngine.SerializeField]
        private WindowType _windowType = WindowType.Normal;

        [UnityEngine.SerializeField, UnityEngine.Min( -1 )]
        private int _presetOrder = -1;

        [UnityEngine.SerializeField]
        private bool _interactable = true;

        [UnityEngine.SerializeField]
        private bool _showOnStart = false;

        [UnityEngine.Header( "Events" )]
        public WindowEvent onStartEvent;
        public WindowEvent onFocusEvent;
        public WindowEventWithArgs onFocusEventWithArgs;
        public WindowEvent onCloseEvent;

        private WindowManager _runtimeManager = null;
        private bool _wasCreated = false;
        private bool _wasDisplayed = false;

        public string windowId => _windowId;
        public WindowType windowType => _windowType;
        public int windowOrder => _canvas.sortingOrder;
        public bool isVisible => _canvas.enabled;
        public bool interactable => _interactable;

        internal int position { get; set; } = -1;

        public void Show( string args )
        {
            WindowManager.Show( _windowId, args );
        }

        public void Hide()
        {
            WindowManager.Hide( _windowId );
        }

        internal void ChangeVisible( WindowManager windowManager, string args, bool yesNo, bool restore )
        {
            var visibilityChanged = false;

            _runtimeManager = windowManager;

            if ( _canvas.enabled != yesNo )
            {
                if ( ( _adapter != null ) && ( yesNo == false ) )
                {
                    _adapter.onHideEvent.AddListener( OnHideByAdapter );
                    _adapter.Hide();
                    return;
                }

                _canvas.enabled = yesNo;
                visibilityChanged = true;

                if ( yesNo == false )
                {
                    _runtimeManager.Release( this );
                    _runtimeManager = null;

                    onCloseEvent?.Invoke();

                    _wasDisplayed = false;
                }
            }

            if ( yesNo == true )
            {
                _runtimeManager.Focus( this );

                if ( restore == false )
                {
                    if ( ( visibilityChanged == true ) && ( _adapter != null ) )
                    {
                        _adapter.onShowEvent.AddListener( OnShowByAdapter );
                        _adapter.args = args;

                        _adapter.Show();
                    }
                    else
                    {
                        if ( !string.IsNullOrEmpty( args ) )
                        {
                            onFocusEventWithArgs?.Invoke( args );
                        }
                        else
                        {
                            onFocusEvent?.Invoke();
                        }
                    }
                }

                _wasDisplayed = true;
            }
        }

        internal void ChangeOrder( int value )
        {
            if ( ( _presetOrder == -1 ) && ( _canvas != null ) )
            {
                _canvas.sortingOrder = UnityEngine.Mathf.Max( -1, value );
            }
        }

        internal void InterruptShow()
        {
            if ( _wasDisplayed == true )
            {
                _canvas.enabled = false;

                onCloseEvent?.Invoke();

                _wasDisplayed = false;
            }
        }

        private void OnHideByAdapter()
        {
            _adapter.onHideEvent.RemoveListener( OnHideByAdapter );

            _canvas.enabled = false;
            _runtimeManager.Release( this );
            _runtimeManager = null;

            onCloseEvent?.Invoke();
        }

        private void OnShowByAdapter()
        {
            _adapter.onShowEvent.RemoveListener( OnShowByAdapter );

            var args = _adapter.args;

            if ( !string.IsNullOrEmpty( args ) )
            {
                onFocusEventWithArgs?.Invoke( args );

                _adapter.args = null;
            }
            else
            {
                onFocusEvent?.Invoke();
            }
        }

        private void Awake()
        {
            WindowManager.Register( this );
        }

        private void OnDestroy()
        {
            WindowManager.Unregister( this );
        }

        private System.Collections.IEnumerator Start()
        {
            _canvas.sortingOrder = _presetOrder;
            _canvas.enabled = false;

            yield return new UnityEngine.WaitForEndOfFrame();

            onStartEvent?.Invoke();

            if ( _showOnStart )
            {
                WindowManager.Show( _windowId, _primaryArgs );
            }

            _wasCreated = true;
        }

        private void OnEnable()
        {
            if ( ( _wasCreated == true ) && ( _wasDisplayed == true ) )
            {
                WindowManager.Restore( _windowId );
            }
        }

        private void OnDisable()
        {
            if ( ( _wasCreated == true ) && ( _wasDisplayed == true ) )
            {
                _runtimeManager.Release( this );
                _runtimeManager = null;
            }
        }

#if UNITY_EDITOR
        [UnityEngine.ContextMenu( "Window/Update Links" )]
        private void UpdateLinks()
        {
            _adapter = GetComponent<WindowAdapter>();
            _canvas = GetComponent<UnityEngine.Canvas>();
        }

        [UnityEngine.ContextMenu( "Window/Debug/Show", true )]
        private bool DebugShowValidate()
        {
            return UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying;
        }

        [UnityEngine.ContextMenu( "Window/Debug/Show", false )]
        private void DebugShow()
        {
            WindowManager.Show( _windowId, null );
        }

        [UnityEngine.ContextMenu( "Window/Debug/Hide", true )]
        private bool DebugHideValidate()
        {
            return UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying;
        }

        [UnityEngine.ContextMenu( "Window/Debug/Hide", false )]
        private void DebugHide()
        {
            WindowManager.Hide( _windowId );
        }

        private void Reset()
        {
            UpdateLinks();
        }
#endif

    }

}
