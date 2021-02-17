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
        private float timer;
        [SerializeField] public Place Place { get; protected set; }

        [Header("Useful Objects")]
        [SerializeField] private MapMagic.Core.MapMagicObject mm;
        [SerializeField] private GameObject FPSController;
        [SerializeField] private TextMeshProUGUI textTime;
        [SerializeField] private TextMeshProUGUI textDate;
        [SerializeField] private GameObject ImageCurrentWeather;

        [Header("Weather sprites")]
        [SerializeField] private Sprite Sunny;
        [SerializeField] private Sprite Cloudy;
        [SerializeField] private Sprite Cloudy2;
        [SerializeField] private Sprite LightRain;
        [SerializeField] private Sprite HeavyRain;

        [SerializeField] public List<Template> Templates { get; protected set; }
        //TO DO : Not sure the list of BuildingMaterials belongs here
        [SerializeField] public List<BuildingMaterial> BuildingMaterials { get; protected set; }

        public static GameManager instance ;

        private bool initialised;

        private void Awake()
        {
            initialised = false;
            instance = this;
            Place = new Place();
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
                Place.update(timer);
                timer = 0f;

                textTime.text = Place.Gametime.TimeStr();
                textDate.text = Place.Gametime.DateOnlyStr();

                // TO DO : Makes this more efficient (only to happen when GameTime changed, should be an event)
                EnviroSky.instance.GameTime.Seconds = 0;
                EnviroSky.instance.GameTime.Minutes = Place.Gametime.getMinute();
                EnviroSky.instance.GameTime.Hours = Place.Gametime.getHour();
                EnviroSky.instance.GameTime.Days = Place.Gametime.DayInYear();
                EnviroSky.instance.GameTime.Years = Place.Gametime.getYear() + 2020;
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
                }

                //TO DO : all of this should be in a conf file

                //Initiailise BuildingMaterials TODO : may rename them to BuildingMaterials
                BuildingMaterials = new List<BuildingMaterial>();

                BuildingMaterials.Add(new BuildingMaterial("Concrete"));
                BuildingMaterials.Add(new BuildingMaterial("Wood"));
                BuildingMaterials.Add(new BuildingMaterial("Metal"));

                // Initiliase templates
                Templates = new List<Template>();

                // Foundation
                Constraint c1 = new Constraint(Operation.BaseIn, new List<object>() { 0.25f });
                Constraint c2 = new Constraint(Operation.TopClear, new List<object>());
                Constraint c3 = new Constraint(Operation.AvoidCollision, new List<object>() { "Buildings" });
                Constraint c = new Constraint(Operation.And, new List<object>() { c1, c2, c3 });
                Template t = new Template(
                    "Foundation",
                    Resources.Load("Prefabs/Blueprints/Foundation") as GameObject,
                    c ,
                    new Vector3(0.5f, 0.5f, 0.5f)
                );
                Templates.Add(t);
                initialised = true;
            }
        }

        public Template GetTemplate(string name)
        {
            return Templates.Find(t => t.Name == name);
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
            if (Place.Gametime.getSpeed()==0)
            {
                Place.Gametime.setSpeed(1);
            }
            else
            {
                Place.Gametime.setSpeed(0);
            }
        }
    }
}
