using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float damage = 5f;
    public float maxRange = 10f;
    public float speed = 5f;

    public Vector2 dir;
    Vector3 initialPosition;

    void Start() {
        initialPosition = transform.position;
    }

    void FixedUpdate() {
        transform.Translate(dir * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, initialPosition) >= maxRange) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) return;
        if (collision.CompareTag("Enemy")) {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
