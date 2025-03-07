#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowEvents.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{

    [System.Serializable]
    public sealed class WindowEvent : UnityEngine.Events.UnityEvent
    {

    }

    [System.Serializable]
    public sealed class WindowEventWithArgs : UnityEngine.Events.UnityEvent<string>
    {

    }

}
