using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] DeathZoneHitbox deathZoneHitbox;
    [SerializeField] DeathZoneHitbox safeZoneHitbox;

    [SerializeField] VariableReference<float> damagePerTick;
    [SerializeField] VariableReference<float> secondsPerDamageTick;
    Dictionary<I_Damageable, PlayerDeathZoneInfo> dictDamageableDamageTick;

    private void Awake()
    {
        dictDamageableDamageTick = new Dictionary<I_Damageable, PlayerDeathZoneInfo>();
    }
    private void OnEnable()
    {
        deathZoneHitbox.damageableTrigger += OnDeathZoneHitbox;
        safeZoneHitbox.damageableTrigger += OnSafeZoneHitbox;
    }
    private void OnDisable()
    {
        deathZoneHitbox.damageableTrigger -= OnDeathZoneHitbox;
        safeZoneHitbox.damageableTrigger -= OnSafeZoneHitbox;
    }
    void Update()
    {
        List<I_Damageable> damageablesToRemove = new List<I_Damageable>();
        foreach (var item in dictDamageableDamageTick)
        {
            item.Value.AddTime(Time.deltaTime);
            if(item.Value.timeToDamageTick >= secondsPerDamageTick)
            {
                if (item.Value.isInsideSafeZone)
                {
                    damageablesToRemove.Add(item.Key);
                }
                else
                {
                    item.Value.AddTime(-secondsPerDamageTick);
                    item.Key.TakeDamage(damagePerTick);
                }
            }
        }
        foreach (var item in damageablesToRemove)
        {
            dictDamageableDamageTick.Remove(item);
        }
    }

    private void OnSafeZoneHitbox(I_Damageable damageable, bool triggerEnter)
    {
        if (!dictDamageableDamageTick.ContainsKey(damageable))
        {
            if (!triggerEnter)
            {
                PlayerDeathZoneInfo newInfo = new PlayerDeathZoneInfo();
                newInfo.timeToDamageTick = secondsPerDamageTick;
                newInfo.isInsideSafeZone = triggerEnter;
                dictDamageableDamageTick.Add(damageable, newInfo);
            }
        }
        else
        {
            dictDamageableDamageTick[damageable].IsInsideSafeZone(triggerEnter);
        }
    }
    private void OnDeathZoneHitbox(I_Damageable damageable, bool triggerEnter)
    {
        if (!dictDamageableDamageTick.ContainsKey(damageable))
        {
            PlayerDeathZoneInfo newInfo = new PlayerDeathZoneInfo();
            newInfo.timeToDamageTick = secondsPerDamageTick;
            newInfo.isInsideSafeZone = false;
            dictDamageableDamageTick.Add(damageable, newInfo);
        }
    }

    void MoveToPosition(Vector3 position, float timeToMove)
    {
        transform.DOMove(position, timeToMove);
    }
    void Scale(Vector3 newScale, float timeToScale)
    {
        transform.DOScale(newScale, timeToScale);
    }
}

class PlayerDeathZoneInfo
{
    public float timeToDamageTick;
    public bool isInsideSafeZone;

    public void IsInsideSafeZone(bool inside)
    {
        isInsideSafeZone = inside;
    }
    public void AddTime(float timeAmount)
    {
        timeToDamageTick += timeAmount;
    }
}