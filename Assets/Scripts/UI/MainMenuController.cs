using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Beyond
{
    public class MainMenuController : MonoBehaviour
    {
        public static MainMenuController instance;
        GameManager gameManager;

        #region Menu Panels
        [Header("Menus")]
        [SerializeField] private GameObject MainMenuPanel;
        [SerializeField] private GameObject OptionsPanel;
        [SerializeField] private GameObject OptionsSoundsPanel;
        #endregion

        #region Sliders
        [Header("Sliders")]
        [SerializeField] private Slider MusicVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;
        #endregion

        private string CurrentState;
        public List<GameObject> AllPanels;
        
        public void Awake()
        {
            instance = this;
            gameManager = GameManager.instance;
        }
        public void Start()
        {
            AllPanels = new List<GameObject>();
            AllPanels.Add(MainMenuPanel);
            AllPanels.Add(OptionsPanel);
            AllPanels.Add(OptionsSoundsPanel);

            ActivatePanel(MainMenuPanel);

        }

        public void InitialiseVolumeSliders(float musicVolume , float SFXVolume)
        {
            MusicVolumeSlider.value = musicVolume ;
            SFXVolumeSlider.value = SFXVolume ;
        }

        public void ActivatePanel(GameObject panel)
        {
            panel.SetActive(true);
            foreach (GameObject go in AllPanels.Where(p => !p.Equals(panel)))
            {
                go.SetActive(false);
            }
        }

        public void StartGame()
        {
            gameManager.StartGame();
        }

        public void QuitGame()
        {
            gameManager.Quit();
        }

        public void SetMusicVolume(float SliderValue)
        {
            AudioManager.instance.SetVolume("Music" , SliderValue);
        }

        public void SetSFXVolume(float SliderValue)
        {
            AudioManager.instance.SetVolume("SFX", SliderValue);
        }
    }
}