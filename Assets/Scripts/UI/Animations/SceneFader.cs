using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class SceneFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeGroup;

        public async UniTask FadeOut(float duration = 1f)
        {
            var t = 0f;
            while (t < duration)
            {
                fadeGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
                t += Time.deltaTime;
                await UniTask.Yield();
            }

            fadeGroup.alpha = 1f;
        }
    }
}