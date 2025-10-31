using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseProjectile : MonoBehaviour
{
    public float damage = 5f;
    public UnityEvent<Enemy> OnHitEffects = new();

    protected readonly HashSet<Transform> hitEnemies = new();

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.transform)) {
            Enemy enemy = other.GetComponent<Enemy>();
            OnHit(enemy);
            hitEnemies.Add(enemy.transform);
        }
    }

    protected virtual void OnHit(Enemy enemy) {
        enemy.TakeDamage(damage);
        OnHitEffects.Invoke(enemy);
    }
}
