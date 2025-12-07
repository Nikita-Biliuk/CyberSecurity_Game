using UnityEngine;

public class MineBehaviour : LandWeaponBehaviour
{
    private MineController controller;

    public void Setup(WeaponScriptableObject weaponData, MineController mineController, LayerMask layer, GameObject effect)
    {
        controller = mineController;
        base.Setup(weaponData, layer, effect);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Detonate();
        }
    }

    public override void Detonate()
    {
        controller.RemoveMine(gameObject);
        base.Detonate();
    }
}
