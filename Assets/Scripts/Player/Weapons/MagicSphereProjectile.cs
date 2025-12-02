using UnityEngine;

public class MagicSphereProjectile : BaseProjectile
{
    public float maxSpeed = 1f;

    public Vector2 targetPosition;
    public Transform player;

    bool movingToTarget = true;

    void FixedUpdate() {
        Vector3 target = movingToTarget ? (Vector3)targetPosition : player.position;
        transform.position = Vector3.MoveTowards(transform.position, target, maxSpeed * Time.fixedDeltaTime);
        if (movingToTarget && Vector3.Distance(transform.position, (Vector3)targetPosition) < 0.01f) movingToTarget = false;
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        if (!movingToTarget && other.transform == player) {
            Destroy(gameObject);
        }
    }
}
