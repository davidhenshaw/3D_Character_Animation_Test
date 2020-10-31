using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action startedMoving;
    public event Action stoppedMoving;

    Vector3 velocity;
    Vector2 direction;
    [SerializeField] MoveController moveCtrl;
    [SerializeField] AnimationController animCtrl;
    [SerializeField] AttackController attackCtrl;

    public InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animCtrl.Attack();
        }  

        direction = controls.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        velocity = moveCtrl.Move(direction);
        animCtrl.UpdateVelocity(velocity);
    }

    public void OnHitLanded()
    {
        animCtrl.EngageHitStop();
    }

}
