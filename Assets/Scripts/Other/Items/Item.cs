using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public abstract class Item : ScriptableObject
{
    [SerializeField]
    public string itemId {  get; set; }
    public string descripcio { get; set; }

    public Sprite sprite { get; set; }

    public abstract void Usar();
}
