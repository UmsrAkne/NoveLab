using ScenarioModel;
using UnityEngine;

namespace UI.Images
{
    public interface ITextureProvider
    {
        Texture2D GetTexture(ImageOrder imageOrder);
    }
}