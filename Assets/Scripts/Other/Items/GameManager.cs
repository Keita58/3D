using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager instance { get; private set; }
    [SerializeField] Player player;
    private void Awake()
    {
        if (instance==null)
            instance = this;
    }

    public void UsarItemCuracio(int curacion)
    {
        player.hp+=curacion;
        Debug.Log("Player usa item de curación");
    }
}
