using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseAction : FSM_Action {

	// Use this for initialization
	public override void Act(StateController controller) {
		chaseEnemy(controller);
	}

	private void chaseEnemy(StateController controller) {
		// controller.playerController
		controller.playerController.GetRigidbody().velocity *= 2;
	}
}
