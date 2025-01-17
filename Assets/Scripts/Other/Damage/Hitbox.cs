using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float _Damage = 0;
    public float Damage
    {
        set => _Damage = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable objective))
            objective.RebreMal(_Damage);
    }
}
