using UnityEngine;

public class ArrowProjectile : BaseProjectile
{
    public float maxRange = 10f;
    public float speed = 5f;

    public Vector2 velocity;
    Vector3 initialPosition;

    void Start() {
        initialPosition = transform.position;
    }

    void FixedUpdate() {
        transform.Translate(velocity * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, initialPosition) >= maxRange) Destroy(gameObject);
    }

    protected override void OnHit(Enemy enemy) {
        base.OnHit(enemy);
        Destroy(gameObject);
    }
}
