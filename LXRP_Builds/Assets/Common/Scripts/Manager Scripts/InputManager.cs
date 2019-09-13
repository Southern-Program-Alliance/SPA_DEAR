using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour
{
    private Camera MainCam;
    private Ray ray;
    private RaycastHit hit;

    private GameObject selectedCharacter;

    private bool movable;

    private void Awake()
    {
        // Setup Raycast
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        selectedCharacter = null;
        movable = false;
    }

    void Update()
    {
        // Listen for input on mobile
        if (!Application.isEditor)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ray = MainCam.ScreenPointToRay(Input.GetTouch(0).position);
                InitiateRaycast();
            }
        }
        // Listen for input on editor
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = MainCam.ScreenPointToRay(Input.mousePosition);
                InitiateRaycast();
            }
        }
    }

    private void InitiateRaycast()
    {
        // If raycast hits the interactable layer
        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log("Initiating Raycast");
            if (movable)
            {   
                if (hit.transform.tag == "TrafficLight")
                {
                    HandleTrafficControllerHit(hit.collider.gameObject);
                }
                else
                {
                    HandleWalkableHit(hit.point);
                }
            }

            if (hit.transform.tag == "Player")
            {
                Debug.Log("Player Character Object Clicked");
                HandlePlayerHit(hit.collider.gameObject);
            }
        }
    }

    private void HandleWalkableHit(Vector3 pos)
    {
        // If no character is selected
        if (!selectedCharacter)
            return;

        selectedCharacter.GetComponent<NavMeshAgent>().SetDestination(pos);
        TargetLocation.Instance.SetLocation(pos);
    }

    private void HandlePlayerHit(GameObject clickedPlayer)
    {
        SelectNewPlayer(hit.collider.gameObject);
        movable = true;
    }

    private void HandleTrafficControllerHit(GameObject clickedTrafficController)
    {
        Debug.Log("Traffic Light Object Clicked");

        hit.collider.gameObject.GetComponentInParent<TrafficLightScript>().OnTrafficButtonPress();
    }

    private void SelectNewPlayer(GameObject newPlayer)
    {
        if (selectedCharacter == newPlayer)
            return;

        if (selectedCharacter != null)
            selectedCharacter.GetComponent<PlayerScript>().SwitchComponents(false);

        selectedCharacter = newPlayer;
        selectedCharacter.GetComponent<PlayerScript>().SwitchComponents(true);
    }
}
