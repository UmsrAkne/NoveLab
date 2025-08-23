using System.Collections.Generic;
using ScenarioModel;
using UnityEngine;

namespace Core
{
    public interface IImageSetFactory
    {
        void CreateAndAdd(IImageContainer stacker, ImageOrder order);

        List<Texture2D> GetTextures(ImageOrder order);
    }
}