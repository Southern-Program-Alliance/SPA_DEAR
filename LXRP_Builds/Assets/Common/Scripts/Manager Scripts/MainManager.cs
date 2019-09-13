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

    // Main selected character
    GameObject currentPlayerCharacter;
    public GameObject CurrentPlayerCharacter { get => currentPlayerCharacter; set => currentPlayerCharacter = value; }

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

        // Listen to player script
        PlayerScript.onGameEvent += GameEvent;
    }

    GameState managerState;

    [SerializeField] PlayerScript[] _arrayPlayers = null;

    [SerializeField] GameObject _uiLose = null;
    [SerializeField] Text _uiText = null;

    [SerializeField] int win = 3;

    // Function to change the state
    void SetState(GameState newState)
    {
        if (managerState == newState)
        {
            return;
        }

        managerState = newState;
        Debug.Log(managerState);
        HandleStateChangedEvent(managerState);
    }

    // Function to handle state changes
    void HandleStateChangedEvent(GameState state)
    {
        
    }


    private void Start()
    {
        StartCoroutine(PlayerSpawn());
        _uiLose.SetActive(false);
    }

    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));

        foreach (PlayerScript player in _arrayPlayers)
        {
            yield return new WaitForSeconds(Random.Range(10, 15));
            player.gameObject.SetActive(true);
        }
    }

    void GameEvent(int i)
    {
        if(i == 0)
        {
            _uiLose.SetActive(true);
            _uiText.text = "Oh no you disrupted the traffic, try again";
        }

        if(i == 1)
        {
            win++;
            if(win == 3)
            {
                _uiLose.SetActive(true);
                _uiText.text = "Yay, well done you got everyone to school";
            }
            
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene
            (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

}

