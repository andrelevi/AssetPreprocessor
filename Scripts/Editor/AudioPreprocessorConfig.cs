using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    [CreateAssetMenu(menuName="ScriptableObject/AssetPreprocessor/AudioPreprocessorConfig")]
    public class AudioPreprocessorConfig : BasePreprocessorConfig
    {
        [Header("Platforms")]
        public List<string> PlatformsRegexList = new List<string>
        {
            "Android",
            "iOS",
            "Standalone",
            "Default"
        };
        
        [Header("Match Criteria")]
        public float MaxClipLengthInSeconds = 999f;
        
        [Header("Load Settings")]
        public bool LoadInBackground;
        public AudioClipLoadType AudioClipLoadType = AudioClipLoadType.DecompressOnLoad;
        public bool PreloadAudioData = true;
        
        [Header("Quality Settings")]
        public bool ForceToMono;
        public bool Ambisonic;
        public AudioCompressionFormat AudioCompressionFormat = AudioCompressionFormat.Vorbis;
        [Range(0, 1)] public float Quality = 1f;

        [Header("Sample Settings")]
        public AudioSampleRateSetting AudioSampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
    }
}
