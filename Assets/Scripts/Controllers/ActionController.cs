using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class ActionController : MonoBehaviour
    {
        public static ActionController instance;
        [SerializeField] public List<GameAction> AllActions { get; protected set; }

        public static string GetListActionsAsString(List<GameAction> lga)
        {
            List<string> result = new List<string>();
            foreach (GameAction ga in lga)
                result.Add(string.Format("{0}({1})",ga.Name,ga.ActionType));
            return string.Join(">", result);
        }

        public static void Do(GameAction ga , List<GameAction>prefix)
        {
            List<GameAction> completeList = new List<GameAction>();
            foreach (GameAction ga2 in prefix)
            {
                completeList.Add(ga2);
            }
            completeList.Add(ga);

            GameAction verb = completeList.Find(a => a.ActionType==ActionType.Verb);
            GameAction blueprint = completeList.Find(a => a.ActionType == ActionType.Blueprint);
            List<GameAction> categories = completeList.FindAll(a => a.ActionType == ActionType.Category);
            List<GameAction> materials = completeList.FindAll(a => a.ActionType == ActionType.Material);

            //Debug.Log(string.Format("The ActionController was asked to do: {0}", GetListActionsAsString(completeList)));
            if (verb.Name == "CreateBlueprint")
                BuildController.instance.CreateBlueprint(blueprint, BuildingMaterial.ActionsToMaterials(materials));
        }

        private void Awake()
        {
            AllActions = new List<GameAction>();
            instance = this;
            // TO DO : This will eventually come from a conf file, and probably be done when the Game wakes up
            GameAction a;
            a = new GameAction("CreateBlueprint", ActionType.Verb); AllActions.Add(a);
            a = new GameAction("Structure", ActionType.Category); AllActions.Add(a);
            a = new GameAction("Network", ActionType.Category); AllActions.Add(a);
            a = new GameAction("Furniture", ActionType.Category); AllActions.Add(a);
            a = new GameAction("Foundation", ActionType.Blueprint); AllActions.Add(a);
            a = new GameAction("Wall", ActionType.Blueprint); AllActions.Add(a);
            a = new GameAction("Floor", ActionType.Blueprint); AllActions.Add(a);
            a = new GameAction("Concrete", ActionType.Material); AllActions.Add(a);
            a = new GameAction("Wood", ActionType.Material); AllActions.Add(a);
            a = new GameAction("Metal", ActionType.Material); AllActions.Add(a);
            AllActions.Find(a => a.Name == "CreateBlueprint").AddNextAction(AllActions.Find(a => a.Name == "Structure"));
            AllActions.Find(a => a.Name == "CreateBlueprint").AddNextAction(AllActions.Find(a => a.Name == "Network"));
            AllActions.Find(a => a.Name == "CreateBlueprint").AddNextAction(AllActions.Find(a => a.Name == "Furniture"));
            AllActions.Find(a => a.Name == "Structure").AddNextAction(AllActions.Find(a => a.Name == "Foundation"));
            AllActions.Find(a => a.Name == "Structure").AddNextAction(AllActions.Find(a => a.Name == "Wall"));
            AllActions.Find(a => a.Name == "Structure").AddNextAction(AllActions.Find(a => a.Name == "Floor"));
            AllActions.Find(a => a.Name == "Foundation").AddNextAction(AllActions.Find(a => a.Name == "Concrete"));
            AllActions.Find(a => a.Name == "Foundation").AddNextAction(AllActions.Find(a => a.Name == "Wood"));
            AllActions.Find(a => a.Name == "Foundation").AddNextAction(AllActions.Find(a => a.Name == "Metal"));
        }

        public GameAction GetActionByName(string s)
        {
            return AllActions.Find(a => a.Name == s);
        }

    }
}