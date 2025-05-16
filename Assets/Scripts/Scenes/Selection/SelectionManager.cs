using System;
using System.IO;
using System.Linq;
using Loaders;
using UI.Adapters;
using UI.Controllers;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Scenes.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject imagePrefab;

        [SerializeField]
        private GameObject spriteAdapterPrefab;

        [SerializeField]
        private Transform thumbnailContainer;

        [SerializeField]
        private ImageSelector imageSelector;

        [SerializeField]
        private GameObject backgroundImage;

        [SerializeField]
        private ImageStacker imageStacker;

        private int selectedIndex;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            #if UNITY_EDITOR
            LoadDebugImages().Forget();
            #endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                imageSelector.MoveSelection(+1);
                SetBackground();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                imageSelector.MoveSelection(-1);
                SetBackground();
            }
        }

        private void AddImage(Texture2D texture)
        {
            var imageGameObject = Instantiate(imagePrefab, thumbnailContainer.transform);
            var adapter = imageGameObject.GetComponent<SpriteAdapter>();
            adapter.SetTexture(texture);
            adapter.SetScale(0.15f);
            adapter.SetAlpha(0.5f);
            imageSelector.DisplayImages.Add(adapter);
        }

        private async UniTask LoadDebugImages()
        {
            if (EditorApplication.isPlaying && !Application.isEditor)
            {
                // Editor で Play 中のみ、ビルド時はスルー
                return;
            }

            Debug.Log("This runs ONLY in the editor during Play mode!");

            var imagePaths = GetThumbnailPaths(true);
            foreach (var imagePath in imagePaths)
            {
                var texture = await ImageLoader.LoadTexture(imagePath, false);
                AddImage(texture);
            }

            imageSelector.DisplayImages.First()?.SetAlpha(1);
        }

        private void SetBackground()
        {
            var d = imageSelector.SelectedItem;
            var adapter = d as SpriteAdapter;
            if (adapter == null)
            {
                return;
            }

            var newAdapter = Instantiate(spriteAdapterPrefab);
            var na = newAdapter.GetComponent<SpriteAdapter>();
            na.SetTexture(adapter.GetTexture());
            imageStacker.AddImage(na);
        }

        private string[] GetThumbnailPaths(bool debugMode)
        {
            return debugMode ? Directory.GetFiles(@"C:\Users\Public\testData\images") : Array.Empty<string>();
        }
    }
}