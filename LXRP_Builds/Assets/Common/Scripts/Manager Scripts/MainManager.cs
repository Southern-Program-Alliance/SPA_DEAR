using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class MainManager : MonoBehaviour
{
    public Camera MainCam;
    //public ARRaycastManager arRaycast;

    private GameObject _GoSelected;

    [SerializeField] PlayerScript[] _arrayPlayers = null;

    [SerializeField] GameObject _uiLose = null;
    [SerializeField] Text _uiText = null;

    [SerializeField] int win = 3;

    // Update is called once per frame
    void Update()
    {
        if (!Application.isEditor)
        {
            // Listen for input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Raycast
                var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Ray ray = MainCam.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 200, 1 << 10))
                {
                    if (hit.transform.tag == "Player")
                    {
                        Debug.Log("Player Character Object Clicked");

                        SelectNewPlayer(hit.collider.gameObject);
                    }

                    if (hit.transform.tag == "TrafficLight")
                    {
                        Debug.Log("Traffic Light Object Clicked");

                        SelectTrafficLight(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponentInParent<TrafficLightScript>().OnTrafficButtonPress();
                    }
                }
            }
        }
        else
        {
            // Listen for input
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast
                var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 200, 1 << 10))
                {
                    if (hit.transform.tag == "Player")
                    {
                        Debug.Log("Player Character Object Clicked");

                        SelectNewPlayer(hit.collider.gameObject);
                    }

                    if (hit.transform.tag == "TrafficLight")
                    {
                        Debug.Log("Traffic Light Object Clicked");

                        SelectTrafficLight(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponentInParent<TrafficLightScript>().OnTrafficButtonPress();
                    }
                }
            }
        }
        
    }

    private void SelectNewPlayer(GameObject newPlayer)
    {
        if (_GoSelected == newPlayer)
            return;

        if (_GoSelected != null)
            _GoSelected.GetComponent<PlayerScript>().SwitchComponents(false);

        _GoSelected = newPlayer;
        _GoSelected.GetComponent<PlayerScript>().SwitchComponents(true);
    }

    private void SelectTrafficLight(GameObject newLight)
    {

    }

    private void Start()
    {
        StartCoroutine(PlayerSpawn());
        _uiLose.SetActive(false);
    }

    private void Awake()
    {
        PlayerScript.onGameEvent += GameEvent;
    }

    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(Random.Range(10, 40));

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

