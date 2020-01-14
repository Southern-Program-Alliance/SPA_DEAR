using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using PathCreation;

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
    [SerializeField] float LevelStartDelay = 6.0f;
    [SerializeField] float LevelEndDelay = 4.0f;

    public int currentLevel = 0;

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
            // Set game world 
            case EGameState.BEGIN:
                InitializeGame();
                break;

            case EGameState.PLACED:
                // Enable Game
                InitLevel();
                break;

            // When new Player is spawned
            case EGameState.PLAYER_START:
                SetupNewPlayerCharacter();
                break;

            // When new player selecteds
            case EGameState.QUEST_START:
                currentLevel++;
                StartMission(currSelectedPlayer.PlayerInfo.characterMission);
                break;

            // When player's mission is complete
            case EGameState.QUEST_COMPLETE:
                EndMission();
                break;

            // When player is taken to the station
            case EGameState.PLAYER_COMPLETE:
                 break;

            case EGameState.GAME_OVER:
                Destroy(currSelectedPlayer);
                UIManager.Instance.DisplayGameOverMessage();
                break;
        }
    }

    // Determine game worl placement method
    private void InitializeGame()
    {
        if (Application.isEditor)
        {
            placementScript.SetState(EARState.PLACEMENT);
            return;
        }
        placementScript.SetState(EARState.TUTORIAL);
    }

     // Initliase next level - spawn level elements and adjust score
    private void InitLevel()
    {
        UIManager.Instance.InitGameOverMessage();
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(ESpawnSelection.VEHICLES);

        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);

        UpdateScore(EScoreEvent.GAME_START);
    }

    // Initalize new level elements
    private void StartLevel()
    {
        //Debug.Log("Current Mission: " + currSelectedPlayer.PlayerInfo.characterMission);
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
        SetState(EGameState.QUEST_START);

    }
    
    // Complete end of level actions; message display etc.
    private void EndLevel()
    {
        UIManager.Instance.ShowLevelStatusText();
        UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_COMPLETE, currSelectedPlayer.PlayerInfo.characterMission);
        StartCoroutine(DisplayEndLevelDelay());
    }

    // Coroutine to delay end of level message display and end of level actions
    private IEnumerator DisplayEndLevelDelay()
    {
        yield return new WaitForSeconds(LevelEndDelay);
        Destroy(currSelectedPlayer);
        Destroy(GameObject.FindWithTag("Player"));
        //UIManager.Instance.HideLevelStatusText();
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);
    }

    // Set up the player appropriate to level
    private void SetupNewPlayerCharacter()
    {
        if (currSelectedPlayer != null)
        {
            // Enable UI 
            UIManager.Instance.SetPlayerInfo(currSelectedPlayer.PlayerInfo);
            StartLevel();
        }
    }

    // Place player in the game world map
    private void PlacePlayer()
    {
        float startAngle = 90.0f;

        GameObject ThePlayer = currSelectedPlayer.PlayerInfo.attachedObject;
        Vector3 pos = new Vector3(currSelectedPlayer.PlayerInfo.X, currSelectedPlayer.PlayerInfo.Y, currSelectedPlayer.PlayerInfo.Z);
        ThePlayer.transform.position = pos;
        Quaternion start = Quaternion.Euler(0, startAngle, 0);
        ThePlayer.transform.rotation = start;
    }

    // Enact specifics of a given level
    public void StartMission(EMissionType mission)
    {
        PlacePlayer();

        if (currSelectedPlayer.PlayerInfo.characterMission == EMissionType.COLLECT_HOTDOGS)
            currSelectedPlayer.PlayerInfo.instructionsIndex = levelOnePartTwoIndex;

        // Start Talk UI
        //UIManager.Instance.StartIntroSpeech(currSelectedPlayer.PlayerInfo.introSpeechText, 
        //    currSelectedPlayer.PlayerInfo.portraitImage, currSelectedPlayer.PlayerInfo.instructionsIndex);
        
        switch (mission)
        {
            case EMissionType.FIND_CORRECT_RULES:
                SpawnManager.Instance.StartSpawn(ESpawnSelection.RULES);
                break;
        }
    }

    // Complete actions to end a level
    public void EndMission()
    {
        EndLevel();
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
        string tagVal = "";
                
        info.IsSelected = isSelected;

        if (isSelected && !selectedRules.Contains(info))
        {
            selectedRules.Add(info);
        }

        if (isSelected)
        {
            tagVal = RulesScript.bookTag;
            //Debug.Log("TAGVAL: " + tagVal);
            Destroy(GameObject.FindGameObjectWithTag(tagVal));
            //Debug.Log("Destroyed " + tagVal);
        }
        else
        {
            selectedRules.Remove(info);
            //Debug.Log("_________________________fdxgnfn");
        }
        UIManager.Instance.UpdateRules(selectedRules.Count);
        if (selectedRules.Count == 5)
        {
            GameObject.FindGameObjectWithTag("RuleUI").SetActive(false);
            MainManager.Instance.SetState(EGameState.QUEST_COMPLETE);
        }
    }

    public void SetVehicleSpeed(float inSpeed)
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Vehicle");
        foreach (GameObject car in cars)
        {
            car.GetComponent<PathFollower>().speed = inSpeed;
        }
    }
    
    public void UpdateScore(EScoreEvent eScoreEvent)
    {
        switch (eScoreEvent)
        {
            case EScoreEvent.GAME_START:
                score = 10;
                UIManager.Instance.UpdateScore(score);
                break;

            case EScoreEvent.ON_ROAD:
                if (score > 0)
                    score -= 5;
                UIManager.Instance.UpdateScore(score);
                break;
            case EScoreEvent.AT_STATION:
                score += 20;
                UIManager.Instance.UpdateScore(score);
                break;
        }
        if (score <= 0)
            SetState(EGameState.GAME_OVER);
    }

    #endregion
}

