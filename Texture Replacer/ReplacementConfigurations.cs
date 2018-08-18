using UnityEngine;

namespace Texture_And_Material_Replacer
{
    abstract class ReplacementConfiguration
    {
        public string PartID { get; set; }
    }

    class TextureConfiguration : ReplacementConfiguration
    {
        public WWW TexturePath { get; set; }

        public TextureConfiguration(WWW texturePath)
        {
            PartID = null;
            TexturePath = texturePath;
        }

        public TextureConfiguration(string partID, WWW texturePath)
        {
            PartID = partID;
            TexturePath = texturePath;
        }
    }

    class MaterialConfiguration : ReplacementConfiguration
    {
        public Color32 Color { get; set; }
        public Material Material { get; set; }

        public MaterialConfiguration(Color32 color)
        {
            PartID = null;
            Color = color;
        }

        public MaterialConfiguration(string partID, Color32 color)
        {
            PartID = partID;
            Color = color;
        }

        public MaterialConfiguration(Material material)
        {
            PartID = null;
            Material = material;
        }

        public MaterialConfiguration(string partID, Material material)
        {
            PartID = partID;
            Material = material;
        }
    }

}
