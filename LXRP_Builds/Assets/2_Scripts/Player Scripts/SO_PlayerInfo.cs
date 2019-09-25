using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo 1", menuName = "ScriptableObjects/CharacterInfo", order = 2)]
public class SO_PlayerInfo : ScriptableObject
{
    public MISSIONTYPE characterMission;
    public GameObject attachedObject;

    [Space]
    public Sprite portraitImage;
    public Sprite fullImage;
    public Sprite collectibleImage;

    [Space]
    public string characterName;
    [TextArea]
    public string objectivesText;
    [TextArea]
    public string[] introSpeechText;
}

