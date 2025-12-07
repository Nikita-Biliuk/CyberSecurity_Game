using UnityEngine;

/// <summary>
/// Base class for weapons like mines
/// </summary>
public abstract class LandWeaponBehaviour : MonoBehaviour
{
    protected float damage;
    protected float radius;
    protected LayerMask enemyLayer;
    protected GameObject explosionEffectPrefab;

    /// <summary>
    /// Initialization of land weapons
    /// </summary>
    public virtual void Setup(WeaponScriptableObject weaponData, LayerMask enemyLayer, GameObject explosionEffect)
    {
        damage = weaponData.Damage * FindObjectOfType<PlayerStats>().CurrentMight;
        radius = weaponData.Speed; // Using speed as radius
        this.enemyLayer = enemyLayer;
        this.explosionEffectPrefab = explosionEffect;
    }

    /// <summary>
    /// Explosion
    /// </summary>
    public virtual void Detonate()
    {
        if (explosionEffectPrefab != null)
        {
            var explosion = Instantiate(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
            explosion.transform.SetParent(null);
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (Collider2D enemyCol in enemies)
        {
            if (enemyCol.TryGetComponent(out EnemyStats enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Weapon radius visualization in editor
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
