using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour {

    private Health health;
    [SerializeField]
    private Image healthBarFilling;
    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private float fullHealthPosition;
    [SerializeField]
    private float emptyHealthPosition;

    private Vector2 healthStartPosition;

    private void OnDisable() {
        health.OnHealthChanged -= HealthChanged;
    }

    public void InitializeUIHealthbar(Health health) {
        this.health = health;
        healthStartPosition = healthBarFilling.rectTransform.localPosition;
        this.health.OnHealthChanged += HealthChanged;
    }

    //Called when hp changes
    private void HealthChanged(float newHealth, float maxHealth) {
         float healthPercent = newHealth / maxHealth;
        //  healthBarFilling.fillAmount = healthPercent;

        Vector2 healthFillPosition = healthStartPosition;
        healthFillPosition.x = Mathf.Lerp(emptyHealthPosition, fullHealthPosition, healthPercent);
        healthBarFilling.rectTransform.localPosition = healthFillPosition;

        if (healthText) healthText.text = $"{Mathf.Ceil(newHealth)}";
    }
}