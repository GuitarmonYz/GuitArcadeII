using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FSM_Transition {
	public FSM_Decision decision;
	public FSM_State trueState;
	public FSM_State falseState;
	
}
