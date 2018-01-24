using UnityEngine;
using System.Collections;

public class IdleReset : StateMachineBehaviour {

	public override void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
    {
        anim.SetInteger("IdleNo", 0);
    } 
}
