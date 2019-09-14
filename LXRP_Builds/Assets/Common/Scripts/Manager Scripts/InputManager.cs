using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour
{
    private Camera MainCam;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField] LayerMask shpereCastLayer;
    [SerializeField] float shpereCastRadius = 0.05f;

    private GameObject selectedCharacter;

    private bool movable;
    public bool Movable { get => movable; set => movable = value; }

    private void Awake()
    {
        // Setup Raycast
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        selectedCharacter = null;
        Movable = false;
    }

    #region Casting Methods
    void Update()
    {
        // Listen for input on mobile
        if (!Application.isEditor)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                InitiateCast();
            }
        }
        // Listen for input on editor
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                InitiateCast();
            }
        }
    }

    private void InitiateCast()
    {
        Vector3 rayOrigin = MainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // If movable then Raycast
        if (Movable)
        {
            if (Physics.Raycast(rayOrigin, MainCam.transform.forward, out hit, 200))
            {
                //Debug.DrawRay(rayOrigin, MainCam.transform.forward, Color.red, 1f);
                if (Movable)
                {
                    if (hit.transform.tag == "TrafficLight")
                    {
                        HandleTrafficControllerHit(hit.collider.gameObject);
                        return;
                    }
                    else
                    {
                        HandleWalkableHit(hit.point);
                        return;
                    }
                }
            }
            return;
        }

        // Else sphere cast
        else if(Physics.SphereCast(rayOrigin, shpereCastRadius, MainCam.transform.forward, out hit, 100f, shpereCastLayer))
        {
            //Debug.DrawRay(rayOrigin, MainCam.transform.forward, Color.green, 1f);

            if (hit.transform.tag == "Player")
            {
                //Debug.Log("Player Character Object Clicked");
                HandlePlayerHit(hit.collider.gameObject);
            }
        }        
    }
    #endregion


    #region Cast Hit Handler Methods
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
    #endregion
}
