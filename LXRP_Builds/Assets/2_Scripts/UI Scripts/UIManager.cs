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
        smallMenu = GameObject.FindGameObjectWithTag("menu");
        smallMenu.SetActive(false);
        
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

    private void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnBackToMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    public void DisplayLevelStatusMessage(EGameState inLevelState, EMissionType inMission)
    {
        //Debug.Log("Level Status BEFORE text: " + LevelStatus.text);
        if (!levelTextSet)
        {
            LevelStatus = getTextElement("LevelMessage");
            levelTextSet = true;
        }
        //LevelStatus = getTextElement("level");
        //Debug.Log("Level Status AFTER text: " + LevelStatus.text);
        int level = (int)inMission;

        if (inLevelState == EGameState.QUEST_START)
        {
            LevelStatus.SetText("Level " + ++level);
        }
        else if (inLevelState == EGameState.QUEST_COMPLETE)
        {
            LevelStatus.SetText("Level " + ++level + " Completed");
        }
    }

    public TextMeshProUGUI getTextElement(string inLabel)
    {
        TextMeshProUGUI result = null;
        /*
        foreach (TextMeshProUGUI t in TextElements)
        {
            if (t.tag == inLabel)
            {
                result = t;
                break;
            }
        }*/
        //result = 
          //  GameObject.FindGameObjectWithTag(inLabel);
        GameObject obj = GameObject.FindGameObjectWithTag(inLabel);
        result = obj.GetComponent<TextMeshProUGUI>();
        Debug.Log("RESULT: " + result);
        return result;
    }

    public void InitGameOverMessage()
    {
        GameOver = getTextElement("GameOver");
        GameOver.gameObject.SetActive(false);
    }
    
    public void DisplayGameOverMessage()
    {
        GameOver.text = "Game Over!";
        GameOver.gameObject.SetActive(true);
        StartCoroutine(DoGameOverDelay());
    }

    private IEnumerator DoGameOverDelay()
    {
        yield return new WaitForSeconds(3.0f);
        smallMenu.SetActive(true);
        GameOver.gameObject.SetActive(false);
    }


    public void HideLevelStatusText()
    {
        LevelStatus.gameObject.SetActive(false);
        LevelStatus.text = "level";
    }
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

        UpdateRules(0);
    }

    public void StartIntroSpeech(string[] speechText, Sprite characterPortrait, int startIndex)
    {
        speechTextComponent.StartIntroSpeech(speechText, characterPortrait, startIndex);
    }

    public void UpdateRules(int no)
    {
        string text = no + " / "
            + SpawnManager.Instance.getNoOfSpawns(ESpawnSelection.RULES)
            + " Rules Selected";
        updateText.text = text;
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
