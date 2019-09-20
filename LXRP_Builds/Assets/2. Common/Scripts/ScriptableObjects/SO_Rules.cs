using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RulesData", menuName = "ScriptableObjects/Rules", order = 1)]
public class SO_Rules : ScriptableObject
{
    public string ruleText;

    public bool isCorrect;

    public Transform spawnTransform;

    public Mesh mesh;
}
