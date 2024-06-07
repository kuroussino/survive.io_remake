using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Damageable
{
    //Permanent immunity can't be changed overtime. Use a const to determine this property.
    public bool PermanentlyImmuneToDeathZone { get; }
    public DamageResponseInfo TakeDamage(DamageQueryInfo info);
}

public struct DamageQueryInfo
{
    public I_DamageOwner owner;
    public I_DamageSource source;
    public float damageAmount;
}
public struct DamageResponseInfo
{
    public bool attackAbsorbed;
}