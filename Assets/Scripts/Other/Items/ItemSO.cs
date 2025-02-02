using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public int id;
    public string nom;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public int quantitat;
    public Sprite imatge;


}
