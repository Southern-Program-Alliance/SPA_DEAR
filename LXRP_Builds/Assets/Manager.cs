using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Manager : MonoBehaviour
{
    public Camera MainCam;
    public ARRaycastManager arRaycast;

    // Update is called once per frame
    void Update()
    {
        // Listen for input
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Raycast
            var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            Ray ray = MainCam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "Player")
                {
                    Debug.Log("-----------Its Working!!");
                }
            }
        }
    }
}
