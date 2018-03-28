using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/EnemyInSight")]
public class EnemyInSightDecision : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.EnemyInSight();
	}
}
