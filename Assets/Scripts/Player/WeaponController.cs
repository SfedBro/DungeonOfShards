using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<BaseWeapon> weapons = new();

    [SerializeField] int weaponInd;

    void Start() {
        weaponInd = weapons.Count > 0 ? 0 : -1;
    }

    void Update() {
        print(weaponInd);
        bool needToShoot = Input.GetButton("Fire1");
        if (needToShoot) Shoot();
    }

    void Shoot() {
        print(Time.deltaTime);
        if (weaponInd == -1) return;
        weapons[weaponInd].ShootWeapon();
    }
}
