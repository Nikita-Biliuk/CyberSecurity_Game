using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Check if the other game object has the Icollectible component
        if(col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            //Pulling animation 
            //Gets the Rigidbody2d component on the item
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            //Vector2 pointing from the item to the player
            Vector2 forceDirection = (transform.position - col.transform.position).normalized;
            //Applies force to the item
            rb.AddForce(forceDirection * pullSpeed);

            //If it does, call the Collect method
            collectible.Collect();
        }
    }
}
