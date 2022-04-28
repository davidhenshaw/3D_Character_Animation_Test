using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public StateBehaviourListener listenerSO;
    public bool IsAttacking { get; set; }
    private Stack<AnimatorStateInfo> attackCalls = new Stack<AnimatorStateInfo>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        listenerSO.OnStateEnter(animator, stateInfo, layerIndex);
        attackCalls.Push(stateInfo);
        IsAttacking = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        listenerSO.OnStateExit(animator, stateInfo, layerIndex);

        if (attackCalls.Count > 0)
        {
            IsAttacking = true;
            attackCalls.Pop();
        }

        if (attackCalls.Count == 0)
            IsAttacking = false;

    }

    public void OnActive()
    {
        if (!(listenerSO is IAttackBehaviorListener attackListener))
            return;

        attackListener.OnAttackActive();
    }

    public void OnCooldown()
    {
        if (!(listenerSO is IAttackBehaviorListener attackListener))
            return;

        attackListener.OnAttackCooldown();
    }
}

public interface IAttackBehaviorListener
{
    void OnAttackActive();
    void OnAttackCooldown();
}