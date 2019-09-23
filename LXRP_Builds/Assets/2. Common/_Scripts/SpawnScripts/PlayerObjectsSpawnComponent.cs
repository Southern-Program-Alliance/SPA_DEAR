using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsSpawnComponent : MonoBehaviour
{
    [SerializeField] SO_RuleInfo[] rules = null;
    [SerializeField] GameObject rulesPrefab = null;

    [SerializeField] Transform[] spawnLocations = null;

    // Start is called before the first frame update
    void Start()
    {
        rules = Resources.LoadAll<SO_RuleInfo>("RULES");

        SpawnRules();
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
}
