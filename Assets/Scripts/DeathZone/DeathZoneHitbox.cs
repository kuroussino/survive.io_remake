using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneHitbox : MonoBehaviour
{
    public Action<I_Damageable, bool> damageableTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        I_Damageable damageable = collision.gameObject.GetComponent<I_Damageable>();
        if(damageable != null)
            damageableTrigger?.Invoke(damageable, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        I_Damageable damageable = collision.gameObject.GetComponent<I_Damageable>();
        if (damageable != null)
            damageableTrigger?.Invoke(damageable, false);
    }
}
