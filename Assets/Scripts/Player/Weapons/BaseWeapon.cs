using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public List<BaseShard> shards;

    public abstract void ShootWeapon(Transform player, Vector2 dir);

    protected void ShardsForEach(Action<BaseShard> doForEachShard) {
        shards.ForEach(doForEachShard);
    }

    protected virtual void Update() {
        ShardsForEach(shard => shard.Update());
    }
}
