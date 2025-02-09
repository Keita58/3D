using UnityEngine;

[CreateAssetMenu(fileName = "Item - Amunition", menuName = "Scriptable Objects/Items/Item - Amunition")]
public class AmmunitionItem : Item
{
    [Header("Common values")]
    [SerializeField] private string nom;
    public override string Nom => nom;
    [SerializeField] private string descripcio;
    public override string Descripcio => descripcio;

    [SerializeField] private Sprite sprite;
    public override Sprite Sprite => sprite;

    [Header("Specific values")]
    [SerializeField] public int bales;


    public override void Usar()
    {
        GameManager.instance.UsarRecarregaBales(bales, this);
    }
}
