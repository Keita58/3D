using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCellPrefab;
    [SerializeField] private GameObject inventoryCellParent;
    private List<InventoryCell> cells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cells = new List<InventoryCell>();
    }

    public void AddItem(Item item)
    {
        InventoryCell cellWithItem = cells.Find((i) => i.GetItem().itemName == item.itemName);

        if (cellWithItem != null)
        {
            cellWithItem.AddCount(1);
            return;
        }

        GameObject newCell = Instantiate(inventoryCellPrefab, inventoryCellParent.transform);
        InventoryCell cellInstance = newCell.GetComponent<InventoryCell>();

        //cellInstance.Init();
        cells.Add(cellInstance);
    }
    public void RemoveItem(GameObject inventoryCellInstance)
    {
        InventoryCell target = cells.Find((X) => X.GetInventoryCell().GetInstanceID() == inventoryCellInstance.GetInstanceID());
        if (target.GetQuantity() == 1) {
            cells.Remove(target);
        }else if (target.GetQuantity() > 1)
        {
            target.AddCount(-1);
        }
    }
}
