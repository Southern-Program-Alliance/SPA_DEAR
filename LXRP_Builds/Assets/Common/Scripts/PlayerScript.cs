using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomCharacterController))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Outline _compOutline = null;
    [SerializeField] CustomCharacterController _compCusCharCon = null;
    [SerializeField] GameObject _compPointer = null;

    private bool CheckRefs()
    {
        bool check = true;
        if(_compOutline == null)
        {
            Debug.Log("Outline : Ref Missing - on " +  transform.name);
            check = false;
        }
        else if(_compCusCharCon == null)
        {
            Debug.Log("CustomCharacterController : Ref Missing - on " + transform.name);
            check = false;
        }
        else if (_compPointer == null)
        {
            Debug.Log("Pointer : Ref Missing - on " + transform.name);
            check = false;
        }
        return check;
    }

    public void SwitchComponents(bool condition)
    {
        _compOutline.enabled = condition;
        _compCusCharCon.enabled = condition;
        _compPointer.SetActive(condition);

    }

    void Start()
    {
        if (!CheckRefs())
            return;

        SwitchComponents(false);
    }
}
