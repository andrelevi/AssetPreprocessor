using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public abstract class BasePreprocessorConfig : ScriptableObject
    {
        [Header("Config Settings")]
        
        [Tooltip("Whether this config should be considered when processing assets.")]
        public bool IsEnabled = true;
        
        [Tooltip("Some assets will not be preprocessed again if they already match certain criteria. Enable this to always force preprocessing.")]
        public bool ForcePreprocess;
        
        [Tooltip("Configs with lower sort order are checked first.")]
        public int ConfigSortOrder = 10;

        [Header("Asset Path Regex Matching")]
        
        [Tooltip("Config will be used if asset path matches regex list. Use a * wildcard to match all.")]
        public List<string> MatchRegexList = new List<string>();

        [Tooltip("Any asset path matching the ignore regex list will be ignored.")]
        public List<string> IgnoreRegexList = new List<string>();

        public virtual bool ShouldUseConfigForAssetImporter(AssetImporter assetImporter)
        {
            if (!IsEnabled) return false;

            var assetPath = assetImporter.assetPath;

            if (!AssetPreprocessorUtils.DoesRegexStringListMatchString(MatchRegexList, assetPath)) return false;

            // Asset path should NOT match any of the regex checks.
            if (AssetPreprocessorUtils.DoesRegexStringListMatchString(IgnoreRegexList, assetPath)) return false;
            
            return true;
        }
    }
}
