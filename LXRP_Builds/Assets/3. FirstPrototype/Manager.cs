using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;

    [SerializeField] WorldPlacementScript placementScript = null;
    [SerializeField] GameObject schoolKid = null;

    private GameState curState = GameState.BLANK;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetState(GameState.BEGIN);
    }

    public void PSetState(int state)
    {
        SetState((GameState)state);
    }

    void SetState(GameState newState)
    {
        if (curState == newState)
        {
            return;
        }
  
        curState = newState;
        //Debug.Log(curState);
        HandleStateChangedEvent(curState);
    }

    // Function to set up animations on state changes
    void HandleStateChangedEvent(GameState state)
    {
        // Starting state of the game
        if (state == GameState.BEGIN)
        {
            //Debug.Log("Manager started");
            placementScript.enabled = true;
        }

        // Once the player places the object
        if (state == GameState.PLACED)
        {
            //Debug.Log("World placed");

            placementScript.enabled = false;
            ActivateWorld();
        }

        //if (state == GameState.RAISED)
        //{
        //    //Debug.Log("World raised");
        //    schoolKid.SetActive(true);
        //}
    }

    IEnumerator ActivateWorld()
    {
        yield return new WaitForSeconds(3.0f);
        //SetState(GameState.RAISED);
    }
}
