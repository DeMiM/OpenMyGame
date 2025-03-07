#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="WindowBuilder.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{
    internal static class WindowBuilder
    {
        private const string kEnumDataPathKey = "test_windows_enum_data_path";

        private static UnityEngine.TextAsset s_enumAsset = default;
        private static System.Collections.Generic.List<string> s_windowNames = default;
        private static string[] s_availableWindowNames = default;

        internal static UnityEngine.TextAsset enumAsset
        {
            set => ChangeEnumAsset( value, true );
            get => s_enumAsset;
        }

        internal static System.Collections.Generic.List<string> windowNames => s_windowNames;
        internal static string[] availableWindowNames => s_availableWindowNames;

        internal static bool TryAddWindowName( string name, string last, out string id )
        {
            System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex( @"[^0-9a-zA-Z]+" );

            name = pattern.Replace( name, string.Empty );

            if ( string.IsNullOrEmpty( name ) == false )
            {
                int index = s_windowNames.FindIndex( wn => wn.Equals( name, System.StringComparison.OrdinalIgnoreCase ) );

                if ( index == -1 )
                {
                    string removedName = string.Format( "_{0}", name );

                    index = s_windowNames.FindIndex( wn => wn.Equals( removedName, System.StringComparison.OrdinalIgnoreCase ) );
                }

                id = name;

                if ( index != -1 )
                {
                    s_windowNames[ index ] = name;
                }
                else
                {
                    s_windowNames.Add( name );
                }

                if ( string.IsNullOrEmpty( last ) == false )
                {
                    index = s_windowNames.FindIndex( wn => wn.Equals( last, System.StringComparison.OrdinalIgnoreCase ) );

                    if ( index != -1 )
                    {
                        s_windowNames[ index ] = string.Format( "_{0}", last );
                    }
                }

                UnityEditor.EditorApplication.delayCall += OnUpdateEnumsFromListHandler;

                return true;
            }

            id = string.Empty;

            return false;
        }

        internal static string GetWindowIdByIndex( int index )
        {
            if ( index > -1 && index < s_availableWindowNames.Length )
            {
                int found = s_windowNames.FindIndex( name => name.Equals( s_availableWindowNames[ index ],
                    System.StringComparison.OrdinalIgnoreCase ) );

                if ( found != -1 )
                {
                    return s_availableWindowNames[ index ];
                }
            }

            return string.Empty;
        }

        internal static int GetIndexByWindowId( string id )
        {
            if ( string.IsNullOrEmpty( id ) == false )
            {
                int index = s_windowNames.FindIndex( wn => wn.Equals( id, System.StringComparison.OrdinalIgnoreCase ) );

                if ( index != -1 )
                {
                    return System.Array.FindIndex( s_availableWindowNames, 0, name => string.IsNullOrEmpty( name ) == false
                        && name.Equals( id, System.StringComparison.OrdinalIgnoreCase ) );
                }
            }

            return -1;
        }

        internal static void RemoveWindowId( string id )
        {
            if ( string.IsNullOrEmpty( id ) == false )
            {
                int index = s_windowNames.FindIndex( wn => wn.Equals( id, System.StringComparison.OrdinalIgnoreCase ) );

                if ( index != -1 )
                {
                    s_windowNames[ index ] = string.Format( "_{0}", id );

                    UnityEditor.EditorApplication.delayCall += OnUpdateEnumsFromListHandler;
                }
            }
        }

        [UnityEditor.InitializeOnLoadMethod]
        private static void Initialize()
        {
            string assetPath = UnityEditor.EditorPrefs.GetString( kEnumDataPathKey, string.Empty );

            if ( string.IsNullOrEmpty( assetPath ) == false )
            {
                s_enumAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>( assetPath );
            }

            s_windowNames = new System.Collections.Generic.List<string>( 1024 )
            {
                "Unknown"
            };

            if ( s_enumAsset != null )
            {
                RefreshEnums( s_enumAsset );
                RefreshAvailableWindowNames();
            }

        }

        private static void OnUpdateEnumsFromListHandler()
        {
            UnityEditor.EditorApplication.delayCall -= OnUpdateEnumsFromListHandler;

            RefreshAvailableWindowNames();

            if ( s_enumAsset != null )
            {
                FlushEnumsToAsset( s_enumAsset );
            }
        }

        private static void RefreshEnums( UnityEngine.TextAsset enumAsset )
        {
            int beginIndex = enumAsset.text.LastIndexOf( '{' ) + 1;
            int endIndex = enumAsset.text.IndexOf( '}' ) - 1;

            if ( ( beginIndex > 0 ) && ( beginIndex < endIndex ) )
            {
                System.Text.RegularExpressions.Regex pettern = new System.Text.RegularExpressions.Regex( @"[\s]" );

                string[] enums = pettern.Replace( enumAsset.text.Substring( beginIndex, endIndex - beginIndex ), string.Empty )
                    .Split( new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries );

                RemoveEnums();

                if ( enums.Length > 0 )
                {
                    s_windowNames.AddRange( enums );

                    for ( int i = 1; i < s_windowNames.Count; ++i )
                    {
                        int separatorIndex = s_windowNames[ i ].LastIndexOf( '=' );

                        if ( separatorIndex != -1 )
                        {
                            s_windowNames[ i ] = s_windowNames[ i ].Substring( 0, separatorIndex );
                        }
                    }
                }
            }
        }

        private static void RemoveEnums()
        {
            s_windowNames.Clear();
            s_windowNames.Add( "Unknown" );
        }

        private static void FlushEnumsToAsset( UnityEngine.TextAsset enumAsset )
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath( enumAsset );

            using ( System.IO.StreamWriter streamWriter = new( assetPath, false, System.Text.Encoding.UTF8 ) )
            {
                streamWriter.WriteLine( "// PRODUCT: Auto-generated window names" );
                streamWriter.WriteLine( "// IMPORTANT: Do not change manually!" );
                streamWriter.WriteLine( "" );
                streamWriter.WriteLine( "namespace Wigro.Windows" );
                streamWriter.WriteLine( "{" );
                streamWriter.WriteLine( "\tpublic enum WindowNames : int" );
                streamWriter.WriteLine( "\t{" );
                for ( var index = 1; index < s_windowNames.Count; ++index )
                {
                    var enumName = s_windowNames[ index ];

                    if ( enumName.StartsWith( "_" ) == true )
                    {
                        streamWriter.WriteLine( $"\t\t{enumName} = -{index}," );
                    }
                    else
                    {
                        streamWriter.WriteLine( $"\t\t{enumName} = {index}," );
                    }

                }
                streamWriter.WriteLine( "\t}" );
                streamWriter.WriteLine( "}" );
            }

            UnityEditor.AssetDatabase.Refresh( UnityEditor.ImportAssetOptions.ForceUpdate );
        }

        private static void ChangeEnumAsset( UnityEngine.TextAsset newAsset, bool @override )
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath( newAsset );

            if ( s_enumAsset != null )
            {
                if ( @override == true )
                {
                    using ( System.IO.StreamWriter streamWriter = new( assetPath, false, System.Text.Encoding.UTF8 ) )
                    {
                        streamWriter.Write( s_enumAsset.text );
                    }
                }

                UnityEngine.Resources.UnloadAsset( s_enumAsset );
            }

            s_enumAsset = newAsset;

            if ( s_enumAsset != null )
            {
                UnityEditor.EditorPrefs.SetString( kEnumDataPathKey, assetPath );

                RefreshEnums( s_enumAsset );
            }
            else
            {
                UnityEditor.EditorPrefs.DeleteKey( kEnumDataPathKey );

                RemoveEnums();
            }

            RefreshAvailableWindowNames();
        }

        private static void RefreshAvailableWindowNames()
        {
            var list = new System.Collections.Generic.List<string>( s_windowNames.Count );

            for ( int i = 1; i < s_windowNames.Count; ++i )
            {
                string name = s_windowNames[ i ];

                if ( name.StartsWith( "_" ) == false )
                {
                    list.Add( name );
                }
            }

            s_availableWindowNames = list.ToArray();
        }

    }

}
