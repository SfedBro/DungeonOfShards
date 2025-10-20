using UnityEngine;

public class Hammer : CloseCombatWeapon
{
    protected override void MeleeAttack(Collider2D[] targets, int targetsCount)
    {
        for (int i = 0; i < targetsCount; i++)
        {
            var target = targets[i];
            if (!target)
                continue;
            var enemyScript = target.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Debug.Log($"{name} поразил {target.name} на {damage} урона!");
            }
        }
    }
}
