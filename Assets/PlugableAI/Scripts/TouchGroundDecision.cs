using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Grounded")]
public class TouchGroundDecision : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.ifNextFloor();
	}
}
