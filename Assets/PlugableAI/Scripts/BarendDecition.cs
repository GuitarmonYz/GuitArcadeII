using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/Barend")]
public class BarendDecition : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.barEnd();
	}
}
