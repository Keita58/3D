using UnityEngine;

public class Wall : MonoBehaviour, IAtenuacio
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int atenuarSo(int nivellSo)
    {
        return nivellSo -= 1;
    }
}
