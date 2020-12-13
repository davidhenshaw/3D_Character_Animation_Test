using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    float currSpeed;
    public bool isRunning;
    [SerializeField] public float accelRate = 4;
    [SerializeField] public float decelRate = 2;
    [SerializeField] public float maxWalkSpeed = 2;
    [SerializeField] public float minWalkSpeed = 0.2f;
    [SerializeField] public float maxRunSpeed = 6;
    [SerializeField] public float deadZone = 0.03f;
    [SerializeField] float turnRate = 0.005f;

    [SerializeField] Transform lookDir;
    [SerializeField] Transform lockOnTarget;
    float currTurnVelocity;
    float targetAngle;
    float currAngle;
    Vector3 fwdDir;

    Rigidbody _rigidbody;
    Collider _collider;

    private void Awake()
    {
        if(lookDir == null)
            lookDir = Camera.main.transform;

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void SetLockOnTarget(Transform target)
    {
        lockOnTarget = target;
    }

    public Vector3 Move(Vector3 inputAxis)
    {
        // if the player inputs any direction, the character speed increases
        // if no input is detected, the speed decreases toward zero
        if (inputAxis.magnitude > deadZone)
        {
            currSpeed += accelRate * Time.deltaTime;
        }
        else
        {
            currSpeed -= decelRate * Time.deltaTime;
        }

        // Clamp velocity between the max and min run speeds based on whether you're running
        currSpeed = isRunning ? Mathf.Clamp(currSpeed, 0, maxRunSpeed) : Mathf.Clamp(currSpeed, 0, maxWalkSpeed);

        // Pass the velocity to the rigidbody
        // mask out the 
        var newVelocity = currSpeed * fwdDir;
        _rigidbody.velocity = new Vector3(newVelocity.x, _rigidbody.velocity.y, newVelocity.z);

        return currSpeed * fwdDir;
    }

    public void UpdateRotation(Vector2 inputAxis)
    {
        float lookOffset = lookDir.eulerAngles.y;

        if(lockOnTarget != null)
        {
            Vector3 playerToTarget = lockOnTarget.position - transform.position;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, playerToTarget);
            lookOffset = rot.eulerAngles.y;
        }

        //If there is no input, don't update the target angle since this will make it default to zero all the time
        if(inputAxis.magnitude > Mathf.Epsilon)
            targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg + lookOffset;

        currAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currTurnVelocity, turnRate);
        transform.rotation = Quaternion.Euler(0, currAngle, 0);

        fwdDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        fwdDir = fwdDir.normalized;
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }
}
