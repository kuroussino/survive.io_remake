using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public Action playerDeath;
    [SerializeField] VariableReference<float> _baseMaxHealth;
    float currentHealth;
    float maxHealth;
    public float CurrentHealth { get => currentHealth; private set => currentHealth = value; }
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
    public bool IsAlive => currentHealth > 0;
    private void Awake()
    {
        CurrentHealth = _baseMaxHealth.Value;
        MaxHealth = _baseMaxHealth.Value;
    }
    public void TakeDirectDamage(float damageAmount)
    {
        if (damageAmount > 0)
            return;

        EventsManager.OnPlayerDamage?.Invoke(damageAmount);
        float newHealth = CurrentHealth - damageAmount;
        newHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
        if(newHealth <= 0)
        {
            newHealth = 0;
            Die();
        }
        CurrentHealth = newHealth;
    }
    public void Heal(float healAmount)
    {
        if (healAmount < 0)
            return;

        EventsManager.OnPlayerHeal?.Invoke(healAmount);
        float newHealth = CurrentHealth + healAmount;
        newHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
        CurrentHealth = newHealth;
    }
    void Die()
    {
        playerDeath?.Invoke();
    }
}
