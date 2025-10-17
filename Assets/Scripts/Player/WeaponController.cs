using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<BaseWeapon> weapons = new();

    [SerializeField] int weaponInd;

    void Start() {
        weapons.AddRange(GetComponentsInChildren<BaseWeapon>());
        weaponInd = weapons.Count > 0 ? 0 : -1;
    }

    void Update() {
        bool needToShoot = Input.GetButton("Fire1");
        if (needToShoot) Shoot();
    }

    void Shoot() {
        if (weaponInd == -1) return;
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        weapons[weaponInd].ShootWeapon(transform, dir);
    }
}
