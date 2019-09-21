using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimController))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PathCreation.PathFollower))]

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Outline _compOutline = null;
    [SerializeField] PlayerAnimController _animController = null;
    [SerializeField] GameObject _compPointer = null;

    [SerializeField] PathCreation.PathFollower _pathFollower = null;

    public delegate void OnGameEvent(int i);
    public static event OnGameEvent onGameEvent;

    void Start()
    {
        if (!CheckRefs())
            return;

        SwitchComponents(false);
    }

    public void SwitchComponents(bool condition)
    {
        _compOutline.enabled = condition;
        _animController.enabled = condition;
        _compPointer.SetActive(condition);

        _pathFollower.enabled = !condition;
    }

    private void OnEnable()
    {
        _pathFollower.enabled = true;
    }

    private bool CheckRefs()
    {
        bool check = true;
        if(_compOutline == null)
        {
            Debug.Log("Outline : Ref Missing - on " +  transform.name);
            check = false;
        }
        else if(_animController == null)
        {
            Debug.Log(" PlayerAnimController : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (_compPointer == null)
        {
            Debug.Log("Pointer : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Vehicle")
        {
            Debug.Log("____________Vehicle Hit");

            if (onGameEvent != null)
            {
                onGameEvent(0);
            }
        }
        if (hit.transform.tag == "Finish")
        {
            Debug.Log("____________Finish");

            if (onGameEvent != null)
            {
                onGameEvent(1);
                Destroy(gameObject);
            }
        }
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
}
