using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    float xVel;
    float zVel;
    float rotation;
    [SerializeField] public float accelRate = 4;
    [SerializeField] public float decelRate = 2;
    [SerializeField] public float maxWalkSpeed = 2;
    [SerializeField] public float minWalkSpeed = 0.2f;
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

    public Vector3 Move(Vector2 direction)
    {
        //Debug.Log("Player wants to move " + direction);

        //No directional input? start deceleration
        if (Mathf.Abs(direction.x) > deadZone)
        {
            xVel += direction.x * (accelRate * Time.deltaTime);
        }
        else
        {
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
        xVel = Mathf.Clamp(xVel, -maxWalkSpeed, maxWalkSpeed);
        zVel = Mathf.Clamp(zVel, -maxWalkSpeed, maxWalkSpeed);

        

        //var currVelocity = new Vector3(xVel, _rigidbody.velocity.y, zVel);

        var currVelocity = _camera.transform.forward * zVel + _camera.transform.right * xVel;
        currVelocity.y = _rigidbody.velocity.y;

        _rigidbody.velocity = currVelocity;

        return currVelocity;
    }

    public void SetRotation(float degrees)
    {
        rotation = degrees;
    }
}
