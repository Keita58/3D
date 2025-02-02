using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MostrarItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] TextMeshProUGUI textQuantitat;

    public void Load(InventariSO.ItemSlot item)
    {
        this.GetComponent<Image>().sprite=item.item.Sprite;
        this.textQuantitat.text=item.amount.ToString();
    }

    
}
