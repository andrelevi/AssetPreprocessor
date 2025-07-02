using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public class AudioPreprocessor : AssetPostprocessor
    {
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessAudio.html
        /// 
        /// IMPORTANT:
        /// Use OnPostprocessAudio() hook instead of OnPreprocessAudio() since OnPostprocessAudio gives a reference to
        /// the AudioClip, which is needed for AudioClip.length.
        /// </summary>
        private void OnPostprocessAudio(AudioClip audioClip)
        {
            var importer = assetImporter as AudioImporter;

            if (importer == null) return;

            var assetPath = importer.assetPath;
            var assetName = AssetPreprocessorUtils.GetAssetNameFromPath(importer.assetPath);
            var platformName = EditorUserBuildSettings.activeBuildTarget.ToString();

            if (audioClip == null)
            {
                Debug.LogError($"{typeof(AudioClip)} is null. Path: {assetPath}", importer);
                
                return;
            }

            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<AudioPreprocessorConfig>();

            if (configs.Count == 0)
            {
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
                
                // Check that the AudioClip length is under the config's limit for clip length.
                if (audioClip.length > configToTest.MaxClipLengthInSeconds) continue;

                // Found matching config.
                config = configToTest;
                
                Debug.Log($"Processing: <color=#2ECC71>{assetName}</color>", importer);
                Debug.Log($"Using: <color=#3498DB>{config.name}</color>", config);
                break;
            }

            // If could not find a matching config, don't process the asset.
            if (config == null) return;

            var hasSampleSettingsOverrides = false;

            if (importer.loadInBackground != config.LoadInBackground)
            {
                importer.loadInBackground = config.LoadInBackground;
                hasSampleSettingsOverrides = true;
            }

            if (importer.preloadAudioData != config.PreloadAudioData)
            {
                importer.preloadAudioData = config.PreloadAudioData;
                hasSampleSettingsOverrides = true;
            }

            if (importer.forceToMono != config.ForceToMono)
            {
                importer.forceToMono = config.ForceToMono;
                hasSampleSettingsOverrides = true;
            }
            
            if (importer.ambisonic != config.Ambisonic)
            {
                importer.ambisonic = config.Ambisonic;
                hasSampleSettingsOverrides = true;
            }

            var sampleSettings = importer.GetOverrideSampleSettings(platformName);

            if (sampleSettings.loadType != config.AudioClipLoadType)
            {
                sampleSettings.loadType = config.AudioClipLoadType;
                hasSampleSettingsOverrides = true;
            }

            if (sampleSettings.compressionFormat != config.AudioCompressionFormat)
            {
                sampleSettings.compressionFormat = config.AudioCompressionFormat;
                hasSampleSettingsOverrides = true;
            }

            if (System.Math.Abs(sampleSettings.quality - config.Quality) > 0.01f)
            {
                sampleSettings.quality = config.Quality;
                hasSampleSettingsOverrides = true;
            }

            if (sampleSettings.sampleRateSetting != config.AudioSampleRateSetting)
            {
                sampleSettings.sampleRateSetting = config.AudioSampleRateSetting;
                hasSampleSettingsOverrides = true;
            }

            if (hasSampleSettingsOverrides || config.ForcePreprocess)
            {
                config.PlatformsRegexList.ForEach(platformRegexString => importer.SetOverrideSampleSettings(platformRegexString, sampleSettings));
                
                // Be sure to set the platform override for the current platform string, in case the current platform was
                // NOT a perfect match to one of the platform regex strings.
                importer.SetOverrideSampleSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), sampleSettings);
            }
        }
    }
}