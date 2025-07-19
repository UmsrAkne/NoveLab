using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loaders;
using UI.Adapters;
using UI.Controllers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Scenes.Loading;
using UI.Animations;
using UnityEngine.SceneManagement;

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

        [SerializeField]
        private SceneFader sceneFader;

        private int selectedIndex;
        private readonly List<string> imagePaths = new ();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            LoadSampleImages().Forget();
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
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleEnterPressed().Forget();
            }
        }

        private void AddImage(Texture2D texture, int width)
        {
            var imageGameObject = Instantiate(imagePrefab, thumbnailContainer.transform);
            var adapter = imageGameObject.GetComponent<SpriteAdapter>();
            adapter.SetTexture(texture);

            var scale = (float)width / texture.width;
            adapter.SetScale(scale);
            adapter.SetAlpha(0.2f);

            imageSelector.DisplayImages.Add(adapter);
        }

        private async UniTask LoadSampleImages()
        {
            imagePaths.Clear();
            imagePaths.AddRange(GetThumbnailPaths());
            foreach (var imagePath in imagePaths)
            {
                var texture = await ImageLoader.LoadTexture(imagePath, false);
                AddImage(texture, 160);
            }

            imageSelector.DisplayImages.First()?.SetAlpha(1);
        }

        private async UniTaskVoid HandleEnterPressed()
        {
            var index = imageSelector.SelectedIndex;
            if (index >= 0)
            {
                var path = Directory.GetParent(Directory.GetParent(imagePaths[index])!.FullName);
                LoadingManager.GlobalScenarioContext.ScenarioDirectoryPath = path?.FullName;

                await sceneFader.FadeOut(2f);
                SceneManager.LoadScene("LoadingScene");
            }
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

        private string[] GetThumbnailPaths()
        {
            var exeDir = Path.GetDirectoryName(Application.dataPath) ?? string.Empty;
            var scenarioDirectory = Path.Combine(exeDir, "scenes");

            var imagePaths = new List<string>();
            var scenarioDirectories = Directory.GetDirectories(scenarioDirectory);

            foreach (var directory in scenarioDirectories)
            {
                var imageDir = Path.Combine(directory, "images");
                imagePaths.Add(Directory.GetFiles(imageDir).FirstOrDefault());
            }

            return imagePaths.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
        }
    }
}