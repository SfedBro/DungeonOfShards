using System;
using UnityEngine;

[Serializable]
public class BaseShard
{
    public virtual void Update() {}
    public virtual void OnPlayerMove(Vector2 desiredMovement) {}

    public virtual void OnHit(Enemy enemy) {}
    public virtual void OnWeaponShootProjectile(BaseProjectile projectile, Vector2 direction) {}


    public virtual float OnEvaluateDamage(float damage) { return damage; }
    public virtual float OnEvaluateCooldown(float cooldown) { return cooldown; }
    public virtual float OnEvaluateProjectileSpeed(float speed) { return speed; }
}
