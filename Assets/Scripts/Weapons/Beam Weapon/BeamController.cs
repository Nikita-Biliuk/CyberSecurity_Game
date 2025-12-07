using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : WeaponController
{


    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
       base.Attack();
       GameObject spawnedBeam = Instantiate(weaponData.Prefab);
       spawnedBeam.transform.position = transform.position; //Assign the position to be the same as this object which is parented to the player
       spawnedBeam.GetComponent<BeamBehaviour>().DirectionCheckerAlt(pm.lastMovedVector); //Reference and set direction
    }
}
