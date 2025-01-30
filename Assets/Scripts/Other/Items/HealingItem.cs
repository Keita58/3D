using UnityEngine;

public class HealingItem : Item
{
    private int healing;
    [SerializeField] ItemCuracionSO curacionSO;

    private void Awake()
    {
        this.healing=curacionSO.nCuracio;
    }
    public override void Usar()
    {
        GameManager.instance.UsarItemCuracio(healing);
    }
}
