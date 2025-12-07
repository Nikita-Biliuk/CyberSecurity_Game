using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
 
    //Movement
    public float moveSpeed;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    //References
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);   //if we dont do this and game starts up and the player doesnt move the projectile weapon will have no momentum 
    }


    void Update()
    {
        InputManagment(); 
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagment()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    //Last moved x
        }
        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);    //Last moved y
        }
        if(moveDir.x != 0 && moveDir.y != 0)
    {
        lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);    //While moving
    }
    }

    void Move()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }
        
        rb.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
    
}
