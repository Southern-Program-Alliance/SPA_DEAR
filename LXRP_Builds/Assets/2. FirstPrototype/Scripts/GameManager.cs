using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] Animation anim_depthMask = null;
    [SerializeField] WorldPlacementScript placementScript = null;
    [SerializeField] GameObject schoolKid;

    private enum GameState { Blank, Begin, Placed, Raised }
    private GameState curState = GameState.Blank;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetState(GameState.Begin);
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
        if (state == GameState.Begin)
        {
            //Debug.Log("Manager started");
            placementScript.enabled = true;
        }

        // Once the player pladces the object
        if (state == GameState.Placed)
        {
            //Debug.Log("World placed");

            anim_depthMask.enabled = true;
            anim_depthMask.Play();

            placementScript.enabled = false;
        }

        if (state == GameState.Raised)
        {
            //Debug.Log("World raised");
            anim_depthMask.gameObject.SetActive(false);
            schoolKid.SetActive(true);
        }
    }
}
