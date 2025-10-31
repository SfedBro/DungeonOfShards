using UnityEngine;

public class MagicSphereWeapon : BaseWeapon
{
    public MagicSphereProjectile SphereProjectilePrefab;
    public float offsetDist = 0.5f;
    public float maxRange = 5f;

    MagicSphereProjectile sphereProjectileInstance;

    public override void ShootWeapon(Transform player, Vector2 dir) {
        if (sphereProjectileInstance == null) {
            sphereProjectileInstance = Instantiate(SphereProjectilePrefab, player.position + (Vector3)dir.normalized * offsetDist, Quaternion.identity);
            sphereProjectileInstance.targetPosition = (Vector2)player.position + Vector2.ClampMagnitude(dir, maxRange);
            sphereProjectileInstance.player = player;
            ShardsForEach(shard => sphereProjectileInstance.OnHitEffects.AddListener(shard.OnHit));
        }
    }
}
