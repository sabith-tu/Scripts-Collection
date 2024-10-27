using System;
using RenownedGames.AITree;
using SABI;
using UnityEngine;

[NodeContent("IsClose", "Custom/IsClose")]
public class IsClose : ObserverDecorator
{
    public override event Action OnValueChange;

    Transform ownerTransform;
    public float minDistance = 10;
    public bool ignorePositionY;
    public TransformKey target;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        ownerTransform = GetOwner().transform;
    }

    protected override void OnFlowUpdate()
    {
        base.OnFlowUpdate();
        OnValueChange?.Invoke();
    }

    public override bool CalculateResult()
    {
        if (target.GetValue() == null)
            return false;
        float distance = ignorePositionY
            ? Vector3.Distance(
                ownerTransform.position.WithY(0),
                target.GetValue().position.WithY(0)
            )
            : Vector3.Distance(ownerTransform.position, target.GetValue().position);
        return distance < minDistance;
    }
}
