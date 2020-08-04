using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public class ModelPreprocessor : AssetPostprocessor
    {
        private void OnPreprocessModel()
        {
        }
        
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessModel.html
        /// </summary>
        /// <param name="gameObject">GameObject of the model.</param>
        private void OnPostprocessModel(GameObject model)
        {
            var modelImporter = (ModelImporter) assetImporter;
            var modelName = AssetPreprocessorUtils.GetAssetNameFromPath(modelImporter.assetPath);

            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<ModelPreprocessorConfig>();

            if (configs.Count == 0)
            {
                Debug.Log($"Could not find a {nameof(ModelPreprocessorConfig)} in project.");

                return;
            }
            
            configs = configs
                .Where(conf => conf.ShouldUseConfigForAssetImporter(assetImporter))
                .ToList();
            
            configs.Sort((config1, config2) => config1.ConfigSortOrder.CompareTo(config2.ConfigSortOrder));

            ModelPreprocessorConfig config = null;

            for (var i = 0; i < configs.Count; i++)
            {
                var configToTest = configs[i];

                // Found matching config.
                config = configToTest;

                Debug.Log($"Processing: {modelName}", model);
                Debug.Log($"Using: {config.name}", config);
                break;
            }

            // If could not find a matching config, don't process the asset.
            if (config == null) return;

            modelImporter.importLights = config.ImportLights;
            modelImporter.importVisibility = config.ImportVisibility;
            modelImporter.importCameras = config.ImportCameras;
            modelImporter.isReadable = config.EnableReadWrite;
            modelImporter.meshCompression = config.MeshCompression;
            modelImporter.resampleCurves = config.ResampleCurves;
            
            if (config.ForceGenerateLightmapUVs)
            {
                modelImporter.generateSecondaryUV = true;
            }
            
            if (modelImporter.importAnimation)
            {
                modelImporter.animationCompression = config.ModelImporterAnimationCompression;
            }

            if (modelImporter.importMaterials != config.ImportMaterials)
            {
                modelImporter.importMaterials = config.ImportMaterials;
            }

            if (modelImporter.importBlendShapes != config.ImportBlendShapes)
            {
                modelImporter.importBlendShapes = config.ImportBlendShapes;
            }

            // Enable the bones for ALL of the model's animation clips.
            for (var i = 0; i < modelImporter.clipAnimations.Length; i++)
            {
                var clip = modelImporter.clipAnimations[i];

                if (clip.maskType != ClipAnimationMaskType.CreateFromThisModel) continue;
                
                var serializedObject = new SerializedObject(modelImporter);
                
                var transformMask = serializedObject.FindProperty($"m_ClipAnimations.Array.data[{i}].transformMask");

                var arrayProperty = transformMask.FindPropertyRelative("Array");

                for (var j = 0; j < arrayProperty.arraySize; j++)
                {
                    var element = arrayProperty.GetArrayElementAtIndex(j);
                    
                    if (config.MaskBonesToEnable.Contains(element.FindPropertyRelative("m_Path").stringValue))
                    {
                        element.FindPropertyRelative("m_Weight").floatValue = 1;
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            Debug.Log($"Processed: {model.name}", model);
        }
        
        private void OnPreprocessAnimation()
        {
            var modelImporter = assetImporter as ModelImporter;
            
            if (modelImporter.defaultClipAnimations.Length == 0) return;
            
            var modelName = AssetPreprocessorUtils.GetAssetNameFromPath(modelImporter.assetPath);
            
            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<ModelPreprocessorConfig>();

            if (configs.Count == 0)
            {
                Debug.Log($"Could not find a {nameof(ModelPreprocessorConfig)} in project.");

                return;
            }
            
            configs = configs
                .Where(conf => conf.ShouldUseConfigForAssetImporter(assetImporter))
                .ToList();
            
            configs.Sort((config1, config2) => config1.ConfigSortOrder.CompareTo(config2.ConfigSortOrder));

            ModelPreprocessorConfig config = null;

            for (var i = 0; i < configs.Count; i++)
            {
                var configToTest = configs[i];

                // Found matching config.
                config = configToTest;

                Debug.Log($"Processing animations for: {modelName}");
                Debug.Log($"Using: {config.name}", config);
                break;
            }

            // If could not find a matching config, don't process the asset.
            if (config == null) return;
            
            var clips = modelImporter.defaultClipAnimations.ToArray();

            for (var i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                
                clip.keepOriginalPositionXZ = config.KeepOriginalPositionXZ;
                clip.keepOriginalOrientation = config.KeepOriginalOrientation;
                clip.keepOriginalPositionY = config.KeepOriginalPositionY;
                
                clip.maskType = config.ClipAnimationMaskType;
                
                clips[i] = clip;
            }

            modelImporter.clipAnimations = clips;
        }
    }
}
