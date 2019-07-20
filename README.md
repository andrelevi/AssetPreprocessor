# Asset Preprocessor
Config-based workflow for [Unity's AssetPostprocessor class](https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html).

### Config Example
<img src="/README/texture-preprocessor-config-inspector-example.jpg?raw" width="500">

### Supported Asset Types
* **Textures**
  - [TexturePreprocessorConfig](/Scripts/Editor/TexturePreprocessorConfig.cs)
* **Audio Clips**
  - [AudioPreprocessorConfig](/Scripts/Editor/AudioPreprocessorConfig.cs)
* **Models**
  - [ModelPreprocessorConfig](/Scripts/Editor/ModelPreprocessorConfig.cs)

### Supported Unity Versions
* Unity 2019.1.x

# Usage
First, create configs via the `Assets` menu:

`Assets -> Create -> ScriptableObject -> AssetPreprocessor`

![](/README/create-config-location.png?raw)

It is recommended to have **multiple** `TextureProcessorConfigs`. You'll probably want *at least* one config for each texture category (model textures, particle textures, etc). 

## Asset Path Regex Strings
You can **limit** the configs to certain assets via the asset path regex strings.

Tweak the asset path regex string lists to ensure that each config only targets the desired assets.

You can use the `*` character as a wildcard.

<img src="/README/asset-path-regex-strings.png?raw" width="500">

## Platform Regex Strings
You can **limit** the configs to certain platforms via the platform regex strings.

The platform regex strings should ideally be perfect matches to actual the platform strings.

Example platform regex strings:
* `Android`
* `iOS`
* `Standalone` (to match OSX, Windows and Linuix standalone builds)
* `StandaloneOSX` (a perfect match platform string for OSX)

To get the string for your current platform, try logging `EditorUserBuildSettings.activeBuildTarget.ToString()`.

<img src="/README/platform-regex-strings.png?raw" width="500">

## Troubleshooting
**Why is nothing happening when importing an asset?**
* Make sure that you actually have a preproccessor config for your asset type created inside of your `Assets/` directory.

**I have a config for my asset type. Why isn't it used when importing an asset of the same type?**
* Check your asset path match and ignore regex strings. Ensure that the asset you are importing matches one of the match regex strings, and that it does *not* match one of the ignore regex strings. And then check that your current platform matches one of the platform regex strings.

## Recommended AudioPreprocessorConfig Settings
Check out the "Suggested Settings" tables in this [article about audio import settings](https://www.gamasutra.com/blogs/ZanderHulme/20190107/333794/Unity_Audio_Import_Optimisation__getting_more_BAM_for_your_RAM.php).  
