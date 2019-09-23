using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]

public abstract class ABPlayerScript : MonoBehaviour
{
    [SerializeField] private MISSIONTYPE missionType;
    public MISSIONTYPE MissionType { get => missionType; set => missionType = value; }

    Outline outlineComponent;
    PlayerAnimController animController;
    PathFollower pathFollower;

    [SerializeField] GameObject pointerComponent = null;

    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;


    [SerializeField] UnityEngine.UI.Image portraitUIImage;
    [SerializeField] UnityEngine.UI.Image fullUIImage;

    private void Awake()
    {
        outlineComponent = GetComponent<Outline>();
        animController = GetComponent<PlayerAnimController>();
        pathFollower = GetComponent<PathFollower>();
    }

    void Start()
    {
        if (!CheckRefs())
            return;

        SwitchComponents(false);
    }

    public void SwitchComponents(bool condition)
    {
        outlineComponent.enabled = condition;
        animController.enabled = condition;
        pointerComponent.SetActive(condition);

        pathFollower.enabled = !condition;
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
        else if (portraitUIImage == null)
        {
            Debug.LogError("Portrait Image : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (fullUIImage == null)
        {
            Debug.LogError("Full UI Image : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }
}
