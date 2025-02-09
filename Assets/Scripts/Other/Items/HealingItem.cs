using UnityEngine;


[CreateAssetMenu(fileName = "Item - Healing", menuName = "Scriptable Objects/Items/Item - Healing")]
public class HealingItem : Item
{
    [Header("Common values")]
    [SerializeField] private string nom;
    public override string Nom => nom;
    [SerializeField] private string descripcio;
    public override string Descripcio => descripcio;

    [SerializeField] private Sprite sprite;
    public override Sprite Sprite => sprite;

    [Header("Specific values")]
    [SerializeField] private int healing;
    public int Healing => healing;

    public override void Usar()
    {
        GameManager.instance.UsarItemCuracio(healing, this);
    }
}
