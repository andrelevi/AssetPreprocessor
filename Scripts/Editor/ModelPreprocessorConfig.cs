using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    [CreateAssetMenu(menuName="ScriptableObject/AssetPreprocessor/ModelPreprocessorConfig")]
    public class ModelPreprocessorConfig : BasePreprocessorConfig
    {
        [Header("Import Settings")]
        public bool ImportMaterials;
        public bool ImportBlendShapes;
        public bool ForceGenerateLightmapUVs;
        public bool EnableReadWrite = true;
        public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;

        [Header("Animation Settings")]
        public ModelImporterAnimationCompression ModelImporterAnimationCompression = ModelImporterAnimationCompression.Optimal;
        
        [Header("Scene Settings")]
        public bool ImportLights;
        public bool ImportVisibility;
        public bool ImportCameras;
    }
}
