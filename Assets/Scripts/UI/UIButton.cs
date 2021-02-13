using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Beyond
{

    public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private List<string> prefixedBy;
        [SerializeField] private string action;


        //Do this when the cursor enters the rect area of this selectable UI object.
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlayAudio(gameObject , AudibleObjects.BUTTON , AudibleEvents.UI_ENTER);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InputManager.instance.ActivateAction(action, prefixedBy, transform.gameObject);
        }

        public void SetActions(List<string> prefix , string action)
        {
            prefixedBy = prefix;
            this.action = action;
            Image img = gameObject.transform.Find("ButtonImage").GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("UI/" + action);
        }

    }
}