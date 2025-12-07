using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveItemData.Multiplier / 100f; //apply the boost value to the player stats
    }
}
