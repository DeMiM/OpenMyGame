#region copyright
/// ------------------------------------------------------------------------
/// <copyright file ="ScriptableObjectUtility.cs">
///     Copyright (c) 2020 - 2025. All rights reserved.
/// </copyright>
///
/// <author>Maksim Mikulski</author>
/// ------------------------------------------------------------------------
#endregion

namespace Test.UI.Editor
{

    public static class ScriptableObjectUtility
    {
        public static T FindInstance<T>( string assetPath, bool genUniquePath )
            where T : UnityEngine.ScriptableObject
        {
            T[] objects = UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if ( objects.Length > 0 )
            {
                return objects[ 0 ];
            }

            string folder = System.IO.Path.GetDirectoryName( assetPath );
            if ( !UnityEditor.AssetDatabase.IsValidFolder( folder ) )
            {
                try
                {
                    System.IO.Directory.CreateDirectory( folder );
                }
                catch ( System.IO.IOException )
                {
                    UnityEngine.Debug.LogAssertion( $"[ScriptableObjectUtility] " +
                        $"Invalid path \"{assetPath}\"!" );
                    return null;
                }

            }

            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if ( asset == null )
            {
                asset = UnityEngine.ScriptableObject.CreateInstance<T>();

                if ( genUniquePath )
                {
                    assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath( assetPath );
                }

                UnityEditor.AssetDatabase.CreateAsset( asset, assetPath );
                UnityEditor.AssetDatabase.SaveAssets();

                UnityEditor.AssetDatabase.Refresh( UnityEditor.ImportAssetOptions.ForceUpdate );
            }

            return asset;
        }

        public static T[] FindInstances<T>( string assetPath, bool genUniquePath )
            where T : UnityEngine.ScriptableObject
        {
            T[] objects = UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if ( objects.Length > 0 )
            {
                return objects;
            }

            string folder = System.IO.Path.GetDirectoryName( assetPath );
            if ( !UnityEditor.AssetDatabase.IsValidFolder( folder ) )
            {
                try
                {
                    System.IO.Directory.CreateDirectory( folder );
                }
                catch ( System.IO.IOException )
                {
                    UnityEngine.Debug.LogAssertion( $"[ScriptableObjectUtility] " +
                        $"Invalid path \"{assetPath}\"!" );
                    return null;
                }

            }

            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if ( asset == null )
            {
                asset = UnityEngine.ScriptableObject.CreateInstance<T>();

                if ( genUniquePath )
                {
                    assetPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath( assetPath );
                }

                UnityEditor.AssetDatabase.CreateAsset( asset, assetPath );
                UnityEditor.AssetDatabase.SaveAssets();

                UnityEditor.AssetDatabase.Refresh( UnityEditor.ImportAssetOptions.ForceUpdate );
            }

            return new T[] { asset };
        }

    }

}
