using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Behaviour Listener/Attack")]
public class AttackBehaviourRelay : StateBehaviourListener
{
    public event System.Action active;
    public event System.Action cooldown;

    public bool isAttacking { get; private set; }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        isAttacking = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        isAttacking = false;
    }

    public void OnAttackActive()
    {
    }

    public void OnAttackCooldown()
    {
    }
}
