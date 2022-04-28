using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerMovementHandler
{
    public void OnMovement(InputValue value);
}

public interface IPlayerCameraHandler
{
    public void OnCameraLook(InputValue value);
}

public interface IPlayerAttackHandler
{
    public void OnLightAttack(InputValue value);
}

public class CharacterMovement : MonoBehaviour, IPlayerCameraHandler, IPlayerMovementHandler, IPlayerAttackHandler
{
    float xVel;
    float zVel;
    public bool isRunning;
    [SerializeField] public float accelRate = 4;
    [SerializeField] public float decelRate = 2;
    [SerializeField] public float maxWalkSpeed = 2;
    [SerializeField] public float minWalkSpeed = 0.2f;
    [SerializeField] public float maxRunSpeed = 6;
    [SerializeField] public float deadZone = 0.03f;
    [SerializeField] float _moveSpeed = 1.5f;
    [Space]
    [Range(0, 0.2f)]
    [SerializeField] float _inputSmoothTime = 2f;
    CharacterController _charController;
    Animator _animator;
    Rigidbody _rigidbody;

    [SerializeField]
    CinemachineVirtualCamera _vCam;
    Camera _camera;
    Vector2 _smoothedInput = new Vector2();
    Vector2 _currentInput = new Vector2();
    Vector2 _cameraInput = new Vector2();


    [SerializeField]
    private float _lookSensitivity = 1;

    private void Awake()
    {
        _camera = FindObjectOfType<Camera>();
        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator.applyRootMotion = true;
    }

    private void Update()
    {
        _animator.speed = _moveSpeed;

        _smoothedInput = Vector2.MoveTowards(_smoothedInput, _currentInput, 0.05f);

        _animator.SetFloat("xVelocity", _smoothedInput.x);
        _animator.SetFloat("zVelocity", _smoothedInput.y);
        _animator.SetFloat("magnitude", Vector3.ClampMagnitude(_smoothedInput, 1).magnitude);

        if (_currentInput.magnitude < 0.01f)
            return;

        var lookVector = GetWorldSpaceInput(_smoothedInput, Camera.main.transform);
        SetXZLookVector(lookVector);
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

    public void OnLightAttack(InputValue value)
    {
        if(value.isPressed)
        {
            //_animator.SetTrigger("attackRequested");
            var info = _animator.GetCurrentAnimatorStateInfo(0);

            _animator.CrossFadeInFixedTime("attack_slash", 0.2f);
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

    private void OnAnimatorMove()
    {
        if (_smoothedInput.magnitude < 0.4f)
            return;

        var deltaPos = _animator.deltaPosition;
        deltaPos.y = _charController.transform.position.y;

        _charController.Move(deltaPos);
    }

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
