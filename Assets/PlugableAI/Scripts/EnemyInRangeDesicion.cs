using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/EnemyInRange")]
public class EnemyInRangeDesicion : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.EnemyInRange();
	}
}
