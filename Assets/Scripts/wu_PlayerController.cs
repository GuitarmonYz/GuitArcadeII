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
		Vector2 cur_position = new Vector2(transform.position.x, transform.position.y);
		Vector2 target = new Vector2(-3.45f, -0.19f);
		// jumpToNextStage(cur_position, target, 60f);
	}
	
	// Update is called once per frame
	void Update () {
		// animator.SetFloat("walk", rigidbody.velocity.x);
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
				rigidbody.velocity = new Vector2(2, rigidbody.velocity.y);
			} else {
				rigidbody.velocity = new Vector2(-2, rigidbody.velocity.y);
			}
		}
	}
	public void Flip() {
		faceRight = !faceRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void jumpToNextStage(Vector2 start, Vector2 target, float initAngle) {
		float gravity = Physics2D.gravity.magnitude;
		float angle = initAngle * Mathf.Deg2Rad;
		float x_distance = Mathf.Abs(start.x - target.x);
		float y_offset = start.y - target.y;
		float initVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(x_distance, 2)) / (x_distance * Mathf.Tan(angle) + y_offset));
		Vector2 finalVelocity = new Vector2(initVelocity * Mathf.Sin(angle) * (start.x > target.x ? -1 : 1), initVelocity * Mathf.Cos(angle));
		rigidbody.velocity = finalVelocity;
	}

	public bool getAttackState(){
		return animator.GetBool("attack");
	}
	public void setAttackState(bool val){
		animator.SetBool("attack", val);
	}
}
