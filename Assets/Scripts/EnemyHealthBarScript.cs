using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarScript : MonoBehaviour
{
    Slider slider;
    Image fill;
    EnemyUniversalController enemy;

    private void Start()
    {
        fill = GetComponentInChildren<Image>(true);
        slider = GetComponentInChildren<Slider>(true);
        enemy = GetComponentInParent<EnemyUniversalController>();

        SetMaxHealth(enemy.maxHealth);

        gameObject.SetActive(false);
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        slider.gameObject.SetActive(false);
    }

    public void TakeDamage(float health)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        slider.value -= health;
        if (slider.value/slider.maxValue*100 > 60)
        {
            //green
            fill.color = Color.green;
        }
        else if(slider.value/slider.maxValue*100 < 35)
        {
            //red
            fill.color = Color.red;
        }
        else
        {
            //orange
            fill.color = new Color(255,165,0);
        }
        slider.gameObject.SetActive(slider.normalizedValue < 1f);
    }
}
