using RenownedGames.AITree;
using UnityEngine;

[NodeContent(
    "CommonAnim",
    "Tasks/Custom/PlayCommonAnimations",
    IconPath = "Images/Icons/Node/CrossFadeIcon.png"
)]
public class PlayCommonAnimations : TaskNode
{
    public bool returnRunningState;

    public enum CommonAnimations
    {
        Idle,
        Spawn,
        Walk,
        Run,
        Sprint,
        Attack_Melay,
        Attack_Range,
        Hit,
        Death,
    }

    public CommonAnimations animation;

    Animator animator;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        animator = GetOwner().GetComponent<Animator>();
    }

    protected override State OnUpdate()
    {
        if (animator == null)
            return State.Failure;

        animator.CrossFade(animation.ToString(), 0.25f);
        return returnRunningState ? State.Running : State.Success;
    }

    public override string GetDescription()
    {
        return $"Cross Fade: {animation}";
    }
}
