using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, ICollectible
{
    protected bool hasBeenCollected = false; //check if the pickup has been collected
    public virtual void Collect()
    {
        hasBeenCollected = true; //set the pickup as collected
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))    //When it hits the player it disapears
        {
            Destroy(gameObject);
        }
    }
}
