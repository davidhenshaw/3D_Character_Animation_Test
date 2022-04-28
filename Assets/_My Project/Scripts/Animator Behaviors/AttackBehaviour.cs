using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public bool IsAttacking { get; set; }
    public AttackState state { get; private set; } = AttackState.None;
    private Stack<AnimatorStateInfo> attackCalls = new Stack<AnimatorStateInfo>();

    public event System.Action startup;
    public bool isStartup { get; private set; }
    public event System.Action active;
    public event System.Action cooldown;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackCalls.Push(stateInfo);
        IsAttacking = true;
        //AttackStartup();
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
            state = AttackState.None;
        }
    }
}

public enum AttackState
{
    Startup, Active, Cooldown, None
}

public interface IAttackBehaviorListener
{
    void OnAttackActive();
    void OnAttackCooldown();
}