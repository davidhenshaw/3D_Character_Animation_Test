using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
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

    PlayerController _pc;
    CharacterController _charController;
    Rigidbody _rigidbody;
    Collider _collider;
    Camera _camera;

    private void Awake()
    {
        _camera = FindObjectOfType<Camera>();
        _pc = GetComponent<PlayerController>();
        _charController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    float prevXVel = 0;
    float prevZVel = 0;
    private void Update()
    {
        //UpdateIsRunning();
    }

    private void UpdateIsRunning()
    {
        if( xVel <= maxWalkSpeed && prevXVel > maxWalkSpeed
            && zVel <= maxWalkSpeed)
        {
            isRunning = false;
        }

        if (zVel <= maxWalkSpeed && prevZVel > maxWalkSpeed
            && xVel <= maxWalkSpeed)
        {
            isRunning = false;
        }

        prevXVel = xVel;
        prevZVel = zVel;
    }

    public Vector3 Move(Vector2 direction)
    {
        
        if (Mathf.Abs(direction.x) > deadZone)
        {
            xVel += direction.x * (accelRate * Time.deltaTime);
        }
        else
        {//No directional input? start deceleration
            xVel -= Mathf.Sign(xVel) * decelRate * Time.deltaTime;

            if (Mathf.Abs(xVel) <= minWalkSpeed)
                xVel = 0;
        }

        if (Mathf.Abs(direction.y) > deadZone)
        {
            zVel += direction.y * (accelRate * Time.deltaTime);
        }
        else
        {
            zVel -= Mathf.Sign(zVel) * decelRate * Time.deltaTime;

            if (Mathf.Abs(zVel) <= minWalkSpeed)
                zVel = 0;
        }

        //clamp the velocity
        xVel = isRunning ? Mathf.Clamp(xVel, -maxRunSpeed, maxRunSpeed) : Mathf.Clamp(xVel, -maxWalkSpeed, maxWalkSpeed);
        zVel = isRunning ? Mathf.Clamp(zVel, -maxRunSpeed, maxRunSpeed) : Mathf.Clamp(zVel, -maxWalkSpeed, maxWalkSpeed);

        //pass the calculated velocity to the rigidbody
        var currVelocity = _camera.transform.forward * zVel + _camera.transform.right * xVel;
        currVelocity.y = _rigidbody.velocity.y;

        _rigidbody.velocity = currVelocity;

        return currVelocity;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(xVel) > Mathf.Epsilon || Mathf.Abs(zVel) > Mathf.Epsilon;
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
