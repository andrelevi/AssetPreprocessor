using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        public bool SortHierarchyByName = true;
        public bool ForceGenerateLightmapUVs;
        public bool EnableReadWrite = true;
        public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;

        [Header("Animation Settings")]
        public bool ResampleCurves = true;
        public ModelImporterAnimationCompression ModelImporterAnimationCompression = ModelImporterAnimationCompression.Optimal;
        
        [Header("Animation Processing")]
        public bool EnableAnimationPreprocessing;
        public bool KeepOriginalOrientation;
        public bool KeepOriginalPositionXZ;
        public bool KeepOriginalPositionY = true;
        public ClipAnimationMaskType ClipAnimationMaskType = ClipAnimationMaskType.None;
        #if ODIN_INSPECTOR
        [ShowIf(nameof(ClipAnimationMaskType), ClipAnimationMaskType.CreateFromThisModel)]
        #endif
        public List<string> MaskBonesToEnable;
        
        [Header("Scene Settings")]
        public bool ImportLights;
        public bool ImportVisibility;
        public bool ImportCameras;
    }
}
