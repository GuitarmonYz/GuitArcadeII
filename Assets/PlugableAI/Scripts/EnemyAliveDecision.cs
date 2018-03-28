using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/EnemyAlive")]
public class EnemyAliveDecision : FSM_Decision {
	public override bool Decide(StateController controller) {
		return controller.playerController.CurEnemyAlive();
	}
}
