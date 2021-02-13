using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Beyond
{

    public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private List<GameAction> prefixedBy;
        [SerializeField] private GameAction action;


        //Do this when the cursor enters the rect area of this selectable UI object.
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayAudio(gameObject , AudibleObjects.BUTTON , AudibleEvents.UI_ENTER);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (InputManager.instance)
                InputManager.instance.ActivateAction(action, prefixedBy, transform.gameObject);
        }

        // Set the action and its prefix on a Button
        public void SetActions(List<GameAction> prefix , GameAction ga)
        {
            prefixedBy = prefix;
            this.action = ga;
            Image img = gameObject.transform.Find("ButtonImage").GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("UI/" + ga.Name);
        }

    }
}