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
        Vector3[] previouslySpawnned = new Vector3[rules.Length];
        for (int i = 0; i < rules.Length; i++)
        {
            GameObject spawn = Instantiate(rulesPrefab, GetLocation(spawnLocations), Quaternion.identity, transform);
            spawn.GetComponent<RulesScript>().RuleInfo = rules[i];
        }       
    }

    private Vector3 GetLocation(Transform[] array)
    {
        Vector3 position = array[Random.Range(0, spawnLocations.Length)].position;
        return position;
    }
}
