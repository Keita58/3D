using UnityEngine;

[CreateAssetMenu(fileName = "Item - Key", menuName = "Scriptable Objects/Items/Item - Key")]

public class KeyItem : Item
{
    [Header("Common values")]
    [SerializeField] private string nom;
    public override string Nom => nom;
    [SerializeField] private string descripcio;
    public override string Descripcio => descripcio;

    [SerializeField] private Sprite sprite;
    public override Sprite Sprite => sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Usar()
    {
        GameManager.instance.UsarItemKey(this);
    }
}
