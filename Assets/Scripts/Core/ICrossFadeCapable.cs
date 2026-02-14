using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface ICrossFadeCapable
    {
        UniTask CrossFadeExpression(Texture2D newTexture, float duration = 0.5f);
    }
}