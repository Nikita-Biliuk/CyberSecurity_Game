using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base script for instant-effect weapons like lasers or melee weapons.
/// </summary>
public class InstantWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;

    protected float currentDamage;
    protected float currentCooldownDuration;

    protected virtual void Awake()
    {
        currentDamage = weaponData.Damage * FindObjectOfType<PlayerStats>().CurrentMight;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    protected virtual void Start()
    {
        ApplyEffect();
        Destroy(gameObject, destroyAfterSeconds);
    }

    /// <summary>
    /// Instantly applies damage in some area or direction.
    /// Override this in child classes.
    /// </summary>
    protected virtual void ApplyEffect()
    {
        // Example: do nothing by default, override in child
    }

    protected void DamageEnemy(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            if (col.TryGetComponent(out EnemyStats enemy))
            {
                enemy.TakeDamage(currentDamage);
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.TryGetComponent(out BreakableProps prop))
            {
                prop.TakeDamage(currentDamage);
            }
        }
    }
}
