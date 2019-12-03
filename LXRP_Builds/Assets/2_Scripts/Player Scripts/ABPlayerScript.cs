using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;
using TMPro;
using System;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]
[RequireComponent(typeof(NavMeshAgent))]

public abstract class ABPlayerScript : MonoBehaviour, IClickable
{
    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;

    private Outline outlineComponent;
    private PlayerAnimController animController;
    private PathFollower pathFollower;
    private NavMeshAgent navMeshAgent;

    [SerializeField] GameObject pointerComponent = null;
    [SerializeField] SO_PlayerInfo playerInfo = null;
    private bool isDecreaseScoreRunning = false;
    private bool isOnCrossing = false;
    private bool isOnRoad;

    public SO_PlayerInfo PlayerInfo { get => playerInfo; }

    private void Awake()
    {
        outlineComponent = GetComponent<Outline>();
        animController = GetComponent<PlayerAnimController>();
        pathFollower = GetComponent<PathFollower>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (!CheckRefs())
            return;

        playerInfo.attachedObject = gameObject;

        SwitchComponents(false);
    }

    private void OnEnable()
    {
        pathFollower.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player entered " + other.name);

        if (other.gameObject.tag == "Vehicle")
        {
            //Debug.Log("Player got hit by a vehicle");
        }
        else if (other.gameObject.tag == "PedestrianBalcombe")
        {
            Debug.Log("Xing ENTER");
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "ComoCrossing")
        {
            Debug.Log("Como ENTER");
            MainManager.Instance.SetVehicleSpeed(0.0f);
            isOnCrossing = true;
        }
        else if (other.gameObject.tag == "Road")
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                Debug.Log("Player stepped on to the road - Collider");
                Debug.Log("Is On Crosssing: " + isOnCrossing);
                isOnRoad = true;

                if (!isOnCrossing)
                    StartCoroutine(DecreaseScore());
            }
        }
        else if (other.gameObject.tag == "Station")
        {
            Debug.Log("Current Level:" + MainManager.Instance.currentLevel);
            if (MainManager.Instance.currentLevel == 1)
            {
                Debug.Log("Station hit");
                Outline outline = other.GetComponent<Outline>();
                outline.OutlineWidth = 0.0f;
                MainManager.Instance.UpdateScore(EScoreEvent.AT_STATION);
                MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
            }
        }

    }

    IEnumerator DecreaseScore()
    {
        if (!isDecreaseScoreRunning && isOnRoad)
        {
            isDecreaseScoreRunning = true;

            while (isOnRoad)
            {
                yield return new WaitForSeconds(1.5f);

                MainManager.Instance.UpdateScore(EScoreEvent.ON_ROAD);
                isDecreaseScoreRunning = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Player exited " + other.name);
        if (other.gameObject.tag == "Road")
        {
            if (MainManager.Instance.GetState() == EGameState.QUEST_START)
            {
                Debug.Log("Player stepped off the road - Trigger exit");
                // Game End
                isOnRoad = false;
            }
        }
        else if (other.gameObject.tag == "PedestrianBalcombe")
        {
            Debug.Log("Xing EXIT");
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
        else if (other.gameObject.tag == "ComoCrossing")
        {
            Debug.Log("Como EXIT");
            isOnCrossing = false;
            MainManager.Instance.SetVehicleSpeed(1.3f);
        }
    }

    private bool CheckRefs()
    {
        bool check = true;
        if (outlineComponent == null)
        {
            Debug.LogError("Outline : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (animController == null)
        {
            Debug.LogError(" PlayerAnimController : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pointerComponent == null)
        {
            Debug.LogError("Pointer : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (pathFollower == null)
        {
            Debug.LogError("PathFollower : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (PlayerInfo == null)
        {
            Debug.LogError("Character UI Info : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }

    public void SwitchComponents(bool condition)
    {
        outlineComponent.enabled = condition;
        animController.enabled = condition;
        pointerComponent.SetActive(condition);

        pathFollower.enabled = !condition;
    }

    public void OnClick()
    {
        if (MainManager.Instance.CurrSelectedPlayer != this)
            return;

        SwitchComponents(true);

        MainManager.Instance.SetState(EGameState.QUEST_START);
    }

    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }

    public void MoveToDestination(Vector3 pos)
    {
        navMeshAgent.SetDestination(pos);
    }
}
