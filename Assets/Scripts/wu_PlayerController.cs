using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_PlayerController : MonoBehaviour {
	Animator animator;
	Rigidbody2D rigidbody;
	bool faceRight = true;
	int curFloor = 0;
	int nextFloor = 1;
	public float speed = 1f;
	public Vector2 curEnemyPos;
	private GameObject rayEye;
	[HideInInspector] public bool enemyAlive = false;
	private float nextTime = 0;
	private bool grounded = false;
	private int tickCount = 0;
	private int health = 100;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
		rayEye = transform.GetChild(0).gameObject;
		Vector2 cur_position = new Vector2(transform.position.x, transform.position.y);
		Vector2 target = new Vector2(-3.45f, -0.19f);
		nextTime = Time.time + 1;
		// jumpToNextStage(cur_position, target, 60f);
	}
	
	// Update is called once per frame
	void Update () {
		// animator.SetFloat("walk", rigidbody.velocity.x);
	}

	public void Idle() {
		rigidbody.velocity = new Vector2(0,0);
	}
	
	public void Patrol(){
		animator.SetBool("attack", false);
		animator.SetBool("jump", false);
		rigidbody.velocity = new Vector2((faceRight ? 1 : -1), rigidbody.velocity.y);
		if (Time.time > nextTime) {
			rigidbody.velocity = new Vector2((faceRight? -1 : 1), rigidbody.velocity.y);
			Flip();
			animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			nextTime = Time.time + 1;
		}
	}
	public void Flip() {
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		if (theScale.x > 0) {
			faceRight = true;
		} else {
			faceRight = false;
		}
	}

	public void jumpToNextStage() {
		if (nextFloor != curFloor) {
			Debug.Log("jump to next stage");
			Vector2 start = new Vector2(transform.position.x, transform.position.y);
			Vector2 target;
			if (curFloor%2 == 0) {
				target = new Vector2(6f, transform.position.y - 2.9f);
			} else {
				target = new Vector2(-6f, transform.position.y - 2.9f);
			}
			float initAngle = 50;
			float gravity = Physics2D.gravity.magnitude;
			float angle = initAngle * Mathf.Deg2Rad;
			float x_distance = Mathf.Abs(start.x - target.x);
			float y_offset = start.y - target.y;
			float initVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(x_distance, 2)) / (x_distance * Mathf.Tan(angle) + y_offset));
			Vector2 finalVelocity = new Vector2(initVelocity * Mathf.Sin(angle) * (start.x > target.x ? -1 : 1), initVelocity * Mathf.Cos(angle));
			rigidbody.velocity = finalVelocity;
			animator.SetBool("jump",true);
		} else {
			nextFloor++;
			animator.SetBool("jump", false);
		}
		
	}

	public void moveToNextStage() {
		setAttackState(false);
		animator.SetBool("jump", false);
		// Debug.Log(rigidbody.velocity.x);
		if (curFloor%2 == 0) {
			if (!faceRight) {
				Flip();
				// faceRight = true;
			}
			if (rigidbody.velocity.x <= 0.5f){
				rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			}
		} else {
			if (faceRight) {
				Flip();
				// faceRight = false;
			}
			if (rigidbody.velocity.x >= -0.5f) {
				rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
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

	public void takeDamage(){
		health -= 10;
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
	
	public bool barEnd() {
		if (tickCount%8 == 0 && tickCount != 0) {
			Debug.Log("barEnd");
			return true;
		} else {
			return false;
		}
	}
	public bool ifGrounded(){
		return grounded;
	}

	public bool getAttackState(){
		return animator.GetBool("attack");
	}
	public void setAttackState(bool val){
		animator.SetBool("attack", val);
	}
	public void incrementTick(){
		tickCount++;
		// Debug.Log("tick!!");
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			grounded = true;
			int.TryParse(other.gameObject.name.Split('_')[1], out curFloor);
			if (curFloor != 0) Flip();
		}
	}
	
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			grounded = false;
		}
	}
}
