using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM_Action : ScriptableObject 
{
	public abstract void Act(StateController controller);
	
}
