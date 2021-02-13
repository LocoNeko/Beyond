using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public enum ActionType
    {
        Undefined, //Not sure
        Verb, //What we are doing
        Category, //Category (or categories) of what we are doing
        Blueprint, //Blueprint used
        Material // Material(or materials) used
    }

    public class GameAction
    {
        [SerializeField] public string Name { get; private set; }
        [SerializeField] public List<GameAction> NextActions { get; private set; } // All possible actions that come after this one
        [SerializeField] public Sprite ButtonSprite { get; private set; }
        [SerializeField] public ActionType ActionType { get; private set; }

        public GameAction(string name, ActionType actionType)
        {
            Name = name;
            ActionType = actionType;
            NextActions = new List<GameAction>();
            try
            {
                ButtonSprite = Resources.Load<Sprite>("UI/" + name);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Couldn't find sprite for action {0}. Exception {1}" , name, e));
            }
        }

        public bool AddNextAction(GameAction a)
        {
            if (!(NextActions.Contains(a)))
            {
                NextActions.Add(a);
                return true;
            }
            return false;
        }

        public void AddListOfNextActions(List<GameAction> la)
        {
            foreach (GameAction ga in la)
            {
                AddNextAction(ga);
            }
        }
    }
}
