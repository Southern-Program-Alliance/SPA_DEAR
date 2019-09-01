using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MainManager : MonoBehaviour
{
    public Camera MainCam;
    //public ARRaycastManager arRaycast;

    private GameObject _GoSelected;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isEditor)
        {
            // Listen for input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Raycast
                var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Ray ray = MainCam.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Player")
                    {
                        Debug.Log("-----------Its Working!!");

                        SelectNewPlayer(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
            // Listen for input
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast
                var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 200, 1 << 10))
                {
                    if (hit.transform.tag == "Player")
                    {
                        Debug.Log("Player Character Object Clicked");

                        SelectNewPlayer(hit.collider.gameObject);
                    }

                    if (hit.transform.tag == "TrafficLight")
                    {
                        Debug.Log("Traffic Light Object Clicked");

                        SelectTrafficLight(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<TrafficLightScript>().playAnim();
                    }
                }
            }
        }
        
    }

    private void SelectNewPlayer(GameObject newPlayer)
    {
        if (_GoSelected == newPlayer)
            return;

        if (_GoSelected != null)
            _GoSelected.GetComponent<PlayerScript>().SwitchComponents(false);

        _GoSelected = newPlayer;
        _GoSelected.GetComponent<PlayerScript>().SwitchComponents(true);

    }

    private void SelectTrafficLight(GameObject newLight)
    {

    }
}
