using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boot
{
    public class BootManager : MonoBehaviour
    {
        private readonly string nextSceneName = "SelectionScene"; // 遷移先のシーン名

        private void Awake()
        {
            Debug.Log("BootManager Awake called");

            // // マネージャ系の初期化（必要なら）
            // InitManagers();
            //
            // // このオブジェクトを破壊されないようにする
            // DontDestroyOnLoad(gameObject);

            LoadNextScene();
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}