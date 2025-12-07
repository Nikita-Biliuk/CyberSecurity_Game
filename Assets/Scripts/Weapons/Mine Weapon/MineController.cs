using System.Collections.Generic;
using UnityEngine;

public class MineController : WeaponController
{
    private List<GameObject> activeMines = new List<GameObject>();
    public int maxMines = 5;

    [Header("Mine Settings")]
    public LayerMask enemyLayer;
    public GameObject explosionEffectPrefab;

    protected override void Attack()
    {
        base.Attack();

        // if we reached max mines, detonate the oldest one
        if (activeMines.Count >= maxMines)
        {
            if (activeMines[0] != null)
            {
                var oldMine = activeMines[0].GetComponent<MineBehaviour>();
                oldMine?.Detonate();
            }
            // Delete the reference to the old mine
        }

        // Create and setup new mine
        GameObject mine = Instantiate(weaponData.Prefab, transform.position, Quaternion.identity);
        var mineBehaviour = mine.GetComponent<MineBehaviour>();
        mineBehaviour.Setup(weaponData, this, enemyLayer, explosionEffectPrefab);
        activeMines.Add(mine);
    }

    public void RemoveMine(GameObject mine)
    {
        activeMines.Remove(mine);
    }
}
