using System.Collections.Generic;
using UnityEngine;

public class MostrarInventari : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject target;

    [SerializeField] List<GameObject> slots = new List<GameObject>();

    [SerializeField] InventariSO inventari;
    void Start()
    {
        
    }

    // Update is called once per frame
   private void Mostrar()
    {
        foreach(InventariSO.ItemSlot itemSlot in inventari.items)
        {

        }
    }
}
