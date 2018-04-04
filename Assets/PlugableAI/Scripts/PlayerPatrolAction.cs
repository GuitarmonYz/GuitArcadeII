using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/PlayerPatrol")]
public class PlayerPatrolAction : FSM_Action {
	public override void Act(StateController controller) {
		controller.playerController.Patrol();	
	}
}
