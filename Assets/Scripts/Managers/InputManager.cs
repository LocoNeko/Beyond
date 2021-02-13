using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beyond
{


    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        [Header("UI Buttons")]
        [SerializeField] private GameObject FirstButtonAnchor;
        [SerializeField] private GameObject ButtonGroupPrefab;
        [SerializeField] private GameObject ButtonActionPrefab;
        // Remember which button groups have already been instantiated so I don't instantiate them again. SerializeField so I can peak (I like to watch)
        [SerializeField] private List<string> instantiatedButtonGroups;


        void Start()
        {
            instance = this;
            Cursor.visible = false;
            instantiatedButtonGroups = new List<string>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                GameManager.instance.PauseGame();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ActivateAction("Build", new List<string>());
            }
        }

        public void ActivateAction(string action, List<string> prefix, GameObject previousButton = null)
        {
            List<string> nextActions = GetNextActions(action);

            // The prefix of all buttons in the group is the current prefix + the action of this button
            //example : Clicking on "Build" from prefix "structure" generates prefix "Build","Structure"
            List<string> newPrefix = new List<string>();
            foreach (string s in prefix)
                newPrefix.Add(s);
            newPrefix.Add(action);

            // 1-It leads to next actions and it should show them in a button group 
            if (nextActions.Count>0)
            {
                if (previousButton != null)
                    HideAllOtherButtonsInGroup(previousButton);
                GameObject newButtonGroup = CreateButtonGroup(action, nextActions, newPrefix , previousButton);
                // Hide all button groups lower than this one
                HideAllGroupsBelow(newButtonGroup);
            }

            // 2-It has no next action, so should trigger the resolvable action
            else
            {
                Debug.Log(string.Format("The inputManager received resolvable action: {1}>{0}", action , string.Join(">", prefix)));
                HideAllGroupsBelow(FirstButtonAnchor);
            }
        }

        private void HideAllGroupsBelow(GameObject newButtonGroup)
        {
            // Hide button group that is attached to any button in newButtonGroup
            List<GameObject> listOfChildren = new List<GameObject>();
            GetChildGroupRecursive(newButtonGroup, ref listOfChildren);
            string s="";
            foreach(GameObject go in listOfChildren)
            {
                go.SetActive(false);
                s += go.name + ", ";
            }
        }

        private void HideAllOtherButtonsInGroup(GameObject previousButton)
        {
            foreach(Transform child in previousButton.transform.parent)
            {
                if (child.gameObject != previousButton)
                    child.gameObject.SetActive(false);
            }
        }

        private void ShowAllButtonsInGroup(GameObject buttonGroup)
        {
            foreach (Transform child in buttonGroup.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        private void GetChildGroupRecursive(GameObject go , ref List<GameObject> listOfChildren)
        {
            if (go == null)
                return;

            foreach (Transform child in go.transform)
            {
                if (child == null)
                    continue;
                if (child.name.StartsWith("buttonGroup"))
                {
                    listOfChildren.Add(child.gameObject);
                }
                GetChildGroupRecursive(child.gameObject, ref listOfChildren);
            }
        }

        public GameObject CreateButtonGroup(string action , List<string> children, List<string> prefix, GameObject rootGO = null)
        {
            GameObject buttonGroup=null;
            // Find this group if it was already instantiated and just inactive
            if (instantiatedButtonGroups.Contains(action))
            {
                List<GameObject> listOfChildren = new List<GameObject>();
                GetChildGroupRecursive((rootGO == null ? FirstButtonAnchor : rootGO), ref listOfChildren);
                foreach (GameObject go in listOfChildren)
                {
                    if (go.name== "buttonGroup_" + action)
                    {
                        go.SetActive(true);
                        buttonGroup = go;
                        break;
                    }
                }
                if (buttonGroup!=null)
                    ShowAllButtonsInGroup(buttonGroup); // re-activate all buttons in this group, they might have been deactivated by HideAllOtherButtonsInGroup(button)
            }
            else
            {
                // This is the first time we see this group, let's build it
                instantiatedButtonGroups.Add(action);
                buttonGroup = Instantiate(ButtonGroupPrefab);
                buttonGroup.name = "buttonGroup_" + action;

                // Find all buttons corresponding to this action
                // Create all buttons giving them : prefix, action , image
                foreach (string subAction in children)
                {
                    GameObject buttonAction = Instantiate(ButtonActionPrefab);
                    buttonAction.name = "buttonAction_" + subAction;
                    UIButton UIbutton = buttonAction.GetComponent<UIButton>();
                    
                    if (UIbutton != null)
                    {
                        UIbutton.SetActions(prefix, subAction);
                    }
                    else
                    {
                        Debug.LogError("Couldn't find UIButton on this button");
                    }
                    buttonAction.transform.SetParent(buttonGroup.transform, false);
                }
            }
            // Set the group parent's to be the previousButton (or the first anchor by default)
            buttonGroup.transform.SetParent(rootGO == null ? FirstButtonAnchor.transform : rootGO.transform , false);
            buttonGroup.SetActive(true);

            return buttonGroup;
        }

        public static List<string> GetNextActions(string action)
        {
            List<string> result = new List<string>();
            switch (action)
            {
                case "Build":
                    result.Add("Structure");
                    result.Add("Network");
                    result.Add("Furniture");
                    break;
                case "Structure":
                    result.Add("Foundation");
                    result.Add("Wall");
                    result.Add("Floor");
                    break;
                case "Foundation":
                    result.Add("Concrete");
                    result.Add("Wood");
                    result.Add("Metal");
                    break;
            }
            return result;
        }

    }

}
