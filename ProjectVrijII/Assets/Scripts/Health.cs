using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public event System.Action<GameObject> OnDeath;
    public event System.Action<float, float> OnHealthChanged;

    [SerializeField]
    private int maxHealth = 100; //may change the value in the inspector
    private int currentHealth;
    public bool died;

    private DamageParticle dmgParticle;

    private void Start() {
        GetHealed(maxHealth);
        died = false;
        dmgParticle = FindObjectOfType<DamageParticle>();
	}

	//Decrease current hp
	public void GetDamaged(int damage) {
        if (damage > 0) {
            currentHealth -= damage;

            if (currentHealth <= 0) {
                currentHealth = 0;
                OnDeath?.Invoke(gameObject);
                died = true;
            }
            Debug.Log($"{name} took {damage} damage!");

			dmgParticle.EnableVisual(transform, damage);
			OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    //Increase current hp
    public void GetHealed(int amount) {
        currentHealth += amount;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        } 

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}