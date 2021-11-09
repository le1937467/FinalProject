using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    Slider slider;
    Image fill;
    PlayerCombat player;

    private void Start()
    {
        fill = GetComponentInChildren<Image>(true);
        slider = GetComponentInChildren<Slider>(true);
        player = PlayerCombat.GetInstance();

        SetMaxHealth(player.maxHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        slider.gameObject.SetActive(false);
    }

    public void Kill()
    {
        slider.value = 0;
    }

    public void TakeDamage(float health)
    {
        slider.value -= health;
        if (slider.value / slider.maxValue * 100 > 60)
        {
            //green
            fill.color = Color.green;
        }
        else if (slider.value / slider.maxValue * 100 < 35)
        {
            //red
            fill.color = Color.red;
        }
        else
        {
            //orange
            fill.color = new Color(255, 165, 0);
        }
    }
}
