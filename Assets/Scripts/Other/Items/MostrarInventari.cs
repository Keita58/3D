using UnityEngine;
using static InventariSO;

public class MostrarInventari : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public GameObject target;

    [SerializeField] GameObject parentGameObject;

    [SerializeField] InventariSO inventari;

    [SerializeField] GameObject itemPrefab;


    public void Mostrar()
    {
        Amagar();
        
        parentGameObject.transform.parent.gameObject.SetActive(true);
        int i = 0;
        for (i = 0; i < inventari.items.Count; i++)
        {
            GameObject displayedItem = Instantiate(itemPrefab, parentGameObject.transform.GetChild(i).transform);
            displayedItem.GetComponent<MostrarItem>().Load(inventari.items[i]);
            displayedItem.GetComponent<MostrarItem>().OnUsarItem += Mostrar;
        }
    }

    public void Amagar()
    {
        parentGameObject.transform.parent.gameObject.SetActive(false);
        foreach (Transform child in parentGameObject.transform)
        {
            for (int i = 0; i < child.childCount; i++)
            {
                Destroy(child.GetChild(i).gameObject);
            }
        }
    }
}
