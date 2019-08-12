using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthMaskCubeScript : MonoBehaviour
{
    [SerializeField] int GameState;

    public void ChangeState()
    {
        Debug.Log("DepthMaskCubeScript event working");
        GameManager.instance.PSetState(GameState);
    }
}
