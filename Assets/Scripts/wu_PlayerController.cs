using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_PlayerController : MonoBehaviour {
	Animator animator;
	Rigidbody2D rigidbody;
	bool faceRight = true;
	int curFloor = 0;
	public float speed = 1f;
	public Vector2 curEnemyPos;
	private GameObject rayEye;
	[HideInInspector] public bool enemyAlive = false;
	private float lastTime;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
		rayEye = transform.GetChild(0).gameObject;
		Vector2 cur_position = new Vector2(transform.position.x, transform.position.y);
		Vector2 target = new Vector2(-3.45f, -0.19f);
		// jumpToNextStage(cur_position, target, 60f);
	}
	
	// Update is called once per frame
	void Update () {
		// animator.SetFloat("walk", rigidbody.velocity.x);
	}
	
	public void Patrol(){
		if (Time.time > lastTime + 1) {
			if (rigidbody.velocity.x <= 0) {
				rigidbody.velocity = new Vector2(1, 0);
				Flip();
			} else {
				rigidbody.velocity = new Vector2(-1, 0);
				Flip();
			}
			animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			lastTime = Time.time;
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
		setAttackState(false);
		Debug.Log(rigidbody.velocity.x);
		if (curFloor%2 == 0) {
			faceRight = true;
			if (rigidbody.velocity.x <= 0.5f){
				rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			}
		} else {
			faceRight = false;
			if (rigidbody.velocity.x >= -0.5f) {
				rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
			}
		}
	}

	public void chaseEnemy() {
		if (curFloor%2 == 0) {
			if (curEnemyPos.x >= transform.position.x){
				rigidbody.velocity = new Vector2(speed * 2, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			} else {
				rigidbody.velocity = new Vector2(speed * -2, rigidbody.velocity.y);
			}
		} else {
			if (curEnemyPos.x >= transform.position.x) {
				rigidbody.velocity = new Vector2(speed * 2, rigidbody.velocity.y);
			} else {
				rigidbody.velocity = new Vector2(speed * -2, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			}
		}
	}

	public bool EnemyInSight() {
		RaycastHit2D hit2D;
		Vector3 rayEyePos = rayEye.transform.position;
		bool findRes = false;
		enemyAlive = true;
		hit2D = Physics2D.Raycast(new Vector2(rayEyePos.x , rayEyePos.y), rayEye.transform.right * (faceRight ? 1 : -1), 3);
		Debug.DrawRay(new Vector3(rayEyePos.x , rayEyePos.y, rayEyePos.z) , rayEye.transform.right * (faceRight ? 1 : -1), Color.red);
		if (hit2D != false && hit2D.collider.tag == "enemy") {
			findRes = true;
			curEnemyPos = hit2D.collider.gameObject.transform.position;
		}
		return findRes;
	}

	public bool EnemyInRange() {
		RaycastHit2D hit2D;
		Vector3 rayEyePos = rayEye.transform.position;
		bool findRes = false;
		hit2D = Physics2D.Raycast(new Vector2(rayEyePos.x , rayEyePos.y), rayEye.transform.right * (faceRight ? 1 : -1), 5);
		Debug.DrawRay(new Vector3(rayEyePos.x , rayEyePos.y, rayEyePos.z) , rayEye.transform.right * (faceRight ? 1 : -1), Color.red);
		if (hit2D != false && hit2D.collider.tag == "enemy") {
			curEnemyPos = hit2D.collider.gameObject.transform.position;
			if (Mathf.Abs(curEnemyPos.x - transform.position.x) < 2f) {
				findRes = true;
			}
		}
		return findRes;
	}

	public void Attack() {
		animator.SetBool("attack", true);
		// rigidbody.velocity = new Vector2(0,0);
	}

	public bool CurEnemyAlive() {
		// animator.SetBool("attack", false);
		// Debug.Log(enemyAlive);
		return enemyAlive;
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
