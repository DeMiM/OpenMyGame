#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowPreferences.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{

    [System.Serializable]
    public sealed class WindowPreferences : UnityEngine.ScriptableObject
    {
        [UnityEngine.SerializeField, UnityEngine.Range( 1000, 1999 )]
        private int _popupBaseOrder = 1000;

        [UnityEngine.SerializeField, UnityEngine.Min( 2000 )]
        private int _modalBaseOrder = 2000;

        public int popupBaseOrder => _popupBaseOrder;
        public int modalBaseOrder => _modalBaseOrder;

    }

}
