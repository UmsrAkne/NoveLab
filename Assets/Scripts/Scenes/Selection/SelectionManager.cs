using System.IO;
using Loaders;
using UI.Adapters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject imagePrefab;

        [SerializeField]
        private Transform thumbnailContainer;

        [SerializeField]
        private Sprite newSprite;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            #if UNITY_EDITOR
            LoadDebugImages();
            #endif
        }

        // Update is called once per frame
        // private void Update()
        // {
        // }

        private void AddImage(Texture2D texture)
        {
            var imageGameObject = Instantiate(imagePrefab, thumbnailContainer.transform);
            var adapter = imageGameObject.GetComponent<SpriteAdapter>();
            adapter.SetTexture(texture);
            adapter.SetScale(0.15f);
        }

        private void LoadDebugImages()
        {
            if (EditorApplication.isPlaying && !Application.isEditor)
            {
                // Editor で Play 中のみ、ビルド時はスルー
                return;
            }

            Debug.Log("This runs ONLY in the editor during Play mode!");

            var imagePaths = Directory.GetFiles(@"C:\Users\Public\testData\images");
            foreach (var imagePath in imagePaths)
            {
                var texture = ImageLoader.LoadTexture(imagePath, false);
                AddImage(texture);
            }
        }
    }
}