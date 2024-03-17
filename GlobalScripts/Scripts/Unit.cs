using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
    [Header("Refs")]
    private Unit player;

    [Header("Unit Stats")]
    [SerializeField] private string name;

    [SerializeField] private float maxLifePoints;
    [SerializeField] private float maxEnergyPoints;
    [SerializeField] private float lifePoints;
    [SerializeField] private float energyPoints;

    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float baseDamage;

    [Header("Events")]
    public UnityEvent hasBeenHitted;

    [SerializeField] public StressReceiver cameraShaker;

    [Header("Effects")]
    public GameObject death_VFX_1;
    public GameObject hit_VFX_1;

    public GameObject Attack_SFX_1;
    public GameObject Attack_SFX_2;

    public GameObject ForceFieldEffect_1;
    public GameObject HollowPurple_1;
    public GameObject Kamehameha_1;
    public GameObject Laser_1;
    public GameObject Laser_2;

    private void Start()
    {
        SetReferences();
    }

    public void setLife()
    {
        lifePoints = maxLifePoints;
    }

    private void SetReferences()
    {
        lifePoints = maxLifePoints;
        player = GameObject.FindObjectOfType<Angeline>();
    }


    private void DealDamage(int damage,Unit target)
    {
        target.TakeDamage(damage);
    }
    public void TakeDamage(int damageToTake)
    {
        lifePoints -= damageToTake;
        Instantiate(hit_VFX_1, transform.position, transform.rotation);

        
    }



    // ----- // ----- // ----- // -----  SFX
    public void instantiateSFX(GameObject sfx, float timer, Vector3 point)
    {
        GameObject newSFX = Instantiate(sfx, point, new Quaternion(0, 0, 0, 1));
        Destroy(newSFX, timer);
    }

    public void heal(float amount)
    {
        lifePoints += amount;
        if (lifePoints > maxLifePoints) lifePoints = maxLifePoints;
    }

    // ----- // ----- // ----- // -----  GETTERS
    public float getLifePoint()
    {
        return this.lifePoints;
    }

    public float getEnergyPoints()
    {
        return this.energyPoints;
    }

    public float getMoveSpeed()
    {
        return this.moveSpeed;
    }
    public float getBaseDamage()
    {
        return this.baseDamage;
    }

    public Unit getPlayer()
    {
        return this.player;
    }

    public float getTotalLifePoints()
    {
        return this.maxLifePoints;
    }
}
