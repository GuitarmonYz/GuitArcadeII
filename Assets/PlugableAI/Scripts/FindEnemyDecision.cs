using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/findEnemy")]
public class FindEnemyDecision : FSM_Decision {
	public override bool Decide(StateController controller) {
		return findEnemy(controller);
	}
	private bool findEnemy(StateController controller) {
		RaycastHit2D hit2D;
		Vector3 rayEyePos = controller.rayEye.transform.position;
		bool findRes = false;
		hit2D = Physics2D.Raycast(new Vector2(rayEyePos.x , rayEyePos.y), controller.rayEye.transform.right, 10);
		Debug.DrawRay(new Vector3(rayEyePos.x, rayEyePos.y, rayEyePos.z), controller.rayEye.transform.right, Color.blue);
		if (hit2D != false && hit2D.collider.tag == "enemy") {
			findRes = true;
		}
		return findRes;
	}
	
}
