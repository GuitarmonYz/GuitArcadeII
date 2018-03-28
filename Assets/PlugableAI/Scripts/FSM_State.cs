using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class FSM_State : ScriptableObject {
	public FSM_Action[] actions;
	public FSM_Transition[] transitions;
	public void updateState(StateController controller){
		doActions(controller);
		// checkTransitions(controller);
	}
	private void doActions(StateController controller) {
		for (int i = 0; i < actions.Length; i++) {
			actions[i].Act(controller);
		}
	}
	private void checkTransitions(StateController controller) {
		for (int i = 0; i < transitions.Length; i++) {
			if (transitions[i].decision.Decide(controller)) {
				controller.TransitionToState(transitions[i].trueState);
			} else {
				controller.TransitionToState(transitions[i].falseState);
			}
		}
	}
}
