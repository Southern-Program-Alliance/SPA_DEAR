using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SpeechTextUI))]
public class UIManager : MonoBehaviour
{
    // Singleton Members
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

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

        speechTextComponent = GetComponent<SpeechTextUI>();
    }

    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] TextMeshProUGUI LevelStatus = null;
    [SerializeField] TextMeshProUGUI GameOver = null;
    TextMeshProUGUI[] TextElements = null;
    [SerializeField] GameObject LevelStatusObject = null;
    // Main selected character
    ABPlayerScript currSelectedPlayer;
    public ABPlayerScript CurrSelectedPlayer { get => currSelectedPlayer; }

    private Animator animatorScoreText = null;

    [SerializeField] Button resetButton = null;
    [SerializeField] Button backToMenuButton = null;

    // Character Info UI members
    [Space]
    [SerializeField] Text characterNameTxt = null;
    [SerializeField] Image portraitImage = null;
    [SerializeField] Image fullImage = null;
    [SerializeField] Image collectibleImage = null;
    [SerializeField] Text objectiveText = null;
    [SerializeField] Text updateText = null;

    // Rule UI Members
    [Space]
    [SerializeField] GameObject ruleBookUI = null;
    [SerializeField] Toggle ruleSelection = null;
    [SerializeField] Text ruleText = null;
    [SerializeField] Text ruleNo = null;
    private SO_RuleInfo ruleInfo = null;

    private SpeechTextUI speechTextComponent;
    private GameObject smallMenu = null;
    private bool levelTextSet = false;
 

    private void Start()
    {
        if (!CheckMissingRefs())
            return;

        animatorScoreText = scoreText.gameObject.GetComponent<Animator>();

        resetButton.onClick.AddListener(OnResetButtonClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);

        ruleSelection.onValueChanged.AddListener(OnRuleSelection);
        TextElements = FindObjectsOfType<TextMeshProUGUI>();
        //smallMenu = GameObject.FindGameObjectWithTag("menu");
        //smallMenu.SetActive(false);
        
    }

    private void OnRuleSelection(bool isSlected)
    {
        MainManager.Instance.OnRuleSelect(isSlected, ruleInfo);
    }

    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.GetComponent<Button>().onClick.RemoveListener(OnResetButtonClicked);

        if (backToMenuButton != null)
            backToMenuButton.GetComponent<Button>().onClick.RemoveListener(OnBackToMenuButtonClicked);

        if (ruleSelection != null)
            ruleSelection.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelection);
    }

    #region MainMenuUI Methods

    // Enact reset game actions when selected
    private void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Enact back to menu actions when selected
    private void OnBackToMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
<<<<<<< HEAD

    // Display intro/outro message when starting/ending a level
=======
    private int level;
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
    public void DisplayLevelStatusMessage(EGameState inLevelState, EMissionType inMission)
    {
        if (!levelTextSet)
        {
            LevelStatus = getTextElement("LevelMessage");
            levelTextSet = true;
        }
<<<<<<< HEAD
        int level = (int)inMission;
=======
        //LevelStatus = getTextElement("level");
        //Debug.Log("Level Status AFTER text: " + LevelStatus.text);
          level = (int)inMission;
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."

        if (inLevelState == EGameState.QUEST_START)
        {
            LevelStatus.SetText("Level " + ++level);
        }
        else if (inLevelState == EGameState.QUEST_COMPLETE)
        {
            LevelStatus.SetText("Level " + ++level + " Completed");
        }
    }

    // Obtain specified text element
    public TextMeshProUGUI getTextElement(string inLabel)
    {
        TextMeshProUGUI result = null;

        GameObject obj = GameObject.FindGameObjectWithTag(inLabel);
        result = obj.GetComponent<TextMeshProUGUI>();
        Debug.Log("RESULT: " + result);
        return result;
    }

    // Initialise "Game Over" message, set to invisible
    public void InitGameOverMessage()
    {
        GameOver = getTextElement("GameOver");
        GameOver.gameObject.SetActive(false);
    }
    
    // Display "Game Over" message for 3 seconds
    public void DisplayGameOverMessage()
    {
        GameOver.text = "Game Over!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

<<<<<<< HEAD
    // Display "Game Finished" for 3 seconds
    public void DisplayGameFinishedMessage()
    {
        HideLevelStatusText();
        GameOver.text = "Game Finished!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

    // Coroutine to switch "Game Over" message off and show end game menu
=======
    public void FinalLevel()
    {
        StartCoroutine(DoFinalLevelDelay());
    }

>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
    private IEnumerator DoGameOverDelay()
    {
        yield return new WaitForSeconds(3.0f);
        GameOver.gameObject.SetActive(false);// Hide the level complete msg after few seconds
        GameObject.FindGameObjectWithTag("MainCanvas").gameObject.transform.GetChild(4).gameObject.SetActive(true);
    }

    private IEnumerator DoFinalLevelDelay()
    {
        yield return new WaitForSeconds(5.0f);
        HideLevelStatusText(); // Hide the level2 complete msg after few seconds
        //smallMenu.SetActive(true);
        SceneManager.LoadScene("MainMenu"); // Load the MainMenu once the level2 is completed
    }

    // Initialise "Level" message, set to invisible
    public void HideLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(false);
       // LevelStatus.text = "level";
    }

<<<<<<< HEAD
    // Display "Level" message
=======

>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
    public void ShowLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(true);
    }

    public void SetRuleInfo(SO_RuleInfo info)
    {
        ruleInfo = info;
        ruleNo.text = "#" + ruleInfo.ruleNo;
        ruleText.text = ruleInfo.ruleText;
        ruleSelection.isOn = ruleInfo.IsSelected;
        ruleBookUI.SetActive(true);
    }

    public void SetPlayerInfo(SO_PlayerInfo info)
    {
        characterNameTxt.text = info.characterName;
        portraitImage.sprite = info.portraitImage;
        fullImage.sprite = info.fullImage;
        collectibleImage.sprite = info.collectibleImage;
        objectiveText.text = info.objectivesText;
<<<<<<< HEAD
       
=======
>>>>>>> parent of 7b3c38f... Revert "Increased size of hot dog and contruction player, placed books on top of buildings, updated the menu bar according to the levels,replaced the busstop with different prefab, placed star on top of bus stop, modified the players information according to the levels, made level2 hit the home menu after the game completion."
        UpdateRules(0);
    }

    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int startIndex)
    {
        speechTextComponent.StartIntroSpeech(speechText, characterPortrait, startIndex);
    }

    public void UpdateRules(int no)
    {
        if(MainManager.Instance.currentLevel == 0)
        {
            updateText.text = "Take me to bus stop safely";
        }
        else
        {
        string text = no + " / "
            + SpawnManager.Instance.getNoOfSpawns(ESpawnSelection.RULES)
            + " Rules Selected";
        updateText.text = text;
        }
    }

    public void UpdateScore(int score)
    {
        Debug.Log("UI Manager Update Score");
        scoreText.text = score.ToString();
        animatorScoreText.SetBool("isScoreEvent", true);
    }


    private bool CheckMissingRefs()
    {
        if (resetButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'resetButton'");
            return false;
        }
        if (backToMenuButton == null)
        {
            Debug.LogError("UIManager: Reference not set - 'backToMenuButton'");
            return false;
        }
        return true;
    }
}
