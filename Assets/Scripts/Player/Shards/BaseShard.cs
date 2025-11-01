using System;
using UnityEngine;

[Serializable]
public class BaseShard
{
    public virtual void Update() {}

    public virtual void OnHit(Enemy enemy) {}
    public virtual void OnWeaponShootProjectile(BaseProjectile projectile, Vector2 direction) {}

    public virtual void OnPlayerMove(Vector2 desiredMovement) {}
}
