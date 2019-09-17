using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    [SerializeField] Button resetButton = null;
    [SerializeField] Button backToMenuButton = null;

    private void Start()
    {
        if (!CheckMissingRefs())
            return;

        resetButton.GetComponent<Button>().onClick.AddListener(OnResetButtonClicked);
        backToMenuButton.GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClicked);
    }

    private void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnBackToMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
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

    private void OnDestroy()
    {
        if(resetButton != null)
            resetButton.GetComponent<Button>().onClick.RemoveListener(OnResetButtonClicked);

        if(backToMenuButton!=null)
            backToMenuButton.GetComponent<Button>().onClick.RemoveListener(OnBackToMenuButtonClicked);
    }
}
