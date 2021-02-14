using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class BeyondComponent : MonoBehaviour
    {
        [SerializeField ]public Template Template {get; protected set;}

        public void SetTemplate(Template t)
        {
            Template = t;
        }
    }
}