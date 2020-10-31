using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    PlayerController player;
    Animator animator;
    int ANIM_XVEL = Animator.StringToHash("xVel");
    int ANIM_ZVEL = Animator.StringToHash("zVel");
    int ANIM_ATTACK = Animator.StringToHash("Attack");
    int ANIM_ISWALKING = Animator.StringToHash("isWalking");

    float walkingThreshold = 0.05f;
    [SerializeField] float hitStopDuration = 0.2f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player.startedMoving += OnMovementStarted;
        player.stoppedMoving += OnMovementEnded;        
    }

    public void Attack()
    {
        animator.SetTrigger(ANIM_ATTACK);
    }

    public void UpdateVelocity(Vector3 velocity)
    {

        var fwdDotVelocity = Vector3.Dot(transform.forward, velocity);
        var rightDotVelocity = Vector3.Dot(transform.right, velocity);

        //expose velocity to animator
        animator.SetFloat(ANIM_XVEL, rightDotVelocity);
        animator.SetFloat(ANIM_ZVEL, fwdDotVelocity);

        if(Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.z)) > walkingThreshold)
        {
            animator.SetBool(ANIM_ISWALKING, true);
        }
        else
        {
            animator.SetBool(ANIM_ISWALKING, false);
        }
    }

    IEnumerator HitStopWait()
    {
        animator.speed = 0;
        yield return new WaitForSeconds(hitStopDuration);
        animator.speed = 1;
    }

    public void EngageHitStop()
    {
        StartCoroutine(HitStopWait());
    }

    void OnMovementStarted()
    {
        animator.SetBool(ANIM_ISWALKING, true);
    }

    void OnMovementEnded()
    {
        animator.SetBool(ANIM_ISWALKING, false);
    }

}