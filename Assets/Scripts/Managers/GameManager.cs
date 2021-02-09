using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            Debug.Log("Weather changed to "+type.name);
        }

        void Update()
        {
            if (!initialised)
                Initialise();
            timer += Time.deltaTime;
            if(timer>0.1f)
            {
                place.update(timer);
                timer = 0f;

                textTime.text = place.gametime.TimeStr();
                textDate.text = place.gametime.DateOnlyStr();

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
    }
}
