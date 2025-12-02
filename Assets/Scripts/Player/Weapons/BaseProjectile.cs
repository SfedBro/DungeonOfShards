using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BaseProjectile : MonoBehaviour
{
    public float damage = 5f;
    public UnityEvent<Enemy> OnHitEffects = new();
    public BaseWeapon weapon;

    protected readonly HashSet<Transform> hitEnemies = new();

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.transform)) {
            Enemy enemy = other.GetComponent<Enemy>();
            OnHit(enemy);
            hitEnemies.Add(enemy.transform);
        }
    }

    protected virtual void OnHit(Enemy enemy) {
        float newDamage = weapon.EvaluateWeaponDamage(damage);
        enemy.TakeDamage(newDamage);
        OnHitEffects.Invoke(enemy);
    }
}
