# TextureSeeker

**TextureSeeker** is an app that extracts all textures associated with a `*.mapbnd.dcx` file.

## Requirements

Edit `userConfig.json` and update the following paths to match where you have installed these tools:

* `BinderToolPath`: [BinderTool by Atvaark](https://github.com/Atvaark/BinderTool)
* `SoulsModelToolPath`: [Aqua Toolset by Shadowth117](https://github.com/Shadowth117/Aqua-Toolset)

## How to Use

In `userConfig.json`, configure the following:

* `ProjectFolder`: The directory where you want to save the extracted files.
* `GetFbxFile`: Set to `true` if you want to generate a `.fbx` model file.
* `UnpackTextures`: Set to `true` to convert `.tpfbdt` files into `.tpf` (if not already converted).
* `MapTexturesDir`: Path to the folder containing map textures.

  * Example: `..\DARK SOULS III\Game\map\m32` (for "Archdragon Peak")
  * You can also set it to `..\DARK SOULS III\Game\map` for all maps, but this will take longer.

## Output Layout

```
ProjectFolder
├── *.fbx
└── materials
    ├── material_name
    │   ├── texture.dds
    │   ├── texture.dds
    │   └── texture.dds
    ├── another_material
    │   ├── ...
    │   └── ...
    └── ...
