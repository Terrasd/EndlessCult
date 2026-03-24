using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("OBSOLETE")]
public class GarlicBehaviour : MeleeWeaponBehaviour
{
    private List<GameObject> markedEnemies;

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !markedEnemies.Contains(collision.gameObject))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);

            markedEnemies.Add(collision.gameObject); // Mark the enemy so it doesn't take another instance of damage
        }
        else if (collision.CompareTag("Prop"))
        {
            if (collision.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(collision.gameObject))
            {
                breakable.TakeDamage(GetCurrentDamage());

                markedEnemies.Add(collision.gameObject);
            }
        }
    }
}
