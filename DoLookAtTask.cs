using DG.Tweening;
using NUnit.Framework;
using RenownedGames.AITree;
using UnityEngine;

[NodeContent("DoLookAt", "Tasks/Custom/DoLookAtTask")]
public class DoLookAtTask : TaskNode
{
    Transform ownerTransform;
    public Key lookAtTarget;
    public float duration = 0.25f;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        ownerTransform = GetOwner().transform;
    }

    bool isRotationCompleted = false;

    protected override State OnUpdate()
    {
        Vector3 lookAtTargetPossition = Vector3.zero;

        if (lookAtTarget is TransformKey transformKey)
            lookAtTargetPossition = transformKey.GetValue().position;

        if (lookAtTarget is Vector3Key vector3Key)
            lookAtTargetPossition = vector3Key.GetValue();

        ownerTransform
            .DOLookAt(lookAtTargetPossition, duration)
            .OnComplete(() => isRotationCompleted = true);

        if (isRotationCompleted)
        {
            isRotationCompleted = false;
            return State.Success;
        }

        return State.Running;
    }
}
