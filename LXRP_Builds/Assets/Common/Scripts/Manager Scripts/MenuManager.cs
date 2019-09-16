using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button startGameButton = null;
    [SerializeField] Button exitButton = null;

    private void Start()
    {
        startGameButton.GetComponent<Button>().onClick.AddListener(OnStartGameButtonClicked);
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitButtonClicked);
    }

    private void OnStartGameButtonClicked()
    {
        SceneManager.LoadScene("MainLevel"); 
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        startGameButton.GetComponent<Button>().onClick.RemoveListener(OnStartGameButtonClicked);
        exitButton.GetComponent<Button>().onClick.RemoveListener(OnExitButtonClicked);
    }
}
