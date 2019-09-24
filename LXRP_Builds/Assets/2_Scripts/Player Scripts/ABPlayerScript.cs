using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.AI;
using TMPro;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]
[RequireComponent(typeof(NavMeshAgent))]

public abstract class ABPlayerScript : MonoBehaviour, IClickable
{
    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;

    Outline outlineComponent;
    PlayerAnimController animController;
    PathFollower pathFollower;
    NavMeshAgent navMeshAgent;

    [SerializeField] GameObject floatingText;
    [SerializeField] GameObject pointerComponent = null;
    [SerializeField] SO_PlayerInfo playerInfo = null;

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Vehicle")
        {
            Debug.Log("____________Vehicle Hit");

            if (onGameEvent != null)
            {
                onGameEvent(0);
            }
        }
    }

    private void ShowFloatingText()
    {
        //floatingText.en
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
        else if (floatingText == null)
        {
            Debug.LogError("Floating Text : Ref Missing - on " + transform.name);
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

        MainManager.Instance.SetState(GAMESTATE.QUEST_START);
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
