using RenownedGames.AITree;
using UnityEngine;

[NodeContent("Instantiate", "Tasks/Custom/Instantiate")]
public class Instantiate : TaskNode
{
    public GameObject gameObjectToSapwn;
    public Vector3 positionOffset,
        rotationOffset;
    Transform ownerTransform;

    public bool useOwnerForwardAsRotation;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        ownerTransform = GetOwner().transform;
    }

    protected override State OnUpdate()
    {
        Vector3 relativePositionOffset =
            ownerTransform.right * positionOffset.x
            + ownerTransform.up * positionOffset.y
            + ownerTransform.forward * positionOffset.z;
        Instantiate(
            gameObjectToSapwn,
            ownerTransform.position + relativePositionOffset,
            Quaternion.Euler(
                (useOwnerForwardAsRotation ? ownerTransform.forward : ownerTransform.eulerAngles)
                    + rotationOffset
            )
        );
        return State.Success;
    }

    public override void OnDrawGizmosNodeSelected()
    {
        base.OnDrawGizmosNodeSelected();
        Vector3 relativePositionOffset =
            ownerTransform.right * positionOffset.x
            + ownerTransform.up * positionOffset.y
            + ownerTransform.forward * positionOffset.z;
        Gizmos.DrawSphere(ownerTransform.position + relativePositionOffset, 0.5f);
    }
}
