using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Beyond
{
    public enum SceneIndices
    {
        MANAGER = 0, // The scene with our game manager
        MAIN = 1, // Main title / start screen
        GAME = 2, // Main game scene
        WORLD = 3 // World map scene
    }

    public class BeyondSceneManager : MonoBehaviour
    {
        public static BeyondSceneManager instance;
        public GameObject loadingScreen;
        public GameObject progressBar;
        public TextMeshProUGUI loadingText;
        public AudioListener mainSceneAudioListener;
        
        #region Tips 
        [Header("Tips")]

        public TextMeshProUGUI tipsText;
        public CanvasGroup alphaCanvas;
        public string[] tips;
        #endregion

        public void Awake()
        {
            instance = this;
            SceneManager.LoadSceneAsync((int)SceneIndices.MAIN, LoadSceneMode.Additive);
        }

        public void Start()
        {
            AudioManager.instance.PlayAudio(
                this.gameObject, AudibleObjects.SCREEN, AudibleEvents.START
            );
        }

        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        public void StartGame()
        {
            loadingScreen.gameObject.SetActive(true);

            StartCoroutine(GenerateTips());

            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndices.MAIN));
            scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndices.GAME, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress());
            StartCoroutine(GetTotalProgress());
        }

        float totalSceneProgress;
        float totalMapGenerationProgress;

        public IEnumerator GetSceneLoadProgress()
        {
            for (int i=0; i<scenesLoading.Count; i++)
            {
                while (!scenesLoading[i].isDone)
                {
                    totalSceneProgress = 0 ;

                    foreach (AsyncOperation operation in scenesLoading)
                    {
                        totalSceneProgress += operation.progress ;
                    }

                    totalSceneProgress = (totalSceneProgress / (float)scenesLoading.Count);
                    progressBar.GetComponent<Slider>().value = totalSceneProgress;
                    //Debug.Log("totalSceneProgress="+totalSceneProgress);
                    loadingText.text = string.Format("Loading Game Scenes: {0}%", (int)(totalSceneProgress*100));

                    yield return null;
                }
            }

        }
        public IEnumerator GetTotalProgress()
        {
            float totalProgress = 0 ;

            while (GameManager.instance == null || GameManager.instance.MapIsGenerating())
            {
                if (GameManager.instance == null)
                {
                    totalMapGenerationProgress = 0;
                }
                else
                {
                    totalMapGenerationProgress = GameManager.instance.GetLoadProgress();
                }
                totalProgress = (totalSceneProgress + totalMapGenerationProgress) / 2f;
                progressBar.GetComponent<Slider>().value = totalProgress;
                loadingText.text = string.Format("Loading Terrain: {0}%", (int)(totalMapGenerationProgress*100));

                yield return null;
            }
            mainSceneAudioListener.enabled = false;
            loadingScreen.gameObject.SetActive(false);
        }

        public int tipCount;
        public IEnumerator GenerateTips()
        {
            tipCount = Random.Range(0, tips.Length);
            tipsText.text = tips[tipCount];
            while (loadingScreen.activeInHierarchy)
            {
                float timeToRead = 0.5f + (float)tipsText.text.Length / 15f; // 15 characters per second
                yield return new WaitForSeconds(timeToRead);
                LeanTween.alphaCanvas(alphaCanvas, 0, 0.5f); // fade off
                yield return new WaitForSeconds(0.5f);

                if (++tipCount >= tips.Length)
                    tipCount = 0;

                tipsText.text = tips[tipCount];
                LeanTween.alphaCanvas(alphaCanvas, 1, 0.5f); // fade in

            }

        }

        public void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}
