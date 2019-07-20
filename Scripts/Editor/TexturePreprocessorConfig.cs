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
        public int MaxTextureSize = 4096;
        public bool EnableReadWrite;
        public bool ForceLinear;
        public TextureImporterNPOTScale NPOTScale = TextureImporterNPOTScale.ToNearest;
        [Tooltip("By default each texture's max size will be based upon the texture's native size. Sometimes you might want to use a multiplier (such as 0.5) of that native size.")]
        public float NativeTextureSizeMultiplier = 1f;

        [Header("Compression Settings")]
        public TextureImporterFormat RGBFormat = TextureImporterFormat.Automatic;
        public TextureImporterFormat RGBAFormat = TextureImporterFormat.Automatic;
        public TextureCompressionQuality TextureCompressionQuality = TextureCompressionQuality.Normal;
    }
}

