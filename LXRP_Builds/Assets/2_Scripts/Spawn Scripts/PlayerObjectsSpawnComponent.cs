using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsSpawnComponent : MonoBehaviour
{
    private SO_RuleInfo[] rules = null;
    [SerializeField] GameObject rulesPrefab = null;
    [SerializeField] Transform[] spawnLocations = null;

    private int amtOfRules;
    public int AmtOfRules { get => amtOfRules; }

    private int amtOfCorrectRules;
    public int AmtOfCorrectRules { get => amtOfCorrectRules; }

    private int amtOfPlayers = 0;
    public int AmtOfPlayers { get => amtOfPlayers; }

    private GameObject[] playerCharacters;
    private int nextPlayer;

    
    private void Awake()
    {
        rules = Resources.LoadAll<SO_RuleInfo>("RULES");
        amtOfRules = rules.Length;

        playerCharacters = Resources.LoadAll<GameObject>("PLAYERS");
        amtOfPlayers = playerCharacters.Length;

        Array.Sort(playerCharacters, delegate (GameObject a, GameObject b) { return b.GetComponent<ABPlayerScript>().PlayerInfo.missionIndex.CompareTo((int)a.GetComponent<ABPlayerScript>().PlayerInfo.missionIndex); });

        //foreach (GameObject player in playerCharacters)
        //{
        //    Debug.Log("Player: " + player);
        //    Debug.Log("Info: " + player.GetComponent<ABPlayerScript>().PlayerInfo); 
        //}

        nextPlayer = 0;
    }

    public void SpawnRules()
    {
        for (int i = 0; i < rules.Length; i++)
        {
            GameObject spawn = Instantiate(rulesPrefab, GetLocation(), Quaternion.identity, transform);
            spawn.GetComponent<RulesScript>().RuleInfo = rules[i];

            if (rules[i].isCorrect)
                amtOfCorrectRules++;
        }       
    }

    private Vector3 GetLocation()
    {
        int pos = UnityEngine.Random.Range(0, spawnLocations.Length);
        if (spawnLocations[pos] != null)
        {
            Vector3 position = spawnLocations[pos].position;
            spawnLocations[pos] = null;
            return position;
        }
        else
            return GetLocation();
    }

    public void SpawnPlayer()
    {
        StartCoroutine(PlayerSpawn());
    }

    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(0.0f);
        //Debug.Log("Instantiating Player");

        GameObject spawn = Instantiate(playerCharacters[nextPlayer]);
        spawn.GetComponent<PathCreation.PathFollower>().pathCreator = SpawnManager.Instance.GetRandomPedestrianPath();
        spawn.GetComponent<ABPlayerScript>().SwitchComponents(false);

        nextPlayer++;
 
        MainManager.Instance.SetState(EGameState.PLAYER_START, spawn.GetComponent<ABPlayerScript>());
    }
}
