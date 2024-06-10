using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPunch : NetworkBehaviour
{
    UnifiedDamageSource unifiedDamageSource;
    bool isPunching;
    Collider2D cachedCollider;
    private void Awake()
    {
        cachedCollider = GetComponent<Collider2D>();
        cachedCollider.enabled = false;
    }
    public void Punch(UnifiedDamageSource unifiedDamageSource)
    {
        this.unifiedDamageSource = unifiedDamageSource;
        StartCoroutine(PunchCoroutine());
    }
    IEnumerator PunchCoroutine()
    {
        if (isPunching)
            yield break;

        EventsManager.resetAlreadyHitTargets?.Invoke(unifiedDamageSource);

        isPunching = true;
        cachedCollider.enabled = true;
        transform.localScale = Vector3.one * 1.5f;
        EditHandsScaleClientRpc(Vector3.one * 1.5f);
        yield return new WaitForSeconds(3);

        isPunching = false;
        cachedCollider.enabled = false;
        transform.localScale = Vector3.one;
        EditHandsScaleClientRpc(Vector3.one);
    }

    [ClientRpc]
    void EditHandsScaleClientRpc(Vector3 scale)
    {
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        I_Damageable damageable = collision.GetComponent<I_Damageable>();
        if (damageable == null)
            return;

        bool? hit = EventsManager.sourceHasAlreadyHitDamageable?.Invoke(unifiedDamageSource, damageable);
        if (hit != null && hit.HasValue && hit.Value)
            return;

        DamageQueryInfo info = new DamageQueryInfo
        {
            owner = unifiedDamageSource.owner,
            source = unifiedDamageSource,
            damageAmount = 10
        };
        damageable.TakeDamage(info);
    }
}
