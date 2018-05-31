using UnityEngine;
using System.Collections;

/// <summary>
/// Reset the animator
/// </summary>
public class IdleReset : StateMachineBehaviour {

	public override void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        anim.SetInteger("IdleNo", 0);
    } 
}
