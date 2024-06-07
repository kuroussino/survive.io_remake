using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnifiedDamageSource: I_DamageSource
{
    public I_DamageOwner owner {  get; private set; }
    public UnifiedDamageSource(I_DamageOwner owner)
    {
        this.owner = owner;
    }
}
