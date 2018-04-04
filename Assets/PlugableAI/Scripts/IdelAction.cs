using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/Idle")]
public class IdleAction : FSM_Action {
	public override void Act(StateController controller) {
		controller.playerController.Idle();	
	}
}
