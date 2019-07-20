using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TextureCompressionQuality = UnityEditor.TextureCompressionQuality;

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
            "Default"
        };

        [Header("Skip Preprocessor Options")]
        [Tooltip("Allows skipping preprocessing if current texture format matches a certain regex. Useful to prevent lengthy preprocessing for textures that already have correct format set.")]
        public List<string> SkipIfCurrentTextureFormatMatchesRegexList = new List<string>();

        [Header("Texture Settings")]
        [Tooltip("Absolute max size allowed for textures. The texture's native size will be used if it is smaller than the max texture size.")]
        public int MaxTextureSize = 4096;
        public bool EnableReadWrite;
        public bool ForceLinear;
        [Tooltip("Decides how to round when a texture's size is NOT a power of two.")]
        public TextureImporterNPOTScale NPOTScale = TextureImporterNPOTScale.ToNearest;
        [Tooltip("By default each texture's max size will be based upon the texture's native size. Sometimes you might want to use a multiplier (such as 0.5) of that native size.")]
        public float NativeTextureSizeMultiplier = 1f;

        [Header("Compression Settings")]
        [Tooltip("Format used if the texture does NOT have an alpha channel.")]
        public TextureImporterFormat RGBFormat = TextureImporterFormat.Automatic;
        [Tooltip("Format used if the texture has an alpha channel.")]
        public TextureImporterFormat RGBAFormat = TextureImporterFormat.Automatic;
        public TextureCompressionQuality TextureCompressionQuality = TextureCompressionQuality.Normal;
    }
}

