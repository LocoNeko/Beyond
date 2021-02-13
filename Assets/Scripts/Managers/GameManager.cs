using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MapMagic;
using UnityEngine.Audio;


namespace Beyond
{
    public class GameManager : MonoBehaviour
    {
        private Place place;
        private float timer;
        [SerializeField] private MapMagic.Core.MapMagicObject mm;
        [SerializeField] private GameObject FPSController;
        [SerializeField] private TextMeshProUGUI textTime;
        [SerializeField] private TextMeshProUGUI textDate;
        [SerializeField] private GameObject ImageCurrentWeather;

        [SerializeField] private Sprite Sunny;
        [SerializeField] private Sprite Cloudy;
        [SerializeField] private Sprite Cloudy2;
        [SerializeField] private Sprite LightRain;
        [SerializeField] private Sprite HeavyRain;

        public static GameManager instance ;


        private bool initialised;

        private void Awake()
        {
            initialised = false;
            instance = this;
            place = new Place();
        }

        // Start is called before the first frame update
        void Start()
        {
            //mm.graph.random.Seed = (int)(System.DateTime.Now.Ticks % 1000000);
            //mm.StartGenerate();

            // Register the EnviroSkyMgr OnWeatherChanged event
            EnviroSkyMgr.instance.OnWeatherChanged += (EnviroWeatherPreset type) =>
            {
                DoOnWeatherChange(type);
            };

        }

        void DoOnWeatherChange(EnviroWeatherPreset type)
        {
            //Debug.Log("Weather changed to "+type.name);
            //TODO : Change icon based on the EnviroWeatherPreset returned
            switch (type.name)
            {
                case "Cloudy 1":
                    ImageCurrentWeather.GetComponent<Image>().sprite = Cloudy;
                    break;
                case "Cloudy 2":
                    ImageCurrentWeather.GetComponent<Image>().sprite = Cloudy2;
                    break;
                case "Light Rain":
                    ImageCurrentWeather.GetComponent<Image>().sprite = LightRain;
                    break;
                case "Heavy Rain":
                    ImageCurrentWeather.GetComponent<Image>().sprite = HeavyRain;
                    break;
                default:
                    ImageCurrentWeather.GetComponent<Image>().sprite = Sunny;
                    break;
            }
        }

        void Update()
        {
            if (!initialised)
                Initialise();
            timer += Time.deltaTime;
            // Update every 1/10th of a second
            if(timer>0.1f)
            {
                place.update(timer);
                timer = 0f;

                textTime.text = place.gametime.TimeStr();
                textDate.text = place.gametime.DateOnlyStr();

                // TO DO : Makes this more efficient (only to happen when GameTime changed, should be an event)
                EnviroSky.instance.GameTime.Seconds = 0;
                EnviroSky.instance.GameTime.Minutes = place.gametime.getMinute();
                EnviroSky.instance.GameTime.Hours = place.gametime.getHour();
                EnviroSky.instance.GameTime.Days = place.gametime.DayInYear();
                EnviroSky.instance.GameTime.Years = place.gametime.getYear() + 2020;
            }
            /*
            Debug.Log(
                EnviroSky.instance.currentYear + "/" +
                EnviroSky.instance.currentDay + "/" +
                EnviroSky.instance.GetTimeStringWithSeconds());
            */
        }

        void Initialise()
        {
            if (!initialised)
            {
                if (!mm.IsGenerating())
                {
                    FPSController.SetActive(false);
                    Vector3 characterPosition = FPSController.transform.position;
                    Vector3 castPosition = characterPosition;
                    castPosition.y = 1000f;
                    RaycastHit hit;
                    int layerMask = 1 << 8; // Terrain layer
                    Physics.Raycast(castPosition, Vector3.down, out hit, Mathf.Infinity, layerMask);
                    characterPosition.y = hit.point.y + 2f;
                    FPSController.transform.position = characterPosition;
                    FPSController.SetActive(true);
                    initialised = true;
                }
            }
        }

        public float GetLoadProgress()
        {
            return mm.GetProgress();
        }

        public bool MapIsGenerating()
        {
            return mm.IsGenerating();
        }

        public void PauseGame()
        {
            if (place.gametime.getSpeed()==0)
            {
                place.gametime.setSpeed(1);
            }
            else
            {
                place.gametime.setSpeed(0);
            }
        }
    }
}
