using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WeaponController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 10f;

    Rigidbody2D rb;
    WeaponController weaponController;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        weaponController = GetComponent<WeaponController>();
    }

    void FixedUpdate() {
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 desiredMovement = input.normalized * speed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredMovement, acceleration * Time.fixedDeltaTime);
        BaseWeapon weapon = weaponController.GetCurrentWeapon();
        if (weapon != null) weapon.ShardsForEach(shard => shard.OnPlayerMove(desiredMovement));
    }
}
