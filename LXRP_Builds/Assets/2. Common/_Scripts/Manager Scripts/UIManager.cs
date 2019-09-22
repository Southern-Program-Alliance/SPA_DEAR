using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

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
    }

    [SerializeField] Button resetButton = null;
    [SerializeField] Button backToMenuButton = null;

    // Rule UI Members
    [SerializeField] GameObject ruleUI = null;
    [SerializeField] Toggle ruleSelection = null;
    [SerializeField] Text ruleText = null;
    [SerializeField] Text ruleNo = null;

    private void Start()
    {
        if (!CheckMissingRefs())
            return;

        resetButton.GetComponent<Button>().onClick.AddListener(OnResetButtonClicked);
        backToMenuButton.GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClicked);

        ruleSelection.GetComponent<Toggle>().onValueChanged.AddListener(OnRuleSelection);
    }
    
    private void OnRuleSelection(bool isSlected)
    {
        //if(isSlected)
    }

    public void SetRuleInfo(SO_RuleInfo info)
    {
        ruleNo.text = "#" + info.ruleNo;
        ruleText.text = info.ruleText;
        ruleUI.SetActive(true);
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

    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.GetComponent<Button>().onClick.RemoveListener(OnResetButtonClicked);

        if (backToMenuButton != null)
            backToMenuButton.GetComponent<Button>().onClick.RemoveListener(OnBackToMenuButtonClicked);

        if(ruleSelection != null)
            ruleSelection.GetComponent<Toggle>().onValueChanged.RemoveListener(OnRuleSelection);
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
