using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceGem : Pickup
{
    public int experienceGranted;

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
        player.IncreaseExperience(experienceGranted);
    }
}
