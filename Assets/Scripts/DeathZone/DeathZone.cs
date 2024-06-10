using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DeathZone : MonoBehaviour, I_DamageOwner
{
    [SerializeField] DeathZoneHitbox deathZoneHitbox;
    [SerializeField] DeathZoneHitbox safeZoneHitbox;

    [SerializeField] float deathZoneStateDuration;
    float deathZoneCurrTimeLeft;

    [SerializeField] float safeZoneMovementDuration;

    [SerializeField] VariableReference<float> secondsPerDamageTick;
    [SerializeField] VariableReference<float> damagePerDamageTick;
    Dictionary<I_Damageable, PlayerDeathZoneInfo> dictDamageableDamageTick;

    private void Awake()
    {
        dictDamageableDamageTick = new Dictionary<I_Damageable, PlayerDeathZoneInfo>();
        
        deathZoneCurrTimeLeft = deathZoneStateDuration;

        deathZoneHitbox.transform.position = Vector3.zero;
        safeZoneHitbox.transform.position = Vector3.zero;

        deathZoneHitbox.transform.localScale = new Vector3(15f,15f,15f);
        safeZoneHitbox.transform.localScale = new Vector3(10f, 10f, 10f);
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
                    DamageQueryInfo info = new DamageQueryInfo();
                    info.damageAmount = damagePerDamageTick;
                    info.owner = this;
                    item.Key.TakeDamage(info);
                }
            }
        }
        foreach (var item in damageablesToRemove)
        {
            dictDamageableDamageTick.Remove(item);
        }

        DeathZoneBehaviour();
    }

    private void OnSafeZoneHitbox(I_Damageable damageable, bool triggerEnter)
    {
        if (!dictDamageableDamageTick.ContainsKey(damageable))
        {
                PlayerDeathZoneInfo newInfo = new PlayerDeathZoneInfo();
                newInfo.timeToDamageTick = secondsPerDamageTick;
                newInfo.isInsideSafeZone = triggerEnter;
                dictDamageableDamageTick.Add(damageable, newInfo);
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

    void DeathZoneBehaviour()
    {
        deathZoneCurrTimeLeft -= Time.deltaTime;

        if (deathZoneCurrTimeLeft <= 0)
        {
            deathZoneCurrTimeLeft = deathZoneStateDuration;
            MoveToPosition(new Vector3(UnityEngine.Random.Range(-18f, 18f), UnityEngine.Random.Range(-18f, 18f)), safeZoneMovementDuration);
            if (safeZoneHitbox.transform.localScale.x - 2f >= 1f || safeZoneHitbox.transform.localScale.y - 2f >= 1f)
                Scale(safeZoneHitbox.transform.localScale - new Vector3(2f, 2f, 2f), safeZoneMovementDuration);
        }
    }

    void MoveToPosition(Vector3 position, float timeToMove)
    {
        safeZoneHitbox.transform.DOMove(position, timeToMove)
            .SetEase(Ease.InSine);
    }
    void Scale(Vector3 newScale, float timeToScale)
    {
        safeZoneHitbox.transform.DOScale(newScale, timeToScale)
            .SetEase(Ease.InSine);

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