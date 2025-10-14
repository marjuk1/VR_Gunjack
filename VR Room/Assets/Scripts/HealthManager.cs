using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthtext;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            if(healthtext != null)
            {
                healthtext.text = setHealthText();
            }
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
    private string setHealthText()
    {
        return this.currentHealth.ToString() + " / " + this.maxHealth.ToString();
    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
        if (healthtext != null)
            healthtext.text = setHealthText();
    }
}
