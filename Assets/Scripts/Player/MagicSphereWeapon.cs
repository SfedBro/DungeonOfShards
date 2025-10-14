using UnityEngine;

public class MagicSphereWeapon : BaseWeapon
{
    public MagicSphereProjectile SphereProjectilePrefab;
    public float offsetDist = 0.5f;

    MagicSphereProjectile sphereProjectileInstance;

    public override void ShootWeapon(Transform player, Vector2 dir) {
        if (!sphereProjectileInstance) {
            sphereProjectileInstance = Instantiate(SphereProjectilePrefab, player.position + (Vector3)dir.normalized * offsetDist, Quaternion.identity);
            
        }
    }
}
