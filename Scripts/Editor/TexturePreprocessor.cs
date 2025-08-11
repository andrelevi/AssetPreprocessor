using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AssetPreprocessor.Scripts.Editor
{
    class TexturePreprocessor : AssetPostprocessor
    {
        // Ensure post-processor runs after libraries, e.g. Bakery lightmapper.
        public override int GetPostprocessOrder() => 999;
        
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessTexture.html
        /// </summary>
        private void OnPreprocessTexture()
        {
            var importer = (TextureImporter) assetImporter;

            var assetPath = importer.assetPath;
            var textureName = AssetPreprocessorUtils.GetAssetNameFromPath(importer.assetPath);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            var platformName = EditorUserBuildSettings.activeBuildTarget.ToString();
			
            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<TexturePreprocessorConfig>();
            
            if (configs.Count == 0)
            {
                return;
            }
            
            configs = configs
                .Where(conf => conf.ShouldUseConfigForAssetImporter(assetImporter))
                .ToList();
            
            configs.Sort((config1, config2) => config1.ConfigSortOrder.CompareTo(config2.ConfigSortOrder));

            TexturePreprocessorConfig config = null;

            for (var i = 0; i < configs.Count; i++)
            {
                var configToTest = configs[i];
		            
                if (!AssetPreprocessorUtils.DoesRegexStringListMatchString(configToTest.PlatformsRegexList, platformName)) continue;
                
                // Found matching config.
                config = configToTest;
                
                break;
            }
            
            // If could not find a matching config, don't process the texture.
            if (config == null) return;

            var currentPlatform = EditorUserBuildSettings.activeBuildTarget.ToString();
            var currentPlatformSettings = importer.GetPlatformTextureSettings(currentPlatform);
			
            var hasAlpha = importer.DoesSourceTextureHaveAlpha();
            var nativeTextureSize = GetOriginalTextureSize(importer);
            var nativeSize = Mathf.NextPowerOfTwo(Mathf.Max(nativeTextureSize.width, nativeTextureSize.height));
            var currentFormat = currentPlatformSettings.format.ToString();
            
            // Handle when native size for texture is too small. Happens for baked light maps and reflection maps.
            // Need to reimport asset once to get correct native texture size.
            if (nativeSize <= 4)
            {
                AssetDatabase.Refresh();
                return;
            }
            
            Debug.Log($"Processing: <color=#2ECC71>{textureName}</color> | Native size: {nativeSize} | Current format: {currentFormat}", importer);
            Debug.Log($"Using: <color=#3498DB>{config.name}</color>", config);
            
            // If already contains correct texture format, skip adjusting import settings.
            var matchingSkipRegex = config.SkipIfCurrentTextureFormatMatchesRegexList.Find(regexString => new Regex(regexString).IsMatch(currentFormat));
            var alreadyContainsFormat = matchingSkipRegex != null;
            if (!config.ForcePreprocess && alreadyContainsFormat)
            {
                Debug.Log($"Skipping preprocess. Current format matching skip regex: '{matchingSkipRegex}'", texture);
                return;
            }
			
            if (config.EnableReadWrite)
            {
                Debug.Log("Enabling Read/Write.", texture);
                importer.isReadable = true;
            }
            else
            {
                Debug.Log("Disabling Read/Write.", texture);
                importer.isReadable = false;
            }
			
            var maxTextureSize = config.MaxTextureSize;
            var multipliedNativeRes = Mathf.RoundToInt(nativeSize * config.NativeTextureSizeMultiplier);
            var textureSize = Mathf.Min(multipliedNativeRes, maxTextureSize);
			
            var format = hasAlpha ? config.RGBAFormat : config.RGBFormat;

            if (config.ForceLinear)
            {
                importer.sRGBTexture = false;
            }

            if (config.ForceFilterMode)
            {
                importer.anisoLevel = config.AnisoLevel;
                importer.filterMode = config.FilterMode;
            }
            
            importer.mipmapEnabled = config.GenerateMipMaps;
            importer.streamingMipmaps = config.EnableMipMapStreaming;

            SetTextureImporterPlatformSetting(config, importer, texture, textureName, textureSize, format);
        }

        private static void SetTextureImporterPlatformSetting(
            TexturePreprocessorConfig config,
            TextureImporter textureImporter,
            Texture texture,
            string textureName,
            int textureSize,
            TextureImporterFormat format
        )
        {
            Debug.Log($"Setting: {textureSize} | Format: {format} | {textureName}", texture);

            config.PlatformsRegexList.ForEach(platformRegexString =>
            {
                textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                {
                    overridden = true,
                    name = platformRegexString,
                    maxTextureSize = textureSize,
                    format = format,
                    compressionQuality = (int) config.TextureCompressionQuality,
                    allowsAlphaSplitting = false
                });
            });
            
            // Be sure to set the platform override for the current platform string, in case the current platform was
            // NOT a perfect match to one of the platform regex strings.
            textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                overridden = true,
                name = EditorUserBuildSettings.activeBuildTarget.ToString(),
                maxTextureSize = textureSize,
                format = format,
                compressionQuality = (int) config.TextureCompressionQuality,
                allowsAlphaSplitting = false
            });

            textureImporter.npotScale = config.NPOTScale;
        }

        /// <summary>
        /// Hacky way to get the native texture size via the TextureImporter.
        /// https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/
        /// </summary>
        private static Size GetOriginalTextureSize(TextureImporter importer)
        {
            if (_getImageSizeDelegate == null) {
                var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                _getImageSizeDelegate = Delegate.CreateDelegate(typeof(GetImageSize), null, method) as GetImageSize;
            }
 
            var size = new Size();
            
            _getImageSizeDelegate(importer, ref size.width, ref size.height);
 
            return size;
        }
		
        private delegate void GetImageSize(TextureImporter importer, ref int width, ref int height);
        private static GetImageSize _getImageSizeDelegate;

        private struct Size {
            public int width;
            public int height;
        }
    }
}
