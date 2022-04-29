using Cinemachine;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IMovementInputHandler
{
    public void OnMovement(InputValue value);
}

public interface ICameraInputHandler
{
    public void OnCameraLook(InputValue value);
}

public interface IPlayerAttackHandler
{
    public void OnLightAttack(InputValue value);
}

public interface IAttackAnimationHandler
{
    void SetAttackState(AttackState state);
}

public class CharacterMovement : MonoBehaviour, ICameraInputHandler, IMovementInputHandler, IPlayerAttackHandler, IAttackAnimationHandler
{
    const string LightAttack = "attack_slash";
    const string ComboTrigger = "comboRequested";

    CharacterController _charController;
    Animator _animator;
    Rigidbody _rigidbody;
    [Min(0)]
    [SerializeField] float _animatorSpeed = 1.5f;
    [Space]


    [SerializeField]
    CinemachineVirtualCamera _vCam;

    public InputAction _lightAttack;

    Camera _camera;
    Vector2 _smoothedInput = new Vector2();
    Vector2 _currentInput = new Vector2();
    Vector2 _cameraInput = new Vector2();
    bool applyRootMotion = true;

    [SerializeField]
    private float _lookSensitivity = 1;

    [SerializeField]
    private BehaviorTree _tree;
    private AttackState _attackState = AttackState.None;

    private void Awake()
    {
        _camera = FindObjectOfType<Camera>();
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        SetUpBehaviorTree();
    }

    private void SetUpBehaviorTree()
    {
        _tree = new BehaviorTreeBuilder(gameObject)
            .Sequence("Continue Loop?")
                .Inverter("Not Attacking?")
                    .Sequence()
                        .Condition("Request Attack", RequestedLightAttack)
                        .Sequence("Attack Sequence")
                            .Do("Transition", TransitionToStartup)
                            .Do("Startup", HandleAttackStartup)
                            .Do("Active", HandleAttackActive)
                            .Do("Cooldown", HandleAttackCooldown)
                        .End()
                    .End()
                .End()
                .Sequence("Handle Locomotion")
                    .Do("Locomotion", LocomotionTask)
                .End()
            .End()
            .Build();
    }

    BehaviorTree AttackSubtree()
    {
        return new BehaviorTreeBuilder(gameObject)
            .Inverter("Not Attacking?")
                    .Sequence()
                        .Condition("Request Attack", RequestedLightAttack)
                        .Sequence("Attack Sequence")
                            .Splice(ComboCheckSubtree())
                            .Do("Transition", TransitionToStartup)
                            .Do("Startup", HandleAttackStartup)
                            .Do("Active", HandleAttackActive)
                            .Do("Cooldown", HandleAttackCooldown)
                        .End()
                    .End()
                .End()
                .Build();
    }

    BehaviorTree ComboCheckSubtree()
    {
        return new BehaviorTreeBuilder(gameObject)
            .Sequence("Combo Check")
                .Inverter()
                    .Condition("Cooldown", () => _attackState == AttackState.Cooldown)
                    .Condition("Request Attack", RequestedLightAttack)
                .End()
            .End()
            .Build();
    }

    private void Update()
    {
        _tree.Tick();
    }

    private void OnEnable()
    {
        _lightAttack.Enable();
    }

    private void OnDisable()
    {
        _lightAttack.Disable();
    }

    private void OnAnimatorMove()
    {
        if (!applyRootMotion)
            return;

        var deltaPos = _animator.deltaPosition;
        deltaPos.y = _charController.transform.position.y;

        _charController.Move(deltaPos);
    }
    
    private void LateUpdate()
    {
        CinemachineComponentBase cinemachineComponent = _vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        if (cinemachineComponent)
        {
            HandleTransposerCam(_cameraInput, cinemachineComponent);
            return;
        }
    }

    //Behavior Tree

    TaskStatus ComboCheckTask()
    {
        var canCombo = _attackState == AttackState.Active || _attackState == AttackState.Cooldown;

        if (RequestedLightAttack() &&  canCombo)
            return TaskStatus.Failure;

        return TaskStatus.Success;
    }

    private TaskStatus LocomotionTask()
    {
        _animator.speed = _animatorSpeed;

        _smoothedInput = Vector2.MoveTowards(_smoothedInput, _currentInput, 0.05f);

        if (_smoothedInput.magnitude < 0.4f)
            applyRootMotion = false;
        else
            applyRootMotion = true;

        UpdateAnimatorState();

        UpdateLookDirection();

        return TaskStatus.Success;
    }

    TaskStatus TransitionToStartup()
    {
        if (_attackState != AttackState.None)
            return TaskStatus.Success;

        _animator.CrossFadeInFixedTime(LightAttack, 0.2f);
        applyRootMotion = true;
        _attackState = AttackState.Startup;
        return TaskStatus.Success;
    }

    TaskStatus HandleAttackStartup()
    {
        if (_attackState != AttackState.Startup)
            return TaskStatus.Success;

        return TaskStatus.Continue;
    }

    TaskStatus HandleAttackActive()
    {
        if (_attackState != AttackState.Active)
            return TaskStatus.Success;

        if (RequestedLightAttack())
            _animator.SetTrigger(ComboTrigger);

        return TaskStatus.Continue;
    }

    TaskStatus HandleAttackCooldown()
    {
        if (_attackState != AttackState.Cooldown)
        {
            applyRootMotion = false;
            return TaskStatus.Success;
        }

        if (RequestedLightAttack())
            _animator.SetTrigger(ComboTrigger);

        return TaskStatus.Continue;
    }

    private void UpdateLookDirection()
    {
        if (_currentInput.magnitude < 0.01f)
            return;

        var lookVector = GetWorldSpaceInput(_smoothedInput, Camera.main.transform);
        SetXZLookVector(lookVector);
    }

    private void UpdateAnimatorState()
    {
        _animator.SetFloat("xVelocity", _smoothedInput.x);
        _animator.SetFloat("zVelocity", _smoothedInput.y);
        _animator.SetFloat("magnitude", Vector3.ClampMagnitude(_smoothedInput, 1).magnitude);
    }

    bool RequestedLightAttack()
    {
        if (_lightAttack.triggered)
        {
            //Start doing the attack animation
            //_animator.CrossFadeInFixedTime(LightAttack, 0.2f);
            //var attackAnim = _animator.GetBehaviour<AttackBehaviour>();
            //attackAnim.IsAttacking = true;
            return true;
        }
        else
            return false;
    }

    // Interface contracts

    public void SetAttackState(AttackState state)
    {
        _attackState = state;
    }

    public void OnLightAttack(InputValue value)
    {
        if(value.isPressed)
        {
            //HandleLightAttack();
        }
    }

    public void OnCameraLook(InputValue input)
    {
        _cameraInput = input.Get<Vector2>();
    }

    public void OnMovement(InputValue value)
    {
        _currentInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
            _animator.SetBool("running", false);
        else
            _animator.SetBool("running", true);
    }    

    //Helpers

    private void HandleTransposerCam(Vector2 input, CinemachineComponentBase component)
    {
        var transposer = component as CinemachineOrbitalTransposer;
        transposer.m_XAxis.Value = input.x * _lookSensitivity;
    }

    public Vector3 GetWorldSpaceInput(Vector2 controllerInput, Transform relativeTo)
    {
        var vec = new Vector3(controllerInput.x, 0, controllerInput.y);
        var worldSpaceVec = relativeTo.TransformVector(vec);

        return worldSpaceVec;
    }

    public void SetXZLookVector(Vector3 direction)
    {
        Vector3 fwdEuler = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        transform.rotation = Quaternion.Euler(0, fwdEuler.y, 0);
    }

    public void SetForwardDirection(Vector3 direction)
    {
        Vector3 fwdEuler = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
        transform.rotation = Quaternion.Euler(0, fwdEuler.y, 0);
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }
}
