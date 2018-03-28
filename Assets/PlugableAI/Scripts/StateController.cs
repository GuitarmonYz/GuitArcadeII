using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {


	public FSM_State curState;
	public GameObject rayEye;
	[HideInInspector] public wu_PlayerController playerController;
	[HideInInspector] public GameObject chaseTarget;
	public FSM_State remainState;
	private bool aiActive;


	void Awake () 
	{
		
	}

	public void SetupAI(wu_PlayerController playerController)
	{
		this.playerController = playerController;
		
	}
	
	void Update()
	{
		
		if (Input.GetKey("f")) {
			curState.updateState(this);
		} else {
			playerController.Patrol();
		}
		
	}
	public void TransitionToState(FSM_State nextState) {
		if (nextState != remainState) {
			curState = nextState;
		}
	}

}