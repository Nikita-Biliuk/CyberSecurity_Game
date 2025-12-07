using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamBehaviour : ProjectileWeaponBehaviour
{

    protected override void Start()
    {
        base.Start();
        currentSpeed *= FindObjectOfType<PlayerStats>().CurrentProjectileSpeed;
    }


    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; //Set the movement of the Beam 
    }
}
