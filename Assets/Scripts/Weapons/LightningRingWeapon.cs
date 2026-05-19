using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Damage does not scale with Might stat currently
public class LightningRingWeapon : ProjectileWeapon
{
    private List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning(string.Format("Hit effect prefab has not been set for {0}", name));
            ActivateCooldown();
            return false;
        }

        // If there is no projectile assigned, set the weapon on cooldown
        if (!CanAttack())
        {
            return false;
        }

        // If the cooldown is less than 0, this is the first firing of the weapon.
        // Refresh the array of selected enemies.
        if (currentCooldown <= 0)
        {
            allSelectedEnemies = new List<EnemyStats>(GetEnemiesOnScreen());
            ActivateCooldown();
            currentAttackCount = attackCount;
        }

        // Find an enemy in the map to strike with lightning
        EnemyStats target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, GetArea(), GetDamage());
            Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
        }

        // If there is a proc effect, play it on the player
        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        // If we have more than 1 attack count
        if (attackCount > 0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }

    // Randomly picks an enemy on screen
    private EnemyStats PickEnemy()
    {
        EnemyStats target = null;
        while (!target && allSelectedEnemies.Count > 0)
        {
            int index = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[index];

            // If the target is already dead, remove it and skip it
            if (!target)
            {
                allSelectedEnemies.RemoveAt(index);
                continue;
            }

            allSelectedEnemies.RemoveAt(index);
            return target;
        }

        return null;
    }

    // Deals damage in an area
    private void DamageArea(Vector2 position, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D t in targets)
        {
            EnemyStats es = t.GetComponent<EnemyStats>();
            if (es)
            {
                es.TakeDamage(damage, transform.position);
            }
        }
    }

    private EnemyStats[] GetEnemiesOnScreen()
    {
        Camera cam = Camera.main;

        // Convert screen edges to world space
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Get the center and size in world units
        Vector3 center = (bottomLeft + topRight) / 2f;
        Vector3 size = new Vector3(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y, 0);

        // OverlapBox call to get colliders
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f, LayerMask.GetMask("Enemy"));

        // Filter for GameObjects that have the EnemyStats component
        return colliders
            .Select(c => c.GetComponent<EnemyStats>())  // Get EnemyStats component
            .Where(es => es != null)                    // Ensure it's not null
            .ToArray();                                 // Convert to array
    }
}
