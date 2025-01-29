using TMPro;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    [SerializeField] private TextMeshPro txt;
    [SerializeField] private UnityEngine.UI.Image itemImg;
    //[SerializeField] Button dropItemBtn;

    private Item currentItem;
    private int quantity;

    public void AddItem(Item item)
    {
        this.quantity = 1;
        currentItem = item;
        txt.SetText(item.itemName);
        itemImg.sprite = item.itemImg.sprite;
    }
    public void AddCount(int c)
    {
        this.quantity += c;
        txt.SetText($"{currentItem.itemName} x{this.quantity}");
    }

    public InventoryCell GetInventoryCell() { return this; }
    public Item GetItem() { return currentItem; }

    public int GetQuantity() { return quantity; }

    public void RemoveItem(Item item)
    {
        Destroy(currentItem);
    }
}
