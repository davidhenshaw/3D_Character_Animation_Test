using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] int damage;
    AttackController attackCtrl;
    public event Action hitLanded;

    private void Awake()
    {
        attackCtrl = GetComponent<AttackController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var hurtbox = other.gameObject.GetComponent<Hurtbox>();

        if(hurtbox != null)
        {
            hurtbox.TakeDamage(damage);
            hitLanded?.Invoke();
            VFXManager.current.SpawnHitVFX(transform.position);
        }
    }
}
