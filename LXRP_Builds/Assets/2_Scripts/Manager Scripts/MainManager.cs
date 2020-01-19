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
    const int NUM_RULEBOOKS = 5;
    const int FINAL_LEVEL = 2; // Need to set to '3' when Level 3 implemented

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

            // When the player completes the game
            case EGameState.PLAYER_COMPLETE:
                Destroy(currSelectedPlayer);
                UIManager.Instance.DisplayGameFinishedMessage();
                break;

            // When the player's score reaches zero -> GAME OVER
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
        SetState(EGameState.QUEST_START);

    }
    
    // Complete end of level actions; message display etc.
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

    // Coroutine to delay end of level message display and end of level actions
    private IEnumerator DisplayEndLevelDelay()
    {
        yield return new WaitForSeconds(LevelEndDelay);
        Destroy(currSelectedPlayer);
        Destroy(GameObject.FindWithTag("Player"));
        //UIManager.Instance.HideLevelStatusText();
        if (currentLevel == FINAL_LEVEL)
            MainManager.Instance.SetState(EGameState.PLAYER_COMPLETE);
        else
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
        float angle = 90.0f;

        GameObject ThePlayer = currSelectedPlayer.PlayerInfo.attachedObject;
        Vector3 pos = new Vector3(currSelectedPlayer.PlayerInfo.X, currSelectedPlayer.PlayerInfo.Y, currSelectedPlayer.PlayerInfo.Z);
        ThePlayer.transform.position = pos;
        Quaternion startAngle = Quaternion.Euler(0, angle, 0);
        ThePlayer.transform.rotation = startAngle;
    }

    // Enact specifics of a given level
    public void StartMission(EMissionType mission)
    {
        // Situate player in level
        PlacePlayer();

        // Move index further along instructions array for Level 1 Hotdog character
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
            case EMissionType.COLLECT_DONUTS:
                SpawnManager.Instance.StartSpawn(ESpawnSelection.DONUTS);
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
        info.IsSelected = isSelected;

        if (isSelected && !selectedRules.Contains(info))
        {
            selectedRules.Add(info);
            Debug.Log(" XXX VALUE "+RulesScript.but);
            tagVal = RulesScript.bookTag;
<<<<<<< HEAD
            //Debug.Log("TAGVAL: " + tagVal);
            Destroy(GameObject.FindGameObjectWithTag(tagVal));
            //Debug.Log("Destroyed " + tagVal);
=======
            Debug.Log("TAGVAL: " + tagVal);
            // Destroy(GameObject.FindGameObjectWithTag(tagVal));
            GameObject.FindGameObjectWithTag(tagVal).SetActive(false);
            Debug.Log("Destroyed " + tagVal);
            GameObject.FindGameObjectWithTag("Button"+ RulesScript.but).GetComponent<Image>().sprite = currSelectedPlayer.PlayerInfo.collectibleImage;
            RulesScript.array.Add(tagVal+"1");
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
        }
        else
        {
            selectedRules.Remove(info);
<<<<<<< HEAD
            //Debug.Log("_________________________fdxgnfn");
=======
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
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
        }
        Debug.Log("LAST TAG NAME"+ tagVal);
        UIManager.Instance.UpdateRules(selectedRules.Count);
<<<<<<< HEAD

        if (selectedRules.Count == NUM_RULEBOOKS)
=======
       // GameObject.FindGameObjectWithTag("Button1").GetComponent<Image>().sprite = currSelectedPlayer.PlayerInfo.backImage;
        if (selectedRules.Count == 5)
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
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

