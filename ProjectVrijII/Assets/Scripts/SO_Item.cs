using UnityEngine;

[CreateAssetMenu(menuName = "Item/Item")]
public class SO_Item : ScriptableObject {
    public string itemName;
    public Sprite Sprite;

    public SO_AttackDecorator attackDecorator;
}
