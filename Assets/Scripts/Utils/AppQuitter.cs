using UnityEngine;

namespace Utils
{
    public class AppQuitter : MonoBehaviour
    {
        [SerializeField] private KeyCode quitKey = KeyCode.Q;

        private void Update()
        {
            if (Input.GetKeyDown(quitKey))
            {
                Quit();
            }
        }

        private void Quit()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // エディタ上
        #else
            Application.Quit(); // ビルド後
        #endif
        }
    }
}