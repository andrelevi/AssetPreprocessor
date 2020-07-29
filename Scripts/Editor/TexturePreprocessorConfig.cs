using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_2019_1_OR_NEWER
using TextureCompressionQuality = UnityEditor.TextureCompressionQuality;
#endif

namespace AssetPreprocessor.Scripts.Editor
{
    [CreateAssetMenu(menuName="ScriptableObject/AssetPreprocessor/TexturePreprocessorConfig")]
    public class TexturePreprocessorConfig : BasePreprocessorConfig
    {
        [Header("Platforms")]
        public List<string> PlatformsRegexList = new List<string>
        {
            "Android",
            "iOS",
            "Standalone",
        };

        [Header("Skip Preprocessor Options")]
        [Tooltip("Allows skipping preprocessing if current texture format matches a certain regex. Useful to prevent lengthy preprocessing for textures that already have correct format set.")]
        public List<string> SkipIfCurrentTextureFormatMatchesRegexList = new List<string>();

        [Header("Texture Settings")]
        [Tooltip("Absolute max size allowed for textures. The texture's native size will be used if it is smaller than the max texture size.")]
        public int MaxTextureSize = 4096;
        public bool EnableReadWrite;
        public bool GenerateMipMaps = true;
        public bool EnableMipMapStreaming;
        public bool ForceLinear;
        [Tooltip("Decides how to round when a texture's size is NOT a power of two.")]
        public TextureImporterNPOTScale NPOTScale = TextureImporterNPOTScale.ToNearest;
        [Tooltip("By default each texture's max size will be based upon the texture's native size. Sometimes you might want to use a multiplier (such as 0.5) of that native size.")]
        public float NativeTextureSizeMultiplier = 1f;

        [Header("Filtering Settings")]
        public bool ForceFilterMode;
        #if ODIN_INSPECTOR
        [ShowIf(nameof(ForceFilterMode))]
        #endif
        public FilterMode FilterMode = FilterMode.Bilinear;
        #if ODIN_INSPECTOR
        [ShowIf(nameof(ForceFilterMode))]
        #endif
        public int AnisoLevel = 1;

        [Header("Compression Settings")]
        [Tooltip("Format used if the texture does NOT have an alpha channel.")]
        public TextureImporterFormat RGBFormat = TextureImporterFormat.Automatic;
        [Tooltip("Format used if the texture has an alpha channel.")]
        public TextureImporterFormat RGBAFormat = TextureImporterFormat.Automatic;
        public TextureCompressionQuality TextureCompressionQuality = TextureCompressionQuality.Normal;
    }
}

