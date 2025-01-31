using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MostrarInventari : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public GameObject target;

    [SerializeField] GameObject parentGameObject;

    [SerializeField] InventariSO inventari;

    [SerializeField] GameObject itemPrefab;

    // Update is called once per frame

    private void Start()
    {
        Mostrar();
    }
    public void Mostrar()
    {
        parentGameObject.transform.parent.gameObject.SetActive(true);
        foreach (InventariSO.ItemSlot itemSlot in inventari.items)
        {
            GameObject displayedItem = Instantiate(itemPrefab,parentGameObject.transform);
            displayedItem.GetComponent<MostrarItem>().Load(itemSlot);
        }
    }

    public void Amagar()
    {
        parentGameObject.transform.parent.gameObject.SetActive(false);
    }



}
