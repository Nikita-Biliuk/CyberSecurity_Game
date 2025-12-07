using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Pickup
{
    public int healthToRestore;
    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return; // If already collected, do nothing
        }
        else
        {
            base.Collect(); // Call the base class Collect method
        }

        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(healthToRestore);
    }
}
