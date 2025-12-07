using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base script of all projectile behaviours [To be placed on prefab of a weapon that is a projectile]
/// </summary>

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    protected Vector3 direction;
    public float destroyAfterSeconds;

    //Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight; //Get the current damage and multiply it by the player's might
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
        
        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx < 0 && diry == 0)   //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if(dirx == 0 && diry < 0)  //down
        {
            scale.y = scale.y * -1;
        }
        else if(dirx == 0 && diry > 0)  //up
        {
            scale.x = scale.x * -1;
        }
        else if(dirx > 0 && diry > 0)  //right up
        {
            rotation.z = 0f;
        }
        else if(dirx > 0 && diry < 0)  //right down
        {
            rotation.z = -90f;
        }
        else if(dirx < 0 && diry > 0)  //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if(dirx < 0 && diry < 0)  //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);    //Cant simply set the vector because cannot convert
    }

    public void DirectionCheckerAlt(Vector3 dir)
{
    direction = dir;

    float dirx = direction.x;
    float diry = direction.y;

    Vector3 scale = transform.localScale;
    Vector3 rotation = Vector3.zero;

    if (dirx == 0 && diry > 0) // up
    {
        rotation.z = 90f;
    }
    else if (dirx == 0 && diry < 0) // down
    {
        rotation.z = -90f;
    }
    else if (dirx > 0 && diry > 0) // right up
    {
        rotation.z = -135f;
    }
    else if (dirx > 0 && diry < 0) // right down
    {
        rotation.z = 135f;
    }
    else if (dirx < 0 && diry > 0) // left up
    {
        rotation.z = 135f;
    }
    else if (dirx < 0 && diry < 0) // left down
    {
        rotation.z = -135f;
    }

    transform.localScale = scale;
    transform.rotation = Quaternion.Euler(rotation);
}


    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        //Refference the script from the collided collider and deal damage using TakeDamage()
        if(col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage()); //Get the current damage and apply it to the enemy
            ReducePierce();
        }
        else if(col.CompareTag("Prop"))
        {
            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage()); //Get the current damage and apply it to the breakable prop
                ReducePierce();
            }
        }
    }

    void ReducePierce() //Destroy once the pierce reaches 0 
    {
        currentPierce--;
        if(currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
