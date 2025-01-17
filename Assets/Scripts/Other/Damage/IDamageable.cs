using System;
using UnityEngine;

public interface IDamageable
{
    void RebreMal(float damage);
}

public interface IAttack
{
    public float Damage { set; }
}
