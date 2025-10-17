using UnityEngine;

public class MagicSphereProjectile : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D other) {
        print("collision");
        if (!movingToTarget && other.transform == player) {
            Destroy(gameObject);
        }
    }
}
