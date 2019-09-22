using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectsSpawnComponent : MonoBehaviour
{
    [SerializeField] SO_Rules[] rules;
    [SerializeField] GameObject rulesPrefab;

    [SerializeField] Transform[] spawnLocations;

    // Start is called before the first frame update
    void Start()
    {
        rules = Resources.LoadAll<SO_Rules>("RULES");

        SpawnRules();
    }

    public void SpawnRules()
    {
        Vector3[] previouslySpawnned = new Vector3[rules.Length];
        for (int i = 0; i < rules.Length; i++)
        {
            GameObject spawn = Instantiate(rulesPrefab, GetLocation(spawnLocations), Quaternion.identity, transform);
            spawn.GetComponent<RulesScript>().RuleText = rules[i].ruleText;
        }       
    }

    private Vector3 GetLocation(Transform[] array)
    {
        Vector3 position = array[Random.Range(0, spawnLocations.Length)].position;
        return position;
    }
}
