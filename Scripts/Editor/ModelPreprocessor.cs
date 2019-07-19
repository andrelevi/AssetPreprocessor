using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetPreprocessor.Scripts.Editor
{
    public class ModelPreprocessor : AssetPostprocessor
    {
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPreprocessModel.html
        /// </summary>
        private void OnPreprocessModel()
        {
            var modelImporter = (ModelImporter) assetImporter;

            var assetPath = modelImporter.assetPath;
            var modelName = AssetPreprocessorUtils.GetAssetNameFromPath(modelImporter.assetPath);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            var configs = AssetPreprocessorUtils.GetScriptableObjectsOfType<ModelPreprocessorConfig>();

            if (configs.Count == 0)
            {
                Debug.Log($"No existing {nameof(ModelPreprocessorConfig)} found.");

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
            
            Debug.Log($"Processed: {model.name}", model);
        }

        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnPostprocessModel.html
        /// </summary>
        /// <param name="gameObject">GameObject of the model.</param>
        private void OnPostprocessModel(GameObject gameObject)
        {
            // Make any modifications to the model's gameObject here, if needed.
        }
    }
}
