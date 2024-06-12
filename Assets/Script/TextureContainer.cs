using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Container;

[CreateAssetMenu(menuName = "Create Texure Container")]
public class TextureContainer : ScriptableObject
{
    [SerializeField] private Texture2D[] numberTexture;
    [SerializeField] private Texture2D[] symbolTexture;
    [SerializeField] private Texture2D[] ReturnLetters;
    private Texture2D[] invertedNumberTexture;
    private Texture2D[] invertedSymbolTexture;

    public Texture2D[] ReturnLetterTexture { get { return ReturnLetters; } }

    public Texture2D[] GetPasswordTexture(PasswordVariant variant)
    {
        switch (variant)
        {
            case PasswordVariant.Number:
                return numberTexture;
            case PasswordVariant.Symbol: 
                return symbolTexture;
            default:
                return numberTexture;
        }
    }

    public Texture2D[] GetInvertPasswordTexture(PasswordVariant variant)
    {
        switch (variant)
        {
            case PasswordVariant.Number:
                return invertedNumberTexture;
            case PasswordVariant.Symbol:
                return invertedSymbolTexture;
            default:
                return invertedNumberTexture;
        }
    }

    public void Invert()
    {
        invertedNumberTexture = InvertingTexture(numberTexture);
        invertedSymbolTexture = InvertingTexture(symbolTexture);
    }

    private Texture2D[] InvertingTexture(Texture2D[] baseTextures)
    {
        List<Texture2D> invertTextures = new List<Texture2D>();
        foreach (var texture in baseTextures)
        {
            Texture2D invertTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            var invertPixels = invertTexture.GetPixels32();
            var basePixels = texture.GetPixels32();
            for (int i = 0; i < invertPixels.Length; i++)
            {
                invertPixels[i] = new Color32((byte)(255 - basePixels[i].r), (byte)(255 - basePixels[i].g), (byte)(255 - basePixels[i].b), basePixels[i].a);
            }
            invertTexture.SetPixels32(invertPixels);
            invertTexture.Compress(false);
            invertTextures.Add(invertTexture);
        }
        return invertTextures.ToArray();
    }

}

namespace Container
{
    public enum PasswordVariant
    {
        Number,
        Symbol
    }
}
