using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private static PlayerCombat instance;

    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float playerDamage = 10f;

    private float currentHealth;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else if(instance && instance != this)
        {
            Destroy(this);
        }

        currentHealth = maxHealth;
    }

    public void DamagePlayer(float damageToDeal)
    {
        currentHealth -= damageToDeal;

        if(currentHealth <= 0)
        {
            KillPlayer();
        }
    }

    public void HealPlayer(float amountToHeal)
    {
        currentHealth += amountToHeal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public float GetPlayerHealth()
    {
        return currentHealth;
    }

    public void KillPlayer()
    {
        //TODO - Reload scene and play death animation
        Debug.LogWarning("You are dead");
    }

    public void DealDamageTo(EnemyUniversalController enemy)
    {
        enemy.TakeDamage(playerDamage);
    }


    public static PlayerCombat GetInstance()
    {
        return instance;
    }

}
