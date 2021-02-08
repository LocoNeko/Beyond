using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Beyond
{

    public class UIButtons : MonoBehaviour, IPointerEnterHandler
    {
        //Do this when the cursor enters the rect area of this selectable UI object.
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.instance.PlayAudio(gameObject , AudibleObjects.BUTTON , AudibleEvents.UI_ENTER);
        }

    }
}