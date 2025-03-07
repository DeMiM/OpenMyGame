#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="Styles.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    public static class Styles
    {
        public static readonly UnityEngine.GUIStyle frame = new( "FrameBox" );
        public static readonly UnityEngine.GUIStyle frameBox = new( "HelpBox" );
        public static readonly UnityEngine.GUIStyle inspectorHeader = new( "BoldLabel" );
        public static readonly UnityEngine.GUIStyle helpString = new( "MiniBoldLabel" );
        public static readonly UnityEngine.GUIStyle smallButton = new( "MiniButton" );
        public static readonly UnityEngine.GUIStyle labelCenter = CreateLabelCenter();

        private static UnityEngine.GUIStyle CreateLabelCenter()
        {
            return new UnityEngine.GUIStyle( "Label" )
            {
                alignment = UnityEngine.TextAnchor.UpperCenter
            };
        }

    }

}
