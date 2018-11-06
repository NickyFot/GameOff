using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats
{
    public int Damage { get; set; }
    public int Durability { get; set; }

    public WeaponStats(int damage, int durability)
    {
        Damage = damage;
        Durability = durability;
    }

    public static WeaponStats operator+ (WeaponStats weaponA, WeaponStats weaponB)
    {
        weaponA.Damage += weaponB.Damage;
        weaponA.Durability += weaponB.Durability;
        return weaponA;
    }
    public static WeaponStats operator- (WeaponStats weaponA, WeaponStats weaponB)
    {
        weaponA.Damage -= weaponB.Damage;
        weaponA.Durability -= weaponB.Durability;
        return weaponA;
    }
    public override string ToString()
    {
        return ("damage: " + Damage +"\n durability: " + Durability);
    }
}
