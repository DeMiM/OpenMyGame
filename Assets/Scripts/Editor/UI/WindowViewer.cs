#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowViewer.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    internal sealed class WindowViewer : UnityEditor.EditorWindow
    {
        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
        private UnityEditor.IMGUI.Controls.TreeViewState _viewState = null;

        private WindowManager _windowManager = null;
        private WindowTree _windowTree = null;

        private UnityEngine.GUIContent[] _windowNames = null;
        private string[] _windowIDs = null;
        private int _windowSelect = -1;

        private string _windowArguments = null;

        [UnityEditor.InitializeOnLoadMethod]
        private static void Initialize()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChangedHandler;
        }

        private static void OnPlayModeStateChangedHandler( UnityEditor.PlayModeStateChange playMode )
        {
            if ( playMode == UnityEditor.PlayModeStateChange.EnteredPlayMode )
            {
                WindowViewer windowViewer = EditorWindowUtility.GetWindow<WindowViewer>();
                if ( windowViewer != null )
                    windowViewer.SwitchMode( true );
            }

            if ( playMode == UnityEditor.PlayModeStateChange.ExitingPlayMode )
            {
                WindowViewer windowViewer = EditorWindowUtility.GetWindow<WindowViewer>();
                if ( windowViewer != null )
                    windowViewer.SwitchMode( false );
            }
        }

        private void FetchWindows()
        {
            System.Reflection.FieldInfo fieldInfo = typeof( WindowManager ).GetField( "s_instance",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic );
            _windowManager = fieldInfo?.GetValue( null ) as WindowManager;
            _windowTree = new WindowTree( _viewState, CreateMultiColumnHeader(), _windowManager );

            fieldInfo = _windowManager?.GetType().GetField( "_registry",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );
            System.Collections.Generic.Dictionary<string, Window> registry = fieldInfo?.GetValue( _windowManager )
                as System.Collections.Generic.Dictionary<string, Window>;
            _windowNames = new UnityEngine.GUIContent[ registry?.Count ?? 0 ];
            _windowIDs = new string[ _windowNames.Length ];

            if ( registry != null )
            {
                int contentIndex = 0;
                foreach ( System.Collections.Generic.KeyValuePair<string, Window> pair in registry )
                {
                    _windowNames[ contentIndex ] = new UnityEngine.GUIContent( pair.Key );
                    _windowIDs[ contentIndex ] = pair.Key;

                    ++contentIndex;
                }
            }
        }

        private void Release()
        {
            if ( _windowTree != null )
            {
                _windowTree.GetRows().Clear();
                _windowTree = null;
            }

            _windowManager = null;
            _windowSelect = -1;
            _windowNames = null;
            _windowIDs = null;

            System.GC.SuppressFinalize( this );
        }

        private static UnityEditor.IMGUI.Controls.MultiColumnHeader CreateMultiColumnHeader()
        {
            UnityEditor.IMGUI.Controls.MultiColumnHeaderState state = new( new[]
            {
                new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column
                {
                    headerContent = new UnityEngine.GUIContent( "ID" ),
                    headerTextAlignment = UnityEngine.TextAlignment.Center,
                    maxWidth = 900f,
                    minWidth = 100f,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false,
                    width = 300f,
                },
                new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column
                {
                    headerContent = new UnityEngine.GUIContent( "Type" ),
                    headerTextAlignment = UnityEngine.TextAlignment.Center,
                    maxWidth = 200f,
                    minWidth = 50f,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false,
                    width = 150f,
                },
                new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column
                {
                    headerContent = new UnityEngine.GUIContent( "Order" ),
                    headerTextAlignment = UnityEngine.TextAlignment.Center,
                    maxWidth = 100f,
                    minWidth = 100f,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false,
                    width = 100f,
                }
            } );

            return new UnityEditor.IMGUI.Controls.MultiColumnHeader( state );
        }

        private void SwitchMode( bool isPlaying )
        {
            Release();

            if ( isPlaying )
            {
                FetchWindows();
            }
        }

        private void OnUpdateStates()
        {
            if ( UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode )
            {
                if ( _windowManager != null )
                {
                    if ( _windowManager.changed == true )
                    {
                        _windowManager.changed = false;
                        _windowManager.dirty = true;

                        _windowSelect = -1;
                        _windowNames = null;
                        _windowIDs = null;

                        System.GC.SuppressFinalize( this );

                        FetchWindows();
                    }

                    if ( _windowManager.dirty == true )
                    {
                        _windowManager.dirty = false;
                        _windowTree?.Reload();

                        Repaint();
                    }
                }
            }
        }

        private void OnEnable()
        {
            UnityEditor.EditorApplication.update += OnUpdateStates;

            if ( _viewState == null )
            {
                _viewState = new UnityEditor.IMGUI.Controls.TreeViewState();
            }

            SwitchMode( UnityEditor.EditorApplication.isPlaying );
        }

        private void OnDisable()
        {
            UnityEditor.EditorApplication.update -= OnUpdateStates;

            Release();
        }

        private void OnGUI()
        {
            if ( _windowTree != null )
            {
                UnityEngine.Rect rect = new( 2f, 2f, position.width - 4f, position.height - 4f );
                UnityEngine.Rect rectWindowTree = new( rect.x, rect.y, rect.width, rect.height - UnityEditor.EditorGUIUtility.singleLineHeight * 1.25f - 2f );
                UnityEngine.Rect rectToolBar = new( rect.x, rectWindowTree.yMax + 2f, rect.width, UnityEditor.EditorGUIUtility.singleLineHeight * 1.25f );
                UnityEngine.Rect rectToolBarGroup = new( rect.x + 2f, rectToolBar.y + 3f, rect.width - 5f, rectToolBar.height - 4f );
                UnityEngine.Rect rectListWindowIds = new( 0f, 0f, 250f, rectToolBarGroup.height );
                UnityEngine.Rect rectButtonShow = new( rectListWindowIds.xMax + 4f, 0f, 56f, rectToolBarGroup.height );
                UnityEngine.Rect rectButtonHide = new( rectButtonShow.xMax + 2f, 0f, 56f, rectToolBarGroup.height );
                UnityEngine.Rect rectButtonPing = new( rectButtonHide.xMax + 2f, 0f, 56f, rectToolBarGroup.height );
                UnityEngine.Rect rectArguments = new( rectButtonPing.xMax + 4f, 0f, rect.width - rectButtonPing.xMax + 4f - rect.x, rectToolBarGroup.height - 1f );

                _windowTree.OnGUI( rectWindowTree );

                UnityEngine.GUI.Label( rectToolBar, UnityEngine.GUIContent.none, Styles.frameBox );
                using ( new UnityEngine.GUI.GroupScope( rectToolBarGroup ) )
                {
                    using ( new UnityEditor.EditorGUI.DisabledScope( _windowNames == null || _windowNames.Length == 0 ) )
                    {
                        _windowSelect = UnityEditor.EditorGUI.Popup( rectListWindowIds,
                            UnityEngine.GUIContent.none, _windowSelect, _windowNames );

                        if ( _windowSelect != -1 )
                        {
                            if ( WindowManager.Find( _windowIDs[ _windowSelect ], out Window window ) )
                            {
                                bool disabled = ( window.enabled == false ) || ( window.gameObject.activeInHierarchy == false );

                                using ( new UnityEditor.EditorGUI.DisabledGroupScope( disabled ) )
                                {
                                    if ( UnityEngine.GUI.Button( rectButtonShow, Content.buttonShow, Styles.smallButton ) )
                                    {
                                        WindowManager.Show( _windowIDs[ _windowSelect ], _windowArguments );
                                    }

                                    if ( UnityEngine.GUI.Button( rectButtonHide, Content.buttonHide, Styles.smallButton ) )
                                    {
                                        WindowManager.Hide( _windowIDs[ _windowSelect ] );
                                    }
                                }

                                if ( UnityEngine.GUI.Button( rectButtonPing, Content.buttonPing, Styles.smallButton ) )
                                {
                                    UnityEditor.EditorGUIUtility.PingObject( window.gameObject.GetInstanceID() );
                                }

                                float labelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                                UnityEditor.EditorGUIUtility.labelWidth = 65f;
                                _windowArguments = UnityEditor.EditorGUI.TextField( rectArguments,
                                    Content.arguments, _windowArguments );
                                UnityEditor.EditorGUIUtility.labelWidth = labelWidth;
                            }
                        }
                    }
                }
            }
        }

        private static class Content
        {
            public readonly static UnityEngine.GUIContent buttonShow = new( "Show" );
            public readonly static UnityEngine.GUIContent buttonHide = new( "Hide" );
            public readonly static UnityEngine.GUIContent buttonPing = new( "Ping" );
            public readonly static UnityEngine.GUIContent arguments = new( "Arguments" );
        }

        private static class Integration
        {
            private static bool CreateWindowValidate()
            {
                return UnityEditor.Selection.activeTransform == null && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == false;
            }

            private static void CreateWindow( UnityEngine.RenderMode renderMode )
            {
                UnityEngine.GameObject gameObject = new( "Window" );

                UnityEditor.GameObjectUtility.SetParentAndAlign( gameObject, null );
                gameObject.layer = UnityEngine.LayerMask.NameToLayer( "UI" );

                UnityEngine.Canvas canvas = gameObject.AddComponent<UnityEngine.Canvas>();
                canvas.renderMode = renderMode;
                canvas.pixelPerfect = false;
                canvas.sortingOrder = -1;
                canvas.targetDisplay = 0;
                canvas.additionalShaderChannels = UnityEngine.AdditionalCanvasShaderChannels.None;

                UnityEngine.UI.CanvasScaler canvasScaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new UnityEngine.Vector2( 2732f, 1536f );
                canvasScaler.matchWidthOrHeight = 1f;
                canvasScaler.referencePixelsPerUnit = 100f;

                UnityEngine.UI.GraphicRaycaster graphicRaycaster = gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                graphicRaycaster.ignoreReversedGraphics = true;
                graphicRaycaster.blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None;

                gameObject.AddComponent<Window>();

                UnityEditor.Undo.RegisterCreatedObjectUndo( gameObject, "Create Window (Test)" );
                UnityEditor.Selection.activeObject = gameObject;
            }

            [UnityEditor.MenuItem( "Test/Windows/Viewer", false, 1200 )]
            private static void ShowViewer()
            {
                EditorWindowUtility.ShowWindow<WindowViewer>( "Windows Viewer", new UnityEngine.Vector2( 384f, 128f ), true );
            }

            [UnityEditor.MenuItem( "GameObject/Test/Windows/Window (Overlay)", true, 12 )]
            private static bool CreateWindowOverlayValidate()
            {
                return CreateWindowValidate();
            }

            [UnityEditor.MenuItem( "GameObject/Test/Windows/Window (Overlay)", false, 12 )]
            private static void CreateWindowOverlay()
            {
                CreateWindow( UnityEngine.RenderMode.ScreenSpaceOverlay );
            }

            [UnityEditor.MenuItem( "GameObject/Test/Windows/Window (Camera)", true, 13 )]
            private static bool CreateWindowCameraValidate()
            {
                return CreateWindowValidate();
            }

            [UnityEditor.MenuItem( "GameObject/Test/Windows/Window (Camera)", false, 13 )]
            private static void CreateWindowCamera()
            {
                CreateWindow( UnityEngine.RenderMode.ScreenSpaceCamera );
            }

        }

    }

}
