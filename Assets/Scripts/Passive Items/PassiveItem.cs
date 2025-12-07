using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyModifier()
    {
        //apply the boost value to the player stats
    }
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ApplyModifier(); //apply the modifier when the item is picked up
    }

}
