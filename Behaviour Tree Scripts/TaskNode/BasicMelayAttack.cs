using System.Runtime.CompilerServices;
using DG.Tweening;
using RenownedGames.AITree;
using SABI;
using UnityEngine;
using UnityEngine.AI;

[NodeContent("BasicMelayAttack", "Tasks/Custom/BasicMelayAttack")]
public class BasicMelayAttack : TaskNode
{
    public TransformKey target;
    NavMeshAgent agent;
    Animator animator;
    Transform selfTransform;
    public float minDitanceToAttack = 3,
        delayBetweenAttacks = 1;
    string animState_attackAnimation = "Attack_Melay",
        animState_moveAnimation = "IdleAndWalk";

    float attackTimeLeft;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        agent = GetOwner().GetComponent<NavMeshAgent>();
        selfTransform = GetOwner().transform;
        animator = GetOwner().GetComponent<Animator>();
    }

    protected override State OnUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        Debug.Log(
            $"[SAB] agent.speed:{agent.velocity.magnitude} animator.GetFloat(Speed){animator.GetFloat("Speed")}"
        );
        if (target.GetValue() == null)
            return State.Running;
        Vector3 targetPosition = target.GetValue().position;

        Debug.Log($"[SAB] distance: {Vector3.Distance(targetPosition, selfTransform.position)}");

        if (Vector3.Distance(targetPosition, selfTransform.position) > minDitanceToAttack)
        {
            agent.SetDestination(targetPosition);
            animator.SetTrigger(animState_moveAnimation);
            Debug.Log($"[SAB] animator.SetTrigger(animState_moveAnimation);");
        }
        else
        {
            agent.ResetPath();

            if (attackTimeLeft > 0)
            {
                attackTimeLeft -= Time.deltaTime;
                return State.Running;
            }
            attackTimeLeft = delayBetweenAttacks;
            selfTransform.DOLookAt(targetPosition.WithY(selfTransform.position.y), 0.2f);
            animator.SetTrigger(animState_attackAnimation);
            Debug.Log($"[SAB] animator.SetTrigger(animState_attackAnimation);");
        }

        return State.Running;
    }
}
