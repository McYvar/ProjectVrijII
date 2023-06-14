using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    // EDIT NOTES: Spawning particles made more dynamic, they will spawn in a randomize direction within a certain range
    
    public event System.Action<GameObject> OnDeath;
    public event System.Action<float, float> OnHealthChanged;

    [SerializeField]
    private int maxHealth = 100; //may change the value in the inspector
    private int currentHealth;
    public bool died;

    [SerializeField] private GameObject dmgParticlePrefab;

    private void Start() {
        GetHealed(maxHealth);
        died = false;
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

            Instantiate(dmgParticlePrefab, transform).GetComponent<DamageParticle>().Spawn(damage);
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