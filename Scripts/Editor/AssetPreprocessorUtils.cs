using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public static class AssetPreprocessorUtils
    {
        /// <summary>
        /// Extracts the asset name from an asset path.
        /// </summary>
        /// <param name="assetPath">Full path of an asset.</param>
        /// <returns>Name of asset.</returns>
        public static string GetAssetNameFromPath(string assetPath)
        {
            var fileNameWithExtension = assetPath.Split('/').Last();
            
            return fileNameWithExtension.Split('.').First();
        }

        /// <summary>
        /// Checks if any regex strings in the list match the target string.
        /// </summary>
        /// <param name="regexStringList">List of regex strings.</param>
        /// <param name="targetString">Target string to compare the list of regex strings against.</param>
        /// <returns>Whether any regex strings in the list match the target string.</returns>
        public static bool DoesRegexStringListMatchString(List<string> regexStringList, string targetString)
        {
            var match = regexStringList.Find(regexString =>
            {
                // https://stackoverflow.com/questions/15275718/regular-expression-wildcard
                regexString = Regex.Escape(regexString).Replace("\\*", ".*?");

                return new Regex(regexString).IsMatch(targetString);
            });
            
            return match != null;
        }
        
        /// <summary>
        /// Searches the Asset Database to find all instances of a type deriving the ScriptableObject class.
        /// </summary>
        /// <param name="folders">Option for limiting the search to explicit folders.</param>
        /// <typeparam name="T">Type deriving the ScriptableObject class.</typeparam>
        /// <returns>A list of instances of the type found in the Asset Database.</returns>
        public static List<T> GetScriptableObjectsOfType<T>(string[] folders = null) where T : ScriptableObject
        {
            const string searchString = "t:ScriptableObject";

            var scriptableObjects = GetAssetsBySearchString<ScriptableObject>(searchString, folders);
            
            var output = new List<T>();

            foreach (var scriptableObject in scriptableObjects)
            {
                if (scriptableObject is T)
                {
                    output.Add(scriptableObject as T);
                }
            }
            
            return output;
        }

        /// <summary>
        /// Searches the Asset Database to find all assets matching a search string. Casts each asset to the type T.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="folders">Option for limiting the search to explicit folders.</param>
        /// <typeparam name="T">Type deriving the Unity Object class.</typeparam>
        /// <returns>A list of instances of the type found in the Asset Database.</returns>
        private static List<T> GetAssetsBySearchString<T>(string searchString, string[] folders = null) where T : Object
        {
            var allObjects = AssetDatabase.FindAssets(searchString, folders)
                .ToList()
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => (T) AssetDatabase.LoadAssetAtPath(path, typeof(T)))
                .Where(obj => obj != null)
                .ToList();

            return allObjects;
        }
    }
}
