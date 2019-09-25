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
    ABPlayerScript currSelectedPlayer;
    public ABPlayerScript CurrSelectedPlayer { get => currSelectedPlayer;}
    

    [Space]
    public List<SO_RuleInfo> selectedRules;

    #region Private Methods

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


    // Function to handle state changes
    private void HandleStateChangedEvent(GAMESTATE state)
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
            
            // When new Player is spawned
            case GAMESTATE.PLAYER_START:
                SetupNewPlayerCharacter();
                break;

            // When new player is found
            case GAMESTATE.QUEST_START:
                StartMission(currSelectedPlayer.PlayerInfo.characterMission);
                break;

            // When player's mission is complete
            case GAMESTATE.QUEST_COMPLETE:
                //SetupNewPlayerCharacter();
                break;

            // When player is taken to the station
            case GAMESTATE.PLAYER_COMPLETE:
                //SetupNewPlayerCharacter();
                break;
        }
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

    private void StartGame()
    {
        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.VEHICLES);

        SpawnManager.Instance.StartSpawn(SPAWNSELECTION.PLAYERS);
    }

    private void SetupNewPlayerCharacter()
    {
        if (currSelectedPlayer != null)
        {
            // Enable UI 
            UIManager.Instance.SetPlayerInfo(currSelectedPlayer.PlayerInfo);
        }
        // Look for player
        InputManager.Instance.IsLookingForPlayer = true;
    }

    private void StartMission(MISSIONTYPE mission)
    {
        // Start Talk UI
        UIManager.Instance.StartIntroSpeech(currSelectedPlayer.PlayerInfo.introSpeechText
            , currSelectedPlayer.PlayerInfo.portraitImage);
        
        switch (mission)
        {
            case MISSIONTYPE.FIND_CORRECT_RULES:
                SpawnManager.Instance.StartSpawn(SPAWNSELECTION.RULES);
                break;
        }
        // Change Casting on Input Manager
        InputManager.Instance.IsLookingForPlayer = false;
    }

    #endregion

    #region Public Methods

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

    public void SetState(GAMESTATE newState, ABPlayerScript player)
    {
        currSelectedPlayer = player;
        SetState(newState);
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

    #endregion
}

