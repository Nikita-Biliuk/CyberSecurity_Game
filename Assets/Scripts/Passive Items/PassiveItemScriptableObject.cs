using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/PassiveItem")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier {get => multiplier; private set => multiplier = value;}

    [SerializeField]
    int level;          //not meant to be modified in the game only in editor
    public int Level { get => level;  private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;          //the prefab of the next level, what the object becomes when it levels up
    public GameObject NextLevelPrefab { get => nextLevelPrefab;  private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;          //the name of the passive item, used in the UI
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;          //the description of the weapassive item, used in the UI
    public string Description { get => description; private set => description = value; }

    [SerializeField]
    Sprite icon;      //the icon of the pasive item, used in the UI
    public Sprite Icon { get => icon; private set => icon = value; }
}
