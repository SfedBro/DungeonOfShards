using UnityEngine;

public class BowWeapon : BaseWeapon
{
    public ArrowProjectile arrowPrefab;
    public float offsetDist = 0.5f;
    public float cooldown = 1f;
    public int maxArrows = 5;

    float currentCooldown = 0;
    int currentArrows;

    void Start() {
        currentArrows = maxArrows;
    }

    void Update() {
        currentCooldown = Mathf.Max(currentCooldown - Time.deltaTime, 0);
    }

    public override void ShootWeapon(Transform player, Vector2 dir) {
        if (currentCooldown == 0 && currentArrows > 0) {
            currentCooldown = cooldown;
            --currentArrows;
            ArrowProjectile arrow = Instantiate(arrowPrefab, player.position + (Vector3)dir.normalized * offsetDist, Quaternion.identity);
            arrow.dir = dir.normalized * arrow.speed;
        }
    }
}
