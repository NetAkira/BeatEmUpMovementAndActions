using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private Unit rootUnit;

    [SerializeField] private GameObject hitSound;

    public Unit getUnit()
    {
        return this.rootUnit;
    }

    public int getDamage()
    {
        return this.damage;
    }
    public GameObject getHitSound()
    {
        return this.hitSound;
    }
}
