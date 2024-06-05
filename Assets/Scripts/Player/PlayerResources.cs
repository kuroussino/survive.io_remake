using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] VariableReference<float> _baseMaxHealth;
    float currentHealth;
    float maxHealth;
    float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    float MaxHealth { get => maxHealth; set => maxHealth = value; }

    private void Awake()
    {
        CurrentHealth = _baseMaxHealth.Value;
        MaxHealth = _baseMaxHealth.Value;
    }
    public void TakeDirectDamage(float damageAmount)
    {
        if (damageAmount > 0)
            return;

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

        float newHealth = CurrentHealth + healAmount;
        newHealth = Mathf.Clamp(newHealth, 0, MaxHealth);
        CurrentHealth = newHealth;
    }
    void Die()
    {
        Debug.Log("Dead");
    }
}
