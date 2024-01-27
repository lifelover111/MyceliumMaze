using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStep : StateMachineBehaviour
{
    Player player;
    float stepDuration = 0.5f;
    float timeStepStart;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
            player = animator.gameObject.GetComponentInParent<Player>();
        if(!player._IsControlledByAnimator)
            player._IsControlledByAnimator = true;
        player.OnControlledByAnimator += StepForward;
        timeStepStart = Time.time;
        player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.weapon.enabled = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.weapon.enabled = false;
        player.OnControlledByAnimator -= StepForward;
        player._IsControlledByAnimator = false;
    }

    void StepForward()
    {
        player.transform.position += Vector3.Slerp(Vector3.zero, player.transform.rotation * Vector3.left * Time.deltaTime * player.speedForward * 0.35f, timeStepStart/stepDuration);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
