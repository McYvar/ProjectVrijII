using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour {

    private Health health;
    [SerializeField]
    private Image healthBarFilling;

    private void OnDisable() {
        health.OnHealthChanged -= HealthChanged;
    }

    public void InitializeUIHealthbar(Health health) {
        this.health = health;
        this.health.OnHealthChanged += HealthChanged;
    }

    //Called when hp changes
    private void HealthChanged(float newHealth, float maxHealth) {
        float healthPercent = newHealth / maxHealth;
        healthBarFilling.fillAmount = healthPercent;
    }
}