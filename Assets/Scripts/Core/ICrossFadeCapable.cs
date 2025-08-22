using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface ICrossFadeCapable
    {
        UniTask CrossFadeExpression(Texture2D newEye, Texture2D newMouth, float duration = 0.5f);
    }
}