using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimatorController : MonoBehaviour {
	private Animator animator;
	public GameObject fireBall;
	public GameObject fireMarker;
	public GameObject Player;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	public void fire(){
		GameObject new_fireBall = Instantiate(fireBall, fireMarker.transform);
		fireballController fc = new_fireBall.GetComponent<fireballController>();
		fc.setPlayerLocation(Player.transform.position);
	}

	public void holdPrepare(){
		animator.SetTrigger("PreFireHold");
	}
	public void backIdel(){
		animator.SetTrigger("Idel");
	}
}
