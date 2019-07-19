using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using TextureCompressionQuality = UnityEditor.TextureCompressionQuality;

namespace AssetPreprocessor.Scripts.Editor
{
    [CreateAssetMenu(menuName="ScriptableObject/AssetPreprocessor/TexturePreprocessorConfig")]
    public class TexturePreprocessorConfig : BasePreprocessorConfig
    {
        [Header("Platforms")]
        [FormerlySerializedAs("PlatformsAffected")] public List<string> PlatformsRegexList = new List<string>
        {
            "Android",
            "iOS",
            "Standalone",
            "Default"
        };

        [Header("Skip Preprocessor Options")]
        public List<string> SkipIfCurrentTextureFormatContains = new List<string>();

        [Header("Texture Settings")]
        [FormerlySerializedAs("MaxResTextureSize")] public int MaxTextureSize = 4096;
        public bool EnableReadWrite;
        public bool ForceLinear;
        public TextureImporterNPOTScale NPOTScale = TextureImporterNPOTScale.ToNearest;
        public float NativeResMultiplier = 1f;

        [Header("Compression Settings")]
        public TextureImporterFormat RGBFormat = TextureImporterFormat.Automatic;
        public TextureImporterFormat RGBAFormat = TextureImporterFormat.Automatic;
        public TextureCompressionQuality TextureCompressionQuality = TextureCompressionQuality.Normal;
    }
}

