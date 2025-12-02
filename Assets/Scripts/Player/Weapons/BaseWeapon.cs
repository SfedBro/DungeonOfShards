using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public List<BaseShard> shards;

    public abstract void ShootWeapon(Transform player, Vector2 dir);

    public void ShardsForEach(Action<BaseShard> doForEachShard) {
        shards.ForEach(doForEachShard);
    }

    protected virtual void Update() {
        ShardsForEach(shard => shard.Update());
    }

    public float EvaluateWeaponDamage(float baseDamage) {
        return shards.Aggregate(baseDamage, (acc, shard) => shard.OnEvaluateDamage(acc));
    }
    public float EvaluateWeaponCooldown(float baseCooldown) {
        return shards.Aggregate(baseCooldown, (acc, shard) => shard.OnEvaluateCooldown(acc));
    }
    public float EvaluateWeaponProjectileSpeed(float baseSpeed) {
        return shards.Aggregate(baseSpeed, (acc, shard) => shard.OnEvaluateProjectileSpeed(acc));
    }
}
