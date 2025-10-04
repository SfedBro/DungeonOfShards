using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 10f;

    Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 desiredMovement = input.normalized * speed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredMovement, acceleration * Time.fixedDeltaTime);
    }
}
