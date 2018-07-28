!! UNFINISHED, MAY OR MAY NOT WORK !!
## Texture Replacer for PC Building Simulator
This is a mod that allows you to overwrite some of the textures in the game with your own. Currently the textures for all parts and all icons/backgrounds for OmegaOS are supported.
If you're lucky you can replace other textures with it as well, but it's untested. Posters do not work as I tested that already.

As an example it overwrites a few of the desktop icons with Windows 98 versions and it changes the Ryzen 7 1700 texture to a 486DX. 
See the Quick Start section below on how to make your own texture replacements with this!

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

## Quick Start Texture/Sprite Patching
1. Open the PCBS_Data folder with AssetStudio ( https://github.com/Perfare/AssetStudio )
2. Search for the Texture you want to change
	2a. Select "Show Type" -> "Texture2D"
	2b. Go to the "Asset List" tab and search for your texture, examples to search for are "Ryzen 5" or "icon_"
	2c. Note down the texture name, be careful as this is case sensitive!
3. Copy the new image you want to use into this folder, PNG or JPG works, for best results make it the same size as the original texture (AssetStudio shows width and height in the preview window)
4. Now depending on if the texture is for a part or for an icon/poster or other graphics in the game:
	4a. Part texture: Open "Parts Texture Replacer.conf" with a text editor and add a line <texture name>|<your new texture file> so as an example it will look like "Ryzen 5 Quad Core 1400|newCpu.png"
	4b. Icon/poster/image: Open "Image Replacer.conf" with a text editor and add a line <texture name>|<your new texture file> so as an example it will look like "icon_email|myMail.jpg"
5. Start the game and try it out!
