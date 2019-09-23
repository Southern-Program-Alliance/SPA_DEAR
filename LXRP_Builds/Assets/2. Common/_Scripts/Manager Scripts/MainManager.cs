using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

// Singleton class
public class MainManager : MonoBehaviour
{
    // Singleton Members
    private static MainManager _instance;
    public static MainManager Instance { get { return _instance; } }

    [SerializeField] WorldPlacementScript placementScript = null;

    GAMESTATE managerState;

    // Main selected character
    GameObject currentSelectedCharacter;
    public GameObject CurrentSelectedCharacter { get => currentSelectedCharacter; set => currentSelectedCharacter = value; }
    

    [Space]
    public List<SO_RuleInfo> selectedRules;
   
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
    }

    private void Start()
    {
        SetState(GAMESTATE.BEGIN);
    }

    // Function to change the state
    public void SetState(GAMESTATE newState)
    {
        if (managerState == newState)
        {
            return;
        }

        Debug.Log("Manager State changed from:  " + managerState + "  to:  " + newState);
        managerState = newState;
        HandleStateChangedEvent(managerState);
    }

    // Function to handle state changes
    void HandleStateChangedEvent(GAMESTATE state)
    {
        switch (state)
        {
            case GAMESTATE.BEGIN:
                InitializeGame();
                break;

            case GAMESTATE.PLACED:
                // Enable Game
                StartGame();
                break;

            case GAMESTATE.PLAYER_START:
                SetupNewPlayerCharacter();
                break;

            case GAMESTATE.PLAYER_COMPLETE:
                //SetupNewPlayerCharacter();
                break;

            case GAMESTATE.QUEST_START:
                //SetupNewPlayerCharacter();
                break;

            case GAMESTATE.QUEST_COMPLETE:
                //SetupNewPlayerCharacter();
                break;
        }
    }

    private void SetupNewPlayerCharacter()
    {
        currentSelectedCharacter = InputManager.Instance.SelectedCharacter;

        // Enable UI 
    }

    private void InitializeGame()
    {
        if (Application.isEditor)
        {
            placementScript.SetState(ARSTATE.PLACEMENT);
            return;
        }
        placementScript.SetState(ARSTATE.TUTORIAL);
    }

    void StartGame()
    {
        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.VEHICLES);

        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.PLAYERS);
    }

    public void OnRuleSelect(bool isSelected, SO_RuleInfo info)
    {
        info.IsSelected = isSelected;
        if (isSelected && !selectedRules.Contains(info))
        {
            selectedRules.Add(info);
        }
        else
        {
            selectedRules.Remove(info);
        }
        UIManager.Instance.UpdateRules(selectedRules.Count);
    }
}

