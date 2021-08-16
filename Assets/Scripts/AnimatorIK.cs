using UnityEngine;

public class AnimatorIK : StateMachineBehaviour
{
    private BodyReferences _bodyRefs;

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_bodyRefs == null)
        {
            _bodyRefs = animator.GetComponentInParent<BodyReferences>();
        }

        animator.SetLookAtWeight(1f);
        animator.SetLookAtPosition(_bodyRefs.headRef.transform.forward);

        SetIKPositionAndRotation(animator, AvatarIKGoal.LeftHand, _bodyRefs.leftHandRef);
        SetIKPositionAndRotation(animator, AvatarIKGoal.RightHand, _bodyRefs.rightHandRef);
    }

    private void SetIKPositionAndRotation(Animator anim, AvatarIKGoal goal, GameObject target)
    {
        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKPosition(goal, target.transform.position);
        anim.SetIKRotation(goal, target.transform.rotation);
    }
}
