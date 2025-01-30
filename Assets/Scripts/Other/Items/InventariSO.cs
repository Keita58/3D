using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventariSO", menuName = "Scriptable Objects/InventariSO")]
public class InventariSO : ScriptableObject
{
    List<ItemSO> items = new List<ItemSO>();

    public void UsarItem(ItemSO i)
    {

        if (items.Count == 1)
            items.Remove(i);
        else
            items[items.IndexOf(i)].quantitat--;
    }

    public void AfegirItem(ItemSO i)
    {
        if (items.Contains(i))
            items[items.IndexOf(i)].quantitat++;
        else
            items.Add(i);
    }
}
