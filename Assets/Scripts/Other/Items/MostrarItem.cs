using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MostrarItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] TextMeshProUGUI textQuantitat;
    public event Action OnUsarItem;
    Item itemSeleccionado;

    public void Load(InventariSO.ItemSlot item)
    {
        this.GetComponent<Image>().sprite=item.item.Sprite;
        this.textQuantitat.text=item.amount.ToString();
        itemSeleccionado=item.item;
    }

    public void ClicarItem()
    {
        if (itemSeleccionado != null)
        {
            itemSeleccionado.Usar();
            OnUsarItem?.Invoke();
        }
    }

    
}
