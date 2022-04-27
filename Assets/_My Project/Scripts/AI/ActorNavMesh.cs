using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActorNavMesh : MonoBehaviour
{
    public Transform movePositionTransform;
    [SerializeField]
    private BehaviorTree _tree;

    public float JumpForce = 40;
    public float StopDistance = 2;

    NavMeshAgent _navMeshAgent;
    Rigidbody _rb;

    // Start is called before the first frame update
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();

        _tree = new BehaviorTreeBuilder(gameObject)
        .Sequence()
            .Condition("Custom Condition", () => {
                return !ReachedDestination();
            })
            .Do("Custom Action", () => {
                _navMeshAgent.destination = movePositionTransform.position;
                return TaskStatus.Success;
            })
        .End()
        .Build();
        }

    // Update is called once per frame
    void Update()
    {
        _tree.Tick();
    }

    bool ReachedDestination()
    {
        return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
    }

    void Jump()
    {
        var jumpVector = Vector3.up * JumpForce;
        _rb.AddForce(jumpVector, ForceMode.Impulse);
    }
}
