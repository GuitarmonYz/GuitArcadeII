using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/ChaseEnemy")]
public class PlayerChaseAction : FSM_Action {
	public override void Act(StateController controller) {
		controller.playerController.chaseEnemy();
	}
}
