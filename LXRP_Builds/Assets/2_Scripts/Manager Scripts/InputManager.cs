using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    // Singleton Members
    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // Setup Raycast
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        selectedCharacter = null;
        IsMovable = false;

        Input.multiTouchEnabled = false;
    }

    private Camera MainCam;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField] LayerMask shpereCastLayer;
    [SerializeField] float shpereCastRadius = 0.05f;

    private GameObject selectedCharacter;
    public GameObject SelectedCharacter => selectedCharacter;

    private bool isMovable;
    public bool IsMovable { get => isMovable; set => isMovable = value; }

    #region Casting Methods
    void Update()
    {
        // Listen for input on the editor
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                InitiateCast();
            }
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!EventSystem.current.IsPointerOverGameObject(0))
                InitiateCast();
        }
    }

    private void InitiateCast()
    {
        Vector3 rayOrigin = MainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // If movable then Raycast
        if (IsMovable)
        {
            if (Physics.Raycast(rayOrigin, MainCam.transform.forward, out hit, 200))
            {
                //Debug.DrawRay(rayOrigin, MainCam.transform.forward, Color.red, 1f);
                if (IsMovable)
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
                HandlePlayerHit(hit.collider.gameObject);
            }

            if(hit.transform.tag == "Rule")
            {
                HandleRuleHit(hit.collider.gameObject);
            }
        }        
    }
    #endregion


    #region Cast Hit Handler Methods
    private void HandleWalkableHit(Vector3 pos)
    {
        TargetLocation.Instance.gameObject.SetActive(false);

        // If no character is selected
        if (!SelectedCharacter)
            return;

        SelectedCharacter.GetComponent<NavMeshAgent>().SetDestination(pos);
        TargetLocation.Instance.SetLocation(pos);

        TargetLocation.Instance.gameObject.SetActive(true);
    }


    private void HandlePlayerHit(GameObject clickedPlayer)
    {
        //Debug.Log("Player Character Object Clicked");
        SelectNewPlayer(hit.collider.gameObject);
        isMovable = true;
    }

    private void HandleRuleHit(GameObject clickedRule)
    {
        //Debug.Log("Rule Game Object Clicked");
        UIManager.Instance.SetRuleInfo(clickedRule.GetComponent<RulesScript>().RuleInfo);
    }

    private void HandleTrafficControllerHit(GameObject clickedTrafficController)
    {
        Debug.Log("Traffic Light Object Clicked");

        hit.collider.gameObject.GetComponentInParent<TrafficLightScript>().OnTrafficButtonPress();
    }


    private void SelectNewPlayer(GameObject newPlayer)
    {
        if (SelectedCharacter == newPlayer)
            return;

        if (SelectedCharacter != null)
            SelectedCharacter.GetComponent<ABPlayerScript>().SwitchComponents(false);

        selectedCharacter = newPlayer;
        selectedCharacter.GetComponent<ABPlayerScript>().SwitchComponents(true);

        MainManager.Instance.SetState(GAMESTATE.QUEST_START);
    }
    #endregion
}
