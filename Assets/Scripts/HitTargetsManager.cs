using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTargetsManager : MonoBehaviour
{
    Dictionary<I_DamageSource, List<I_Damageable>> hitTargets;
    private void Awake()
    {
        hitTargets = new Dictionary<I_DamageSource, List<I_Damageable>>();
    }
    private void OnEnable()
    {
        EventsManager.sourceHasAlreadyHitDamageable += OnSourceHasAlreadyHitDamageable;
        EventsManager.resetAlreadyHitTargets += OnResetAlreadyHitTargets;
    }
    private void OnDisable()
    {
        EventsManager.sourceHasAlreadyHitDamageable -= OnSourceHasAlreadyHitDamageable;
        EventsManager.resetAlreadyHitTargets -= OnResetAlreadyHitTargets;
    }
    private void OnResetAlreadyHitTargets(I_DamageSource source)
    {
        hitTargets.Remove(source);
    }
    private bool OnSourceHasAlreadyHitDamageable(I_DamageSource source, I_Damageable damageable)
    {
        if(!hitTargets.TryGetValue(source, out List<I_Damageable> damageableList))
        {
            hitTargets[source] = new List<I_Damageable> { damageable };
            return false;
        }
        
        if(damageableList.Contains(damageable))
            return true;

        damageableList.Add(damageable);
        return false;
    }
}
