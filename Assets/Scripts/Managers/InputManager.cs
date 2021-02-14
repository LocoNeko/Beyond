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
        [SerializeField] private List<GameAction> instantiatedButtonGroups;


        void Start()
        {
            instance = this;
            Cursor.visible = false;
            instantiatedButtonGroups = new List<GameAction>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                GameManager.instance.PauseGame();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                ActivateAction(ActionController.instance.GetActionByName("CreateBlueprint"), new List<GameAction>());
            }
            //Left click
            if (Input.GetMouseButtonDown(0))
            {
                BuildController.instance.TryPlacingBlueprint();
            }
        }

        public void ActivateAction(GameAction ga, List<GameAction> prefix, GameObject previousButton = null)
        {
            List<GameAction> nextActions = ga.NextActions;

            // The prefix of all buttons in the group is the current prefix + the action of this button
            //example : Clicking on "CreateBlueprint" from prefix "structure" generates prefix "CreateBlueprint","Structure"
            List<GameAction> newPrefix = new List<GameAction>();
            foreach (GameAction ga2 in prefix)
                newPrefix.Add(ga2);
            newPrefix.Add(ga);

            // 1-It leads to next actions and it should show them in a button group 
            if (nextActions.Count>0)
            {
                if (previousButton != null)
                    HideAllOtherButtonsInGroup(previousButton);
                GameObject newButtonGroup = CreateButtonGroup(ga, nextActions, newPrefix , previousButton);
                // Hide all button groups lower than this one
                HideAllGroupsBelow(newButtonGroup);
            }

            // 2-It has no next action, so should trigger the resolvable action
            else
            {
                HideAllGroupsBelow(FirstButtonAnchor);
                ActionController.Do(ga,prefix);
            }
        }

        private void HideAllGroupsBelow(GameObject newButtonGroup)
        {
            // Hide button group that is attached to any button in newButtonGroup
            List<GameObject> listOfChildren = new List<GameObject>();
            GetChildGroupRecursive(newButtonGroup, ref listOfChildren);
            foreach(GameObject go in listOfChildren)
            {
                go.SetActive(false);
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

        public GameObject CreateButtonGroup(GameAction action , List<GameAction> children, List<GameAction> prefix, GameObject rootGO = null)
        {
            GameObject buttonGroup=null;
            // Find this group if it was already instantiated and just inactive
            if (instantiatedButtonGroups.Contains(action))
            {
                buttonGroup = ShowInstantiatedGroup(action , rootGO);
            }

            // This is the first time we see this button group, let's build it
            else
            {
                buttonGroup = InstantiateGroup(action, children, prefix);
            }

            if (buttonGroup!=null)
            {
                // Set the group parent's to be the previousButton (or the first anchor by default)
                buttonGroup.transform.SetParent(rootGO == null ? FirstButtonAnchor.transform : rootGO.transform, false);
                buttonGroup.SetActive(true);
            }
            else
                Debug.LogError("Couldn't instantiate or find button group for "+action.Name);

            return buttonGroup;
        }

        // This group has never been instantiated, so let's do it here
        public GameObject InstantiateGroup(GameAction action, List<GameAction> children, List<GameAction> prefix)
        {
            GameObject buttonGroup = Instantiate(ButtonGroupPrefab);
            buttonGroup.name = "buttonGroup_" + action.Name;
            instantiatedButtonGroups.Add(action);

            // Find all buttons corresponding to this action
            // Create all buttons giving them : prefix, action , image
            foreach (GameAction subAction in children)
            {
                GameObject buttonAction = Instantiate(ButtonActionPrefab);
                buttonAction.name = "buttonAction_" + subAction.Name;
                UIButton UIbutton = buttonAction.GetComponent<UIButton>();

                if (UIbutton != null)
                    UIbutton.SetActions(prefix, subAction);
                else
                    Debug.LogError("Couldn't find UIButton on this button");
                buttonAction.transform.SetParent(buttonGroup.transform, false);
            }
            return buttonGroup;
        }

        // This group has been instantiated before, let's show it
        public GameObject ShowInstantiatedGroup(GameAction action, GameObject rootGO)
        {
            GameObject result = null;
            List<GameObject> listOfChildren = new List<GameObject>();
            GetChildGroupRecursive((rootGO == null ? FirstButtonAnchor : rootGO), ref listOfChildren);
            foreach (GameObject go in listOfChildren)
            {
                if (go.name == "buttonGroup_" + action.Name)
                {
                    go.SetActive(true);
                    result = go;
                    break;
                }
            }
            if (result != null)
                ShowAllButtonsInGroup(result); // re-activate all buttons in this group, they might have been deactivated by HideAllOtherButtonsInGroup(button)
            return result;
        }

    }

}
