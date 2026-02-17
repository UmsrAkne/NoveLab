using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;

namespace Core
{
    public interface ICrossFadeCapable
    {
        UniTask CrossFadeExpression(ImageOrder imageOrder, float duration = 0.5f);
    }
}