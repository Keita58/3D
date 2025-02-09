using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject panelCogerItem;
    [SerializeField] Player player;
    [SerializeField] private TextMeshProUGUI _Bales;

    private void Awake()
    {
        player.onInteractuable += MostrarPanelCogerItem;
        player.onNotInteractuable += OcultarPanelCogerItem;
        player.OnDisparar += TextBales;
        TextBales();
    }

    public void MostrarPanelCogerItem()
    {
        panelCogerItem.SetActive(true);
    }
    
    public void OcultarPanelCogerItem()
    {
        panelCogerItem.SetActive(false);
    }

    public void TextBales()
    {
        _Bales.text = player._Bales.ToString();
    }
}
