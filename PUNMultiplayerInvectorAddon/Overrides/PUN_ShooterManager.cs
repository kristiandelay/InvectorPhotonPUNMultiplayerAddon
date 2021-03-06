﻿using Invector.vShooter;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUN_ShooterManager : vShooterManager
{

    public override void ReloadWeapon(bool ignoreAmmo = false, bool ignoreAnim = false)
    {
        var weapon = rWeapon ? rWeapon : lWeapon;

        if (!weapon || !weapon.gameObject.activeInHierarchy) return;
        UpdateTotalAmmo();
        bool primaryWeaponAnim = false;
        if (!(!ignoreAmmo && (weapon.ammoCount >= weapon.clipSize || !WeaponHasAmmo())) && !weapon.autoReload)
        {
            if (!ignoreAnim)
                onReloadWeapon.Invoke(weapon);
            var needAmmo = weapon.clipSize - weapon.ammoCount;
            if (!ignoreAmmo && WeaponAmmo(weapon).count < needAmmo)
                needAmmo = WeaponAmmo(weapon).count;

            weapon.AddAmmo(needAmmo);
            if (!ignoreAmmo)
                WeaponAmmo(weapon).Use(needAmmo);
            if (GetComponent<Animator>() && !ignoreAnim)
            {
                GetComponent<Animator>().SetInteger("ReloadID", GetReloadID());
                GetComponent<PhotonView>().RPC("SetTrigger", RpcTarget.All, "Reload");
            }
            if (!ignoreAnim)
                weapon.ReloadEffect();
            primaryWeaponAnim = true;
        }
        if (weapon.secundaryWeapon && !((weapon.secundaryWeapon.ammoCount >= weapon.secundaryWeapon.clipSize || !WeaponHasAmmo(true))) && !weapon.secundaryWeapon.autoReload)
        {
            var needAmmo = weapon.secundaryWeapon.clipSize - weapon.secundaryWeapon.ammoCount;
            if (!ignoreAmmo && WeaponAmmo(weapon.secundaryWeapon).count < needAmmo)
                needAmmo = WeaponAmmo(weapon.secundaryWeapon).count;
            weapon.secundaryWeapon.AddAmmo(needAmmo);
            if (!ignoreAmmo)
                WeaponAmmo(weapon.secundaryWeapon).Use(needAmmo);
            if (!primaryWeaponAnim)
            {
                if (GetComponent<Animator>() && !ignoreAnim)
                {
                    primaryWeaponAnim = true;
                    GetComponent<Animator>().SetInteger("ReloadID", weapon.secundaryWeapon.reloadID);
                    GetComponent<PhotonView>().RPC("SetTrigger", RpcTarget.All, "Reload");
                }
                if (!ignoreAnim)
                    weapon.secundaryWeapon.ReloadEffect();
            }
        }

        UpdateTotalAmmo();
    }

    protected override IEnumerator Recoil(float horizontal, float up)
    {
        yield return new WaitForSeconds(0.02f);
        if (GetComponent<Animator>()) GetComponent<PhotonView>().RPC("SetTrigger", RpcTarget.All, "Shoot");
        if (tpCamera != null) tpCamera.RotateCamera(horizontal, up);
    }
}
