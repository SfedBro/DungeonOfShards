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
        if (weapons.Count > 0) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) weaponInd = (weaponInd - 1) % weapons.Count;
            if (Input.GetKeyDown(KeyCode.Alpha2)) weaponInd = (weaponInd + 1) % weapons.Count;
        }
        bool needToShoot = Input.GetButton("Fire1");
        if (needToShoot) Shoot();
    }

    void Shoot() {
        if (weaponInd == -1) return;
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        weapons[weaponInd].ShootWeapon(transform, dir);
    }

    public BaseWeapon GetCurrentWeapon() {
        return weaponInd == -1 ? null : weapons[weaponInd];
    }
}
