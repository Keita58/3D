using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject panelCogerItem;
    [SerializeField] Player player;

    private void Awake()
    {
        player.onInteractuable += MostrarPanelCogerItem;
        player.onNotInteractuable += OcultarPanelCogerItem;
    }

    public void MostrarPanelCogerItem()
    {
        panelCogerItem.SetActive(true);
    }
    
    public void OcultarPanelCogerItem()
    {
        panelCogerItem.SetActive(false);
    }


}
