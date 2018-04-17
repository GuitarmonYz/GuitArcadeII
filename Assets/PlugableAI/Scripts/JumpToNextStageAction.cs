using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/JumpToNextFloor")]
public class JumpToNextStageAction : FSM_Action {
	public override void Act(StateController controller) {
		if (controller.playerController.ifGrounded()) {
			controller.playerController.jumpToNextStage();
		}
	}
}
