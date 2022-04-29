using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public bool IsAttacking { get; set; }
    public AttackState state { get; private set; } = AttackState.Done;
    private Stack<AnimatorStateInfo> attackCalls = new Stack<AnimatorStateInfo>();

    public event System.Action startup;
    public bool isStartup { get; private set; }
    public event System.Action active;
    public event System.Action cooldown;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackCalls.Push(stateInfo);
        IsAttacking = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackCalls.Count > 0)
        {
            IsAttacking = true;
            attackCalls.Pop();
        }

        if (attackCalls.Count == 0)
        {
            IsAttacking = false;
            state = AttackState.Done;
        }
    }
}

public enum AttackState
{
    Startup, Active, Cooldown, Done, Transitioning
}

public interface IAttackBehaviorListener
{
    void OnAttackActive();
    void OnAttackCooldown();
}