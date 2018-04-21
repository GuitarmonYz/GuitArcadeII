using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/BehaveRight")]
public class BehaveRightDecision : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.checkCorretness();
		// return Input.GetKey("f");
	}
}
