using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AssetPreprocessor.Scripts.Editor
{
    [CreateAssetMenu(menuName="ScriptableObject/AssetPreprocessor/AudioPreprocessorConfig")]
    public class AudioPreprocessorConfig : BasePreprocessorConfig
    {
        [Header("Platforms")]
        [FormerlySerializedAs("PlatformsAffected")] public List<string> PlatformsRegexList = new List<string>
        {
            "Android",
            "iOS",
            "Standalone",
            "Default"
        };
        
        [Header("Match Criteria")]
        public float MaxClipLengthInSeconds = 5f;
        
        [Header("Load Settings")]
        public bool LoadInBackground = true;
        public bool PreloadAudioData = true;
        
        [Header("Quality Settings")]
        public bool ForceToMono = true;
        public float Quality = 0.6f;
        public AudioClipLoadType AudioClipLoadType = AudioClipLoadType.CompressedInMemory;
        public AudioCompressionFormat AudioCompressionFormat = AudioCompressionFormat.Vorbis;

        [Header("Sample Settings")]
        public AudioSampleRateSetting AudioSampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
    }
}
