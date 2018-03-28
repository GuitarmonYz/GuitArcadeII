using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName="PluggableAI/Actions/AttackEnemy")]
public class AttackEnemyAction : FSM_Action {
	public override void Act(StateController controller) {
		controller.playerController.Attack();
	}
}
