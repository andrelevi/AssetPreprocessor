# Asset Preprocessor
Config-based workflow for [Unity's AssetPostprocessor class](https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html).

### Supported Unity Versions
* Unity 2019.1.x

# Usage
First, create configs via the window menu:

`Assets -> Create -> ScriptableObject -> AssetPreprocessor`

![](/README/create-config-location.png?raw)

It is recommended to have **multiple** TextureProcessorConfigs.

You'll probably want *at least* one config for each texture category (model textures, particle textures, etc). 

Tweak the asset path regex string lists to ensure that each config only targets the desired assets.

<img src="/README/asset-path-regex-strings.png?raw" width="500">

## Platform Regex Strings
The platform regex strings should be perfect matches to the platform strings as much as possible.

Example platform regex strings:
* `Android`
* `iOS`
* `Standalone` (for OSX, Windows and Linuix standalone builds)
* `StandaloneOSX` (a more accurate platform string for OSX)

If unsure of the string for your current platform, try logging `EditorUserBuildSettings.activeBuildTarget.ToString()`.

<img src="/README/platform-regex-strings.png?raw" width="500">

## Troubleshooting
**Why is nothing happening when importing an asset?**
* Make sure that you actually have a preproccessor config for your asset type created inside of your `Assets/` directory.

**I have a config for my asset type. Why isn't it getting used when importing an asset of the same type?**
* Check your match and ignore regex strings. Ensure that the asset you are importing matches one of the match regex strings, and that it does *not* match one of the ignore regex strings. And then check that your platform matches one of the platform regex strings.


## Recommended AudioPreprocessorConfig Settings
Check out the "Suggested Settings" tables in this [article about audio import settings](https://www.gamasutra.com/blogs/ZanderHulme/20190107/333794/Unity_Audio_Import_Optimisation__getting_more_BAM_for_your_RAM.php).  
