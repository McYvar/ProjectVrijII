using UnityEngine;

[CreateAssetMenu(menuName = "Item/Decorators/AttackDecorator")]
public class SO_AttackDecorator : ScriptableObject {
    [Range(0, 100)] public float increaseAttackByPercentage;

    [HideInInspector] public SO_Attack attack;

    public SO_Attack Decorate(SO_Attack attack, int hitNumber) {
        SO_Attack attackToDecorate = attack;
        attack.strength[hitNumber] += attack.strength[hitNumber] * increaseAttackByPercentage / 100;
        return attackToDecorate;
    }
}
