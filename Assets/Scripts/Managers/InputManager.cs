using System;
using UnityEngine;

namespace Beyond
{
    public class InputManager : MonoBehaviour
    {

        [Header("UI Buttons")]
        [SerializeField]private GameObject BuildButton;

        void Start()
        {
            BuildButton.SetActive(false);
            Cursor.visible = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                GameManager.instance.PauseGame();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                BuildButton.SetActive(!BuildButton.activeSelf);
                Cursor.visible = BuildButton.activeSelf;
            }
        }

    }

}
