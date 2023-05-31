using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour {
    public Transform assignedCharacter;

    [SerializeField]
    private Image healthBarFilling;
   
    private void OnEnable() {
        if (assignedCharacter) assignedCharacter.GetComponent<Health>().OnHealthChanged += HealthChanged;
    }

    private void OnDisable() {
        if (assignedCharacter) assignedCharacter.GetComponent<Health>().OnHealthChanged -= HealthChanged;
    }

    //Called when hp changes
    private void HealthChanged(float newHealth, float maxHealth) {
        float healthPercent = newHealth / maxHealth;
        healthBarFilling.fillAmount = healthPercent;
    }
}