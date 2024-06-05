using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Damageable
{
    //Permanent immunity can't be changed overtime. Use a const to determine this property.
    public bool PermanentlyImmuneToDeathZone { get; }
    public void TakeDamage(float damageAmount);
}
