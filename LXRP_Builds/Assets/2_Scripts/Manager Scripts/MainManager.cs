using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

// Singleton class
public class MainManager : MonoBehaviour
{
    // Singleton Members
    private static MainManager _instance;
    public static MainManager Instance { get { return _instance; } }

    // Main selected character
    ABPlayerScript currSelectedPlayer;
    public ABPlayerScript CurrSelectedPlayer { get => currSelectedPlayer; }

    private EGameState managerState = EGameState.BLANK;
    private List<SO_RuleInfo> selectedRules = new List<SO_RuleInfo>();

    [SerializeField] WorldPlacementScript placementScript = null;
    [SerializeField] float LevelStartDelay = 2.0f;

    // constant
    int levelOnePartTwoIndex = 1;

    private int score = 0;

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
        SetState(EGameState.BEGIN);
    }


    // Function to handle state changes
    private void HandleStateChangedEvent(EGameState state)
    {
        switch (state)
        {
            case EGameState.BEGIN:
                InitializeGame();
                break;

            case EGameState.PLACED:
                // Enable Game
                //StartGame();
                InitLevel();
                break;

            // When new Player is spawned
            case EGameState.PLAYER_START:
                SetupNewPlayerCharacter();
                // For Stage 2 - Same above function
                break;

            // When new player selecteds
            case EGameState.QUEST_START:
                StartMission(currSelectedPlayer.PlayerInfo.characterMission);
                // For Stage 2 - begin
                break;

            // When player's mission is complete
            case EGameState.QUEST_COMPLETE:
                //SetupNewPlayerCharacter();
                break;

            // When player is taken to the station
            case EGameState.PLAYER_COMPLETE:
                //SetupNewPlayerCharacter();
                break;
        }
    }

    private void InitializeGame()
    {
        if (Application.isEditor)
        {
            placementScript.SetState(EARState.PLACEMENT);
            return;
        }
        placementScript.SetState(EARState.TUTORIAL);
    }

    //private void startgame()
    //{
    //    mainmanager.instance.setstate(egamestate.level2_start);
    //    debug.log("state: " + mainmanager.instance.getstate());
    //}

    // Initliase next level - spawn level elements and adjust score
    private void InitLevel()
    {
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(ESpawnSelection.VEHICLES);

        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);

        UpdateScore(EScoreEvent.GAME_START);
    }

    private void StartLevel()
    {
        // Display "Level" label 
        UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_START, currSelectedPlayer.PlayerInfo.characterMission);

        // Delay then display character level instructions
        StartCoroutine(DelayThenStartLevel());
    }

    // Coroutine to delay (2 seconds default) then show level instructions 
    private IEnumerator DelayThenStartLevel()
    {
        yield return new WaitForSeconds(LevelStartDelay);
        UIManager.Instance.HideLevelStatusText();

        // Display Level Instructions UI
        UIManager.Instance.StartIntroSpeech(currSelectedPlayer.PlayerInfo.instructionsSpeechText
           , currSelectedPlayer.PlayerInfo.portraitImage, currSelectedPlayer.PlayerInfo.instructionsIndex);

    }

    private void SetupNewPlayerCharacter()
    {
        if (currSelectedPlayer != null)
        {
            // Enable UI 
            UIManager.Instance.SetPlayerInfo(currSelectedPlayer.PlayerInfo);
            StartLevel();
        }
    }

    private void StartMission(EMissionType mission)
    {
        // if playinf 2nd part of Level 1, shift index to further along in instructions array
        if (currSelectedPlayer.PlayerInfo.characterMission == EMissionType.COLLECT_HOTDOGS)
            currSelectedPlayer.PlayerInfo.instructionsIndex = levelOnePartTwoIndex;

        // Start Talk UI
        UIManager.Instance.StartIntroSpeech(currSelectedPlayer.PlayerInfo.introSpeechText, 
            currSelectedPlayer.PlayerInfo.portraitImage, currSelectedPlayer.PlayerInfo.instructionsIndex);
        
        switch (mission)
        {
            case EMissionType.FIND_CORRECT_RULES:
                SpawnManager.Instance.StartSpawn(ESpawnSelection.RULES);
                break;
        }
    }

    #endregion

    #region Public Methods

    // Function to change the state
    public void SetState(EGameState newState)
    {
        if (managerState == newState)
        {
            return;
        }

        //Debug.Log("Manager State changed from:  " + managerState + "  to:  " + newState);
        managerState = newState;
        HandleStateChangedEvent(managerState);
    }

    public void SetState(EGameState newState, ABPlayerScript player)
    {
        currSelectedPlayer = player;
        SetState(newState);
    }

    public EGameState GetState()
    {
        return managerState;
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
            Debug.Log("_________________________fdxgnfn");
        }
        UIManager.Instance.UpdateRules(selectedRules.Count);
    }

    public void UpdateScore(EScoreEvent eScoreEvent)
    {
        switch (eScoreEvent)
        {
            case EScoreEvent.GAME_START:
                UIManager.Instance.UpdateScore(score);
                break;

            case EScoreEvent.ON_ROAD:
                score -= 5;
                UIManager.Instance.UpdateScore(score);
                break;
        }
    }

    #endregion
}

