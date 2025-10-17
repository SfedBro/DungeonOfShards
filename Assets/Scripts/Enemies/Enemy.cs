using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp = 100f;

    public void TakeDamage(float damage) {
        hp = Mathf.Max(hp - damage, 0);
        if (hp == 0) Die();
    }

    void Die() {
        Destroy(gameObject);
    }
}
