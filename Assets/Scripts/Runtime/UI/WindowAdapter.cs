#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowAdapter.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{

    [UnityEngine.RequireComponent( typeof( Window ) ), UnityEngine.DisallowMultipleComponent]
    public abstract class WindowAdapter : UnityEngine.MonoBehaviour
    {
        [UnityEngine.Header( "Events" )]
        public UnityEngine.Events.UnityEvent onShowStartEvent;
        public UnityEngine.Events.UnityEvent onShowEvent;
        public UnityEngine.Events.UnityEvent onHideStartEvent;
        public UnityEngine.Events.UnityEvent onHideEvent;

        internal string args { get; set; }

        public abstract void Show();
        public abstract void Hide();

#if UNITY_EDITOR
        [UnityEngine.ContextMenu( "WindowAdapter/Update Links" )]
        private void UpdateLinks()
        {
            Window window = GetComponent<Window>();

            window.GetType().GetField( "m_adapter", System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic )?.SetValue( window, this );

            UnityEditor.EditorUtility.SetDirty( window );
        }

        private void Reset()
        {
            UpdateLinks();
        }
#endif

    }

}
