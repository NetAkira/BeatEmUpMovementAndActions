using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AngelineAnim : MonoBehaviour
{
    [SerializeField] private Angeline thisAngeline;
    public UnityEvent endAttack;
    public void endThisAttack()
    {
        endAttack.Invoke();
    }

    public void Instantiate_FX(GameObject newObj)
    {
        thisAngeline.instantiateSFX(newObj,10f, transform.position);
    }

    public void castedSkill()
    {
        thisAngeline.castedSkill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Mibman")thisAngeline.hackformHit(1);
        if(other.tag == "Cthulhu")thisAngeline.hackformHit(2);
        if(other.tag == "Tetsuo") thisAngeline.hackformHit(3);
        
    }

}
