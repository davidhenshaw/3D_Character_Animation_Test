using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StateBehaviourListener : ScriptableObject
{
    public event System.Action<StateBehaviourInfo> OnEnter;
    public event System.Action<StateBehaviourInfo> OnExit;

    public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var info = new StateBehaviourInfo(animator, stateInfo, layerIndex);
        OnEnter?.Invoke(info);
    }

    public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var info = new StateBehaviourInfo(animator, stateInfo, layerIndex);
        OnExit?.Invoke(info);
    }
}

public struct StateBehaviourInfo
{
    public Animator animator;
    public AnimatorStateInfo stateInfo;
    public int layerIndex;

    public StateBehaviourInfo(Animator anim, AnimatorStateInfo info, int index)
    {
        animator = anim;
        stateInfo = info;
        layerIndex = index;
    }
}
