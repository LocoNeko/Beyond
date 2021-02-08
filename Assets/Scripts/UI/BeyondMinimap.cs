using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class BeyondMinimap : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;

        void LateUpdate()
        {
            Vector3 minimapCameraPosition = playerTransform.position;
            minimapCameraPosition.y = transform.position.y;
            transform.position = minimapCameraPosition;
            transform.eulerAngles = new Vector3(90,playerTransform.rotation.eulerAngles.y,0);
        }
    }
}
