## Texture And Material Replacer for PC Building Simulator
This is a mod that allows you to overwrite most of the textures and some of the materials in the game with your own. Currently the textures and materials for all parts, 
all icons/backgrounds for OmegaOS and stuff like posters and floor textures are supported.
If you find a texture or material that can't be replaced with this feel free to let me know.

An example pack is included. It replaces a few of the desktop icons and the default background with Windows 98 versions and it changes the Ryzen 7 1700 texture 
to a 486DX. Also the flooring is replaced with a brighter wood and the walls and ceiling have gotten a different color to show how material replacing works.

If you have downloaded a replacement pack just copy the folder with the .conf files ("Image Replacer.conf", "Material Colors.conf", "Parts Texture Replacer.conf")
into the mod folder and it should get loaded automatically when the game starts.
If you want to disable a pack just move the pack folder to a backup folder somewhere. The mod only loads subfolders in the mod folder which contain the 
configuration files needed for a texture/material replacer pack.

See the Quick Start section below on how to make your own texture/material replacements with this!

## Requirements
Requires FusioN.'s modloader for PCBS to be installed ( https://github.com/fusiion/PCBSModloader/releases )

## Installation
Extract the mod folder in the archive to <PCBS folder>\Mods

## Compatibility
Should work with all other mods as long as the PCBS modloader is installed. You should use a clean Assembly-CSharp-firstpass.dll with only the modloader installed.
Code changes in future versions of PC Building Simulator may break the mod.

## Credits
FusioN. for his PCBS modloader ( https://www.youtube.com/channel/UCb8NrhEi7gWJzVt1eVC7IRQ )
The Harmony C# patching library ( https://github.com/pardeike/Harmony )

## Quick Start Texture/Sprite/Material Patching
Texture and Material Replacer uses three configuration files for replacing textures and materials:
  Image Replacer.conf -> For most of the static images in game like posters, Omega OS icons and wallpapers and such
  Material Colors.conf -> To change the color of materials in game, for example the walls and ceiling
  Parts Texture Replacer.conf -> For replacing textures on parts that are spawned in the game like CPUs, graphics cards and such

Either create a new folder in the mod directory with those three files or use/copy the included "ExamplePack" or "TemplatePack" (empty pack) folder.
Once you have done that you can start looking for the first texture you want to replace as explained in the following:

1. Open the PCBS_Data folder with AssetStudio ( https://github.com/Perfare/AssetStudio )
2. Search for the Texture you want to change
	2a. Select "Show Type" -> "Texture2D"
	2b. Go to the "Asset List" tab and search for your texture, examples to search for are "Ryzen 5" or "icon_"
	2c. Note down the texture name, be careful as this is case sensitive!
3. Copy the new image you want to use into your pack folder, PNG or JPG works, for best results make it the same size as the original texture (AssetStudio shows
	width and height in the preview window)
4. Now depending on if the texture is for a part or for an icon/poster or other graphics in the game:
	4a. Part texture: Open or create "Parts Texture Replacer.conf" with a text editor and add a line <texture name>|<your new texture file> so as an example it will
		look like "Ryzen 5 Quad Core 1400|newCpu.png"
	4b. Icon/poster/image: Open "Image Replacer.conf" with a text editor and add a line <texture name>|<your new texture file> so as an example it will look like 
		"icon_email|myMail.jpg"
5. Start the game and try it out!
6. The mod creates a "replacer.log" in the mod folder where you can check if things have been loaded correctly in case there are errors or things don't get replaced

If you are in-game and you changed something in the replacement configuration you can reload the replacement configurations by pressing "R", you still have to reload 
your save game for the changes to become visible though!

## Material Replacements
For materials the general process is the same as for textures. To find materials I would recommend using UABE ( https://github.com/DerPopo/UABE ) instead of 
AssetStudio though, since AssetStudio doesn't show materials by default, it has no preview for them and it doesn't seem to list them by name like UABE does.
There are two possible ways to change materials right now:
1. Use the "Material Colors.conf" file to change the main color of a material. Just like in the other conf files you write the material name first, then seperated 
by a | come four comma seperated values representing the RGBA color (Red, Green, Blue, Alpha) with values ranging from 0 to 255 for each of them, here's an example:
Wall_Mat|255,0,0,255
This would color the wall material red with 100% opacity.

2. You can even create an asset bundle in Unity with materials you define yourself. This is unsupported and for advanced users only.
If you decide to try this give the material in Unity the same name as the material in PCBS that you want to overwrite. Save the asset bundle as "materials.assetbundle"
in the mod folder.
I do not know if the manifest file is required but I always copied it as well during my tests.
I have not gotten custom shaders to work using this method and it seems rather buggy right now so you're on your own with that.

## Part Specific Texture/Material Replacements
For parts this mod now allows patching textures and materials for a specific part ID. To do that you can simply specify the part ID in the replacement config like this:
Ryzen 7 Eight Core 1700|CPU_JOKE_1|cpu/cyrix.png
The mod will then replace that texture only if it is being loaded for a part with that specific ID.
Let's say you added a new CPU based on the Ryzen 7 1700 and gave it an ID CPU_AMD_351 you could just write "Ryzen 7 Eight Core 1700|CPU_AMD_351|mytexture.png" and it 
will replace the Ryzen 7 Texture ONLY for your new CPU.
Material replacements for parts work the same way, just specify your part ID after the material name. An example would be:
SSD_Case|SSD_TIC_1|0,0,255,255
This will color the Shean Mega 60GB SSD blue, while leaving all other Shean SSDs unaffected.

Part Specific Replacements are not supported for asset bundles right now.

Examples for part ID specific material color replacements are included in the "Material Colors.conf".
There are even examples for part ID specific texture replacements in the "Parts Texture Replacer.conf" but to see those you have to install the included unitypatcher
mod that adds the two test CPUs (PartCPU.zip).