using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public class AudioPreprocessor : AssetPostprocessor
    {
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessAudio.html
        /// </summary>
        private void OnPreprocessAudio()
        {
            var importer = assetImporter as AudioImporter;

            if (importer == null) return;

            var assetPath = importer.assetPath;
            var assetName = AssetPreprocessorUtils.GetAssetNameFromPath(importer.assetPath);
            var platformName = EditorUserBuildSettings.activeBuildTarget.ToString();
            var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            // BUG:
            //
            // Unity 2019.1.9f1 - OSX
            //
            // When an AudioClip is first imported into the project, attempting to load it in OnPreprocessAudio()
            // as an AudioClip results in null. Forcing an AssetDatabase.Refresh() will then allow the asset
            // to be properly loaded as an AudioClip in the subsequent OnPreprocessAudio() hook.
            if (audioClip == null)
            {
                // Workaround for when an AudioClip is loaded as null for its first OnPreprocessAudio().
                AssetDatabase.Refresh();
                return;
            }

            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<AudioPreprocessorConfig>();

            if (configs.Count == 0)
            {
                Debug.Log($"Could not find a {nameof(AudioPreprocessorConfig)} in project.");

                return;
            }

            configs = configs
                .Where(conf => conf.ShouldUseConfigForAssetImporter(assetImporter))
                .ToList();
            
            configs.Sort((config1, config2) => config1.ConfigSortOrder.CompareTo(config2.ConfigSortOrder));

            AudioPreprocessorConfig config = null;

            for (var i = 0; i < configs.Count; i++)
            {
                var configToTest = configs[i];

                if (!AssetPreprocessorUtils.DoesRegexStringListMatchString(configToTest.PlatformsRegexList, platformName)) continue;
                
                if (audioClip.length > configToTest.MaxClipLengthInSeconds) continue;

                // Found matching config.
                config = configToTest;

                Debug.Log($"Processing: {assetName}", audioClip);
                Debug.Log($"Using: {config.name}", config);
                break;
            }

            // If could not find a matching config, don't process the asset.
            if (config == null) return;

            var needsReimport = false;

            if (importer.loadInBackground != config.LoadInBackground)
            {
                importer.loadInBackground = config.LoadInBackground;
                needsReimport = true;
            }

            if (importer.preloadAudioData != config.PreloadAudioData)
            {
                importer.preloadAudioData = config.PreloadAudioData;
                needsReimport = true;
            }

            if (importer.forceToMono != config.ForceToMono)
            {
                importer.forceToMono = config.ForceToMono;
                needsReimport = true;
            }
            
            if (importer.ambisonic != config.Ambisonic)
            {
                importer.ambisonic = config.Ambisonic;
                needsReimport = true;
            }

            var sampleSettings = importer.GetOverrideSampleSettings(platformName);

            if (sampleSettings.loadType != config.AudioClipLoadType)
            {
                sampleSettings.loadType = config.AudioClipLoadType;
                needsReimport = true;
            }

            if (sampleSettings.compressionFormat != config.AudioCompressionFormat)
            {
                sampleSettings.compressionFormat = config.AudioCompressionFormat;
                needsReimport = true;
            }

            if (System.Math.Abs(sampleSettings.quality - config.Quality) > 0.01f)
            {
                sampleSettings.quality = config.Quality;
                needsReimport = true;
            }

            if (sampleSettings.sampleRateSetting != config.AudioSampleRateSetting)
            {
                sampleSettings.sampleRateSetting = config.AudioSampleRateSetting;
                needsReimport = true;
            }

            if (needsReimport || config.ForcePreprocess)
            {
                config.PlatformsRegexList.ForEach(platformRegexString => importer.SetOverrideSampleSettings(platformRegexString, sampleSettings));
                
                // Be sure to set the platform override for the current platform string, in case the current platform was
                // NOT a perfect match to one of the platform regex strings.
                importer.SetOverrideSampleSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), sampleSettings);
                
                Debug.Log($"Processed: {audioClip}", audioClip);
            }
            else
            {
                Debug.Log($"Skipping reimport for: {audioClip}", audioClip);
            }
        }
    }
}
