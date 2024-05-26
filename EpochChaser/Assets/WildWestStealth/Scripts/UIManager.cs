using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text currentWeaponText;
    public GameObject projectileManager;
    ProjectileManager specialAbility;
    string currentWeapon;
    // Start is called before the first frame update
    void Start()
    {
        specialAbility = projectileManager.GetComponent<ProjectileManager>();
        currentWeapon = specialAbility.specialProjectilePrefab.name;
        currentWeaponText.SetText("Current Weapon: " + specialAbility.specialProjectilePrefab.name);
    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = specialAbility.specialProjectilePrefab.name;
        currentWeaponText.SetText("Current Weapon: " + currentWeapon);

    }
}
