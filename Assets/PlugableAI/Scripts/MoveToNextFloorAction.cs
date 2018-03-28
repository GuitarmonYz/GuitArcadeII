using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/MoveToNextFloor")]
public class MoveToNextFloorAction : FSM_Action {

	public override void Act(StateController controller) {
		controller.playerController.moveToNextStage();
	}
}
