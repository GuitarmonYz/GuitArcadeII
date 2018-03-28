using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_PlayerController : MonoBehaviour {
	Animator animator;
	Rigidbody2D rigidbody;
	bool faceRight = true;
	int curFloor = 0;
	public float speed = 2f;
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
		// faceRight = !faceRight;
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

	public void moveToNextStage() {
		if (curFloor%2 == 0) {
			faceRight = true;
			if (rigidbody.velocity.x <= 0){
				rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
				animator.SetFloat("walk", rigidbody.velocity.x);
			}
		} else {
			faceRight = false;
			if (rigidbody.velocity.x >= 0) {
				rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
			}
		}
	}

	public Rigidbody2D GetRigidbody(){
		return this.rigidbody;
	}

	public bool getAttackState(){
		return animator.GetBool("attack");
	}
	public void setAttackState(bool val){
		animator.SetBool("attack", val);
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			int.TryParse(other.gameObject.name.Split('_')[1], out curFloor);
			if (curFloor != 0) Flip();
		}
	}
}
