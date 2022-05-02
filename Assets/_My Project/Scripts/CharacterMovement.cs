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
    const string LightAttackTrigger = "lightAttackReq";
    const string DodgeTrigger = "dodgeRequested";

    CharacterController _charController;
    Animator _animator;
    Rigidbody _rigidbody;
    [Min(0)]
    [SerializeField] float _animatorSpeed = 1.5f;
    [Space]

    public InputAction _lightAttackInput;
    public InputAction _dodgeInput;

    Vector2 _smoothedInput = new Vector2();
    Vector2 _currentInput = new Vector2();
    Vector2 _cameraInput = new Vector2();
    bool applyRootMotion = true;
    bool dodgeQueued = false;

    [SerializeField]
    private float _lookSensitivity = 1;

    [SerializeField]
    private BehaviorTree _tree;

    [Tooltip("Used by Animator")]
    public AttackState _attackState = AttackState.Done;

    private void Awake()
    {
        _attackState = AttackState.Done;
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        SetUpBehaviorTree();
    }


    private void Update()
    {
        _tree.Tick();
        _smoothedInput = Vector2.MoveTowards(_smoothedInput, _currentInput, 0.05f);
    }

    private void OnEnable()
    {
        _lightAttackInput.Enable();
        _dodgeInput.Enable();
    }

    private void OnDisable()
    {
        _lightAttackInput.Disable();
        _dodgeInput.Disable();
    }

    
    private void OnAnimatorMove()
    {
        if (!applyRootMotion)
            return;

        var deltaPos = _animator.deltaPosition;
        deltaPos.y = _charController.transform.position.y;

        _charController.Move(deltaPos);
    }

    //Behavior Tree
    private void SetUpBehaviorTree()
    {
        _tree = new BehaviorTreeBuilder(gameObject)
            .Selector("Continue Loop?")
                .Sequence("Handle Dodge")
                    .Condition("Requested Dodge?", ()=> (_dodgeInput.triggered || dodgeQueued))
                    .Splice(DodgingSubTree())
                .End()
                .Sequence("Handle Attack")
                    .Condition("Request Attack", RequestedLightAttack)
                    .Condition("Check Can Attack", CanAttackCheck)
                    .Splice(AttackSubTree())
                .End()
                .Sequence("Handle Locomotion")
                    .Do("Locomotion", LocomotionTask)
                .End()
            .End()
            .Build();
    }

    BehaviorTree AttackSubTree()
    {
        return new BehaviorTreeBuilder(gameObject)
            .Sequence("Attack Sequence")
                .Do("Set Animation", () => TriggerAnim(LightAttackTrigger))
                .Do("Await Startup", WaitUntilAnimStart)
                .Do("Handle Attack", AttackTask)
            .End()
            .Build();
    }

    BehaviorTree DodgingSubTree()
    {
        return new BehaviorTreeBuilder(gameObject)
        .Sequence("Dodge Sequence")
            .Do("Set Animation", ()=>TriggerAnim(DodgeTrigger))
            .Do("Await Startup", WaitUntilAnimStart)
            .Do("Handle Dodge", DodgeTask)
        .End()
        .Build();
    }

    bool CanAttackCheck()
    {
        bool permission = true;
        switch(_attackState)
        {
            case AttackState.Transitioning:
            case AttackState.Startup:
                permission = false;
                break;
        }
        return permission;
    }

    TaskStatus TriggerAnim(string animTrigger)
    {
        _animator.SetTrigger(animTrigger);
        applyRootMotion = true;
        _attackState = AttackState.Transitioning;

        return TaskStatus.Success;
    }

    TaskStatus WaitUntilAnimStart()
    {
        if (_attackState != AttackState.Startup)
            return TaskStatus.Continue;

        return TaskStatus.Success;
    }

    TaskStatus AttackTask()
    {
        if (_attackState == AttackState.Done)
            return TaskStatus.Success;

        bool canCombo = _attackState == AttackState.Active || _attackState == AttackState.Cooldown;

        if(_attackState == AttackState.Startup)
        {
            applyRootMotion = true;
            UpdateLookDirection();
        }

        if(_dodgeInput.triggered)
        {
            if(_attackState != AttackState.Active && _attackState != AttackState.Startup)
            {
                dodgeQueued = true;
                _tree.Reset();
                return TaskStatus.Failure;
            }
        }

        if(RequestedLightAttack() && canCombo)
        {
            TriggerAnim(LightAttackTrigger);
            return TaskStatus.Continue;
        }

        return TaskStatus.Continue;
    }

    TaskStatus DodgeTask()
    {
        if(_attackState == AttackState.Done)
        {
            dodgeQueued = false;
            return TaskStatus.Success;
        }
        
        if (_attackState == AttackState.Startup || _attackState == AttackState.Cooldown)
            UpdateLookDirection();

        if (_dodgeInput.triggered && _attackState != AttackState.Startup)
            TriggerAnim(DodgeTrigger);

        return TaskStatus.Continue;
    }

    private TaskStatus LocomotionTask()
    {
        _animator.speed = _animatorSpeed;

        //_smoothedInput = Vector2.MoveTowards(_smoothedInput, _currentInput, 0.05f);

        if (_smoothedInput.magnitude < 0.4f)
            applyRootMotion = false;
        else
            applyRootMotion = true;

        UpdateAnimatorState();

        UpdateLookDirection();

        return TaskStatus.Success;
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
        if (_lightAttackInput.triggered)
        {
            return true;
        }
        else
            return false;
    }

    // Interface contracts

    public void SetAttackState(AttackState requestedState)
    {
        if (requestedState == AttackState.Done && _attackState != AttackState.Cooldown)
            return; //Only allow transitions to Done if we were previously in cooldown

        _attackState = requestedState;
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
