Intro
=

Some starter scripts for getting familiar with: https://docs.unity3d.com/ScriptReference/AssetPostprocessor.html.

It is recommended to fork this branch, and make modifications as needed for your own project.

First Steps
=
First, create configs via the window menu:

Assets -> Create -> ScriptableObject -> AssetProcessor

![](/README/create-config-location.png?raw)

Usage
=
It is recommended to have **multiple** TextureProcessorConfigs.

You'll probably want *at least* one config for each texture category (model textures, particle textures, etc). 

Tweak the asset path regex string lists to ensure that each config only targets the desired assets.

<img src="/README/asset-path-regex-strings.png?raw" width="500">

Platform Regex Strings
=

Example regex strings:
* Android
* iOS
* Standalone (for OSX, Windows and Linuix standalone builds)

<img src="/README/platform-regex-strings.png?raw" width="500">
