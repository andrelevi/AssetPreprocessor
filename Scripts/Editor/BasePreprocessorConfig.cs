using System.Collections.Generic;
using System.Linq;
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

            if (!AssetPreprocessorUtils.DoesRegexStringListMatchString(FilterOutBadRegexStrings(MatchRegexList, nameof(MatchRegexList)), assetPath)) return false;

            // Check that the Asset path does NOT match any ignore regex strings.
            if (AssetPreprocessorUtils.DoesRegexStringListMatchString(FilterOutBadRegexStrings(IgnoreRegexList, nameof(IgnoreRegexList)), assetPath)) return false;
            
            return true;
        }

        private List<string> FilterOutBadRegexStrings(List<string> regexStrings, string fieldName)
        {
            // Create a copy so that we don't mutate the original list.
            regexStrings = regexStrings.ToList();
            
            for (var i = 0; i < regexStrings.Count; i++)
            {
                var regexString = regexStrings[i];
                
                if (regexString == string.Empty)
                {
                    Debug.LogError($"{name} - {fieldName} - Regex string at index: {i} is empty. Ignoring, otherwise it will match everything.", this);

                    regexStrings.RemoveAt(i);
                    
                    i--;
                }
            }
            
            return regexStrings;
        }
    }
}
