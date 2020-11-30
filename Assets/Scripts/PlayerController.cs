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
    Vector2 inputDir;
    [SerializeField] Camera targetCamera;
    [SerializeField] MoveController moveCtrl;
    [SerializeField] AnimationController animCtrl;
    [SerializeField] AttackController attackCtrl;

    public InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
    }

    private void Start()
    {
        if(targetCamera == null)
            targetCamera = Camera.main;        
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    //Player input is read here
    private void Update()
    {
        inputDir = controls.Player.Movement.ReadValue<Vector2>();

        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animCtrl.Attack();
        }  

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            animCtrl.AttackSword_L();
        }
    }

    private void FixedUpdate()
    {
        velocity = moveCtrl.Move(inputDir);

        if(moveCtrl.IsMoving())
            moveCtrl.SetForwardDirection(targetCamera.transform.forward);

        animCtrl.UpdateVelocity(velocity);      
    }

    public void OnHitLanded()
    {
        animCtrl.EngageHitStop();
    }

}
