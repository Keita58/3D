using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public abstract class Item : ScriptableObject
{
    [SerializeField]
    string itemId {  get; set; }
    string descripcio { get; set; }

    Sprite sprite { get; set; }

    public abstract void Usar();
}
