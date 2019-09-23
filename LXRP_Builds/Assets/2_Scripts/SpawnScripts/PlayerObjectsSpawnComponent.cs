using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsSpawnComponent : MonoBehaviour
{
    private SO_RuleInfo[] rules = null;
    [SerializeField] GameObject rulesPrefab = null;
    [SerializeField] Transform[] spawnLocations = null;

    private GameObject[] playerCharacters;
    private int nextPlayer;
    
    private void Awake()
    {
        rules = Resources.LoadAll<SO_RuleInfo>("RULES");
        playerCharacters = Resources.LoadAll<GameObject>("PLAYERS");

        nextPlayer = 0;
    }

    public void SpawnRules()
    {
        for (int i = 0; i < rules.Length; i++)
        {
            GameObject spawn = Instantiate(rulesPrefab, GetLocation(), Quaternion.identity, transform);
            spawn.GetComponent<RulesScript>().RuleInfo = rules[i];
        }       
    }

    private Vector3 GetLocation()
    {
        int pos = Random.Range(0, spawnLocations.Length);
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
        Debug.LogWarning("Intantiating Player");
        StartCoroutine(PlayerSpawn());
    }

    IEnumerator PlayerSpawn()
    {
        yield return new WaitForSeconds(4.0f);
        GameObject spawn = Instantiate(playerCharacters[nextPlayer]);
        nextPlayer++;

        MainManager.Instance.SetState(GAMESTATE.PLAYER_START);
    }
}
