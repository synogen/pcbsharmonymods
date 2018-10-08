using UnityEngine;

namespace Asset_Replacer
{
    abstract class ReplacementConfiguration
    {
        public string PartID { get; set; }
    }

    class TextureConfiguration : ReplacementConfiguration
    {
        public Texture2D Texture { get; set; }

        public WWW www { get; set; }

        public bool replaced = false;

        public TextureConfiguration(WWW texturePath)
        {
            PartID = null;
            www = texturePath;
            Texture = new Texture2D(texturePath.texture.width, texturePath.texture.height);
            texturePath.LoadImageIntoTexture(Texture);
        }

        public TextureConfiguration(string partID, WWW texturePath)
        {
            PartID = partID;
            www = texturePath;
            Texture = new Texture2D(texturePath.texture.width, texturePath.texture.height);
            texturePath.LoadImageIntoTexture(Texture);
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

    class SoundConfiguration : ReplacementConfiguration
    {
        public AudioClip Clip { get; set; }

        public SoundConfiguration(AudioClip newClip)
        {
            this.Clip = newClip;
        }
    }

}
