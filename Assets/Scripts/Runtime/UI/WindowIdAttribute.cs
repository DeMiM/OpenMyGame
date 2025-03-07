#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowIdAttribute.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI
{
    public sealed class WindowIdAttribute : UnityEngine.PropertyAttribute
    {
        public readonly bool editable;

        internal WindowIdAttribute( bool editable )
        {
            this.editable = editable;
        }

        public WindowIdAttribute()
        {
            editable = false;
        }

    }

}
