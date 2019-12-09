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
    [SerializeField] GameObject UI = null;
    public  int currentLevel = 0;
    string namechild;
    string tagVal;
    public static int index;
    public static string lasttag;

    // constant
    int levelOnePartTwoIndex = 1;

    private int score = 50;

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
                currentLevel++;
                StartMission(currSelectedPlayer.PlayerInfo.characterMission);
                // For Stage 2 - begin
                break;

            // When player's mission is complete
            case EGameState.QUEST_COMPLETE:
                //SetupNewPlayerCharacter();
                EndMission();
                break;

            // When player is taken to the station
            case EGameState.PLAYER_COMPLETE:
                //SetupNewPlayerCharacter();
                break;

            case EGameState.GAME_OVER:
                Destroy(currSelectedPlayer);
                UIManager.Instance.DisplayGameOverMessage();
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
        UIManager.Instance.InitGameOverMessage();
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PEDESTRIANS);
        SpawnManager.Instance.StartSpawn(ESpawnSelection.VEHICLES);

        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);

        UpdateScore(EScoreEvent.GAME_START);
    }

    private void StartLevel()
    {
        Debug.Log("Current Mission: " + currSelectedPlayer.PlayerInfo.characterMission);
        // Display "Level" label 

        if (currSelectedPlayer.PlayerInfo.characterMission == EMissionType.GET_TO_STATION)
        {
            GameObject.FindGameObjectWithTag("BG").SetActive(false);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(0).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(1).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(2).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(3).gameObject.SetActive(false);
            //GameObject.FindGameObjectWithTag("Menubutton").transform.Translate(200, 0, 0);
            UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_START, currSelectedPlayer.PlayerInfo.characterMission);
            // Delay then display character level instructions
            StartCoroutine(DelayThenStartLevel());
        }
        else{
           GameObject.FindGameObjectWithTag("sidebar").gameObject.transform.GetChild(0).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("sidebar").gameObject.transform.GetChild(1).gameObject.SetActive(true);
           // GameObject.FindGameObjectWithTag("Menubutton").transform.Rotate(0, 0, 0);
           GameObject.FindGameObjectWithTag("Menubutton").GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 138);
           //GameObject.FindGameObjectWithTag("Menubutton").GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 142);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(0).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(1).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(2).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(3).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag("star").SetActive(false);
            Debug.Log("name");
            UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_START, currSelectedPlayer.PlayerInfo.characterMission);
            // Delay then display character level instructions
            StartCoroutine(DelayThenStartLevel());
           // GameObject.FindGameObjectWithTag("Button1").SetActive(false);


        }

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
    
    private void EndLevel()
    {
        UIManager.Instance.ShowLevelStatusText();
        if(currentLevel == 1) //check for level 1
            {
            Debug.Log("Level 1 complete");
        UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_COMPLETE, currSelectedPlayer.PlayerInfo.characterMission);
            StartCoroutine(DisplayEndLevelDelay());
            }
        else{
            Debug.Log("Final level");
            UIManager.Instance.DisplayLevelStatusMessage(EGameState.QUEST_COMPLETE, currSelectedPlayer.PlayerInfo.characterMission);
            UIManager.Instance.FinalLevel(); // this method loads the MainMenu after completing level2
        }     
    }

    private IEnumerator DisplayEndLevelDelay()
    {
        yield return new WaitForSeconds(LevelEndDelay);
        Destroy(currSelectedPlayer);
        Destroy(GameObject.FindWithTag("Player"));
        //UIManager.Instance.HideLevelStatusText();
        SpawnManager.Instance.StartSpawn(ESpawnSelection.PLAYERS);
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

    public void StartMission(EMissionType mission)
    {
        // if playing 2nd part of Level 1, shift index to further along in instructions array
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
        info.IsSelected = isSelected;

        if (isSelected && !selectedRules.Contains(info))
        {
            selectedRules.Add(info);
            Debug.Log(" XXX VALUE "+RulesScript.but);
            tagVal = RulesScript.bookTag;
            Debug.Log("TAGVAL: " + tagVal);
            // Destroy(GameObject.FindGameObjectWithTag(tagVal));
            GameObject.FindGameObjectWithTag(tagVal).SetActive(false);
            Debug.Log("Destroyed " + tagVal);
            GameObject.FindGameObjectWithTag("Button"+ RulesScript.but).GetComponent<Image>().sprite = currSelectedPlayer.PlayerInfo.collectibleImage;
            RulesScript.array.Add(tagVal+"1");
        }
        else
        {
            selectedRules.Remove(info);
            Debug.Log(tagVal);
             for (int j = 0; j < GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.childCount ; j++)
             {
                 if (GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.GetChild(j).gameObject.tag == tagVal)
                 {
                    Debug.Log("INSIDE");
                    // GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.GetChild(j).gameObject.SetActive(true);
                    index = j;
                    lasttag = tagVal;
                 }
             }
            GameObject.FindGameObjectWithTag("Button"+RulesScript.but).GetComponent<Image>().sprite = currSelectedPlayer.PlayerInfo.backImage;
            GameObject.FindGameObjectWithTag("playerobjectspawn").gameObject.transform.GetChild(index).gameObject.SetActive(true);
            Debug.Log("_________________________fdxgnfn");
            Debug.Log("YYY - VALUE " + RulesScript.but);
        }
        Debug.Log("LAST TAG NAME"+ tagVal);
        UIManager.Instance.UpdateRules(selectedRules.Count);
       // GameObject.FindGameObjectWithTag("Button1").GetComponent<Image>().sprite = currSelectedPlayer.PlayerInfo.backImage;
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
                UIManager.Instance.UpdateScore(score);
                break;

            case EScoreEvent.ON_ROAD:
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

