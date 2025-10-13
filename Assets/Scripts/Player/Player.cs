using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHp = 100f;

    public float hp;

    void Start() {
        hp = maxHp;
    }

    public void TakeDamage(float damage) {
        hp = Mathf.Max(hp - damage, 0);
    }
}
