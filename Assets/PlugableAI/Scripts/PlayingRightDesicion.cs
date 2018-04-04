using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/PlayingRight")]
public class PlayingRightDecition : FSM_Decision {
	public override bool Decide(StateController controller) {
		return Input.GetKey("f");
	}
}
