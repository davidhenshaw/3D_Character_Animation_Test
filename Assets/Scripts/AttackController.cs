using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    PlayerController player;
    Hitbox[] hitBoxes;

    private void Awake()
    {
        hitBoxes = GetComponentsInChildren<Hitbox>();
        player = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(Hitbox hb in hitBoxes)
        {
            hb.hitLanded += OnHitLanded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnHitLanded()
    {
        player.OnHitLanded();
    }
}
