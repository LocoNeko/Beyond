using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Beyond
{
    public enum AudibleObjects
    {
        BUTTON,
        SCREEN
    }
    public enum AudibleEvents
    {
        UI_ENTER,
        START
    }
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        #region Mixers
        [Header("Mixers")]
        [SerializeField] public AudioMixer MusicMixer;
        [SerializeField] public AudioMixer SFXMixer;
        #endregion

        #region Audio Clips
        [Header("Audio clips")]
        [SerializeField] public AudioClip Button_UIEnter;
        [SerializeField] public AudioClip Screen_MainTheme;
        #endregion

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            MusicMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1f)) * 20);
            SFXMixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1f)) * 20);
            MainMenuController.instance.InitialiseVolumeSliders(PlayerPrefs.GetFloat("MusicVolume", 1f), PlayerPrefs.GetFloat("SFXVolume", 1f));
        }

        public void SetVolume(string mixerName , float sliderValue)
        {
            if (mixerName == "Music")
            {
                MusicMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
                PlayerPrefs.SetFloat("MusicVolume", sliderValue);
            }
            if (mixerName == "SFX")
            {
                SFXMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
                PlayerPrefs.SetFloat("SFXVolume", sliderValue);
            }
        }

        public void PlayAudio(GameObject go , AudibleObjects audibleObject , AudibleEvents audibleEvent)
        {
            if (audibleObject == AudibleObjects.BUTTON && audibleEvent == AudibleEvents.UI_ENTER)
                GetGameObjectAudioSource(go , SFXMixer).PlayOneShot(Button_UIEnter);
            if (audibleObject == AudibleObjects.SCREEN && audibleEvent == AudibleEvents.START)
            {
                AudioSource audioSource = GetGameObjectAudioSource(go , MusicMixer);
                audioSource.loop = true;
                audioSource.PlayOneShot(Screen_MainTheme);
            }
        }

        AudioSource GetGameObjectAudioSource(GameObject go , AudioMixer mixer)
        {
            AudioSource source = go.GetComponent<AudioSource>();
            if (source == null)
                source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
            return source;
        }


    }
}
