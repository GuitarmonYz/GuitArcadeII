using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {
	public FSM_State curState;
	[HideInInspector] public wu_PlayerController playerController;
	public FSM_State remainState;
	private bool aiActive;

	public void SetupAI(wu_PlayerController playerController)
	{
		this.playerController = playerController;
	}
	
	void Update()
	{
		curState.updateState(this);
	}
	public void TransitionToState(FSM_State nextState) {
		if (nextState != remainState) {
			curState = nextState;
		} 
	}

}