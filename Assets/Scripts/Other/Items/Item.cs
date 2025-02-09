using System;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public abstract string Nom { get; }
    public abstract string Descripcio { get; }
    public abstract Sprite Sprite { get; }

    public abstract void Usar();
}
