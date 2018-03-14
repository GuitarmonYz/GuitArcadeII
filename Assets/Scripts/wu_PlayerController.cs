using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_PlayerController : MonoBehaviour {
	Animator animator;
	Rigidbody2D rigidbody;
	bool faceRight = true;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetFloat("walk", rigidbody.velocity.x);
	}
	public void Attack(ArrayList enemysPoses) {
		// rigidbody.velocity = new Vector2(0,0);
		Vector3 curPos = transform.position;
		foreach(Vector3 enemysPos in enemysPoses) {
			if (Mathf.Abs(enemysPos.x - curPos.x) < 0.5 && Mathf.Abs(enemysPos.y - curPos.y) < 0.3) {
				if (!animator.GetBool("attack"))
					animator.SetBool("attack", true);
				break;
			} else {
				animator.SetBool("attack", false);
			}
		}
	}
	public void Patrol(){
		if (!animator.GetBool("attack")) {
			if (faceRight) {
				rigidbody.velocity = new Vector2(2, 0);
			} else {
				rigidbody.velocity = new Vector2(-2, 0);
			}
		}
	}
	public void Flip() {
		faceRight = !faceRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public bool getAttackState(){
		return animator.GetBool("attack");
	}
	public void setAttackState(bool val){
		animator.SetBool("attack", val);
	}
}
