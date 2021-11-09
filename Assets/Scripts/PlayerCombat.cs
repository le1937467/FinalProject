using ClearSky;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private static PlayerCombat instance;
    private PlayerHealthBar healthBar;
    private SimplePlayerController player;

    [SerializeField]
    public float maxHealth = 100f;
    [SerializeField]
    private float playerDamage = 10f;

    private float currentHealth;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else if(instance && instance != this)
            Destroy(this);

        healthBar = GetComponentInChildren<PlayerHealthBar>();
        player = GetComponent<SimplePlayerController>();

        currentHealth = maxHealth;
    }

    public void DamagePlayer(float damageToDeal)
    {
        DamagePlayer(damageToDeal,1);
    }

    public void DamagePlayer(float damageToDeal, float kbStr)
    {
        currentHealth -= damageToDeal;

        if (currentHealth <= 0)
        {
            KillPlayer();
            healthBar.Kill();
        }
        else
        {
            healthBar.TakeDamage(damageToDeal);
        }

        player.Hurt(kbStr);
        
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
        player.Die();
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
