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
            var healthScript = target.GetComponent<Enemy>();
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
                Debug.Log($"{name} поразил {target.name} на {damage} урона!");
            }
        }
    }
}
