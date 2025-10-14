using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public abstract void ShootWeapon(Transform player, Vector2 dir);
}
