using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager instance { get; private set; }
    [SerializeField] Player player;
    [SerializeField] InventariSO inventari;
    [SerializeField] MostrarInventari inventariUI;
    private void Awake()
    {
        if (instance==null)
            instance = this;
    }

    public void AfegirItem(Item item)
    {
        inventari.AfegirItem(item);
        Debug.Log("Afegeixo item "+item.name);
    }

    public void ObrirInventari(GameObject target)
    {
        inventariUI.target = target;
        inventariUI.Mostrar();
    }

    public void TancarInventari()
    {
        inventariUI.target = null;
        inventariUI.Amagar();
    }

    public void UsarItemCuracio(int curacion, Item item)
    {
        player.hp+=curacion;
        Debug.Log("Player usa item de curaci�n");
        inventari.UsarItem(item);
    }

    public void UsarRecarregaBales(int numBales, Item item)
    {
        player.RecarregaBales(numBales);
        Debug.Log("Player recarrega les bales");
        inventari.UsarItem(item);
    }
    public void UsarItemKey(Item item)
    {
        inventari.UsarItem(item);
    }

    public void UsarItemThrowable(Item item, GameObject objeto)
    {
        inventari.UsarItem(item);
        player.EquiparItem(objeto);
    }
}
