using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wu_PlayerController : MonoBehaviour {
	public Image content; // healthbar
	public GameObject _camera; // moving camera
	public float speed = 1f; // moving speed
	[HideInInspector] public Vector2 curEnemyPos;
	[HideInInspector] public bool enemyAlive = false;
	
	private Animator animator;
	private Rigidbody2D rigidbody;
	private GameObject rayEye; // for enemy raycasting
	private SpriteRenderer spriteRenderer;
	private wu_CameraController cameraController; // access camera controller script
	private wu_MusicAnalysis analyser; // accessing music analysiser
	private bool faceRight = true; // for facing direction
	private int preFloor = 0; // for screen scrolling
	private int curFloor = 0; // indicate current floor
	private int nextFloor = 1; // indicate whether jumped to next stage
	private float nextPatrolFlipTime = 0;
	private bool grounded = false; // whether grounded
	private int tickCount = 0; // for tracing bar position
	private int health = 100; // health value
	private Dictionary<int, string> chordList; //chord list map
	private bool takingDamage = false;
	private float damageTime = 0;
	private float attackSpeed = 0.5f;
	private bool curBarSetted = false;
	void Start () {
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
		rayEye = transform.GetChild(0).gameObject;
		cameraController = _camera.GetComponent<wu_CameraController>();
		analyser = GetComponent<wu_MusicAnalysis>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		Vector2 cur_position = new Vector2(transform.position.x, transform.position.y);
		Vector2 target = new Vector2(-3.45f, -0.19f);
		nextPatrolFlipTime = Time.time + 1;
		chordList = new Dictionary<int, string>(){
			{0, "Dm"}, {1, "G"}, {2, "C"}, {3, "C"}
		};
	}
	
	void Update () {
		content.fillAmount = health/100.0f;
		if (takingDamage) {
			if (Time.time < damageTime) {
				spriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 3, 1.0f));
			} else {
				takingDamage = false;
			}
		} else if (spriteRenderer.color != Color.white) {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, 0.1f);
		}
		if (Input.GetKey("o")) {
			animator.speed = 0.5f;
			attackSpeed = 0.75f;
		} else {
			animator.speed = 1;
			attackSpeed = 0.5f;
		}
		if (barBegin()) {
			if (!curBarSetted) {
				analyser.setBarBegin(Time.time);
				curBarSetted = true;
			}
		}
	}

	public void Idle() {
		rigidbody.velocity = new Vector2(0,0);
	}
	
	public void Patrol(){
		if (!grounded) return;
		animator.SetBool("attack", false);
		animator.SetBool("jump", false);
		rigidbody.velocity = new Vector2((faceRight ? 1 : -1), rigidbody.velocity.y);
		if (Time.time > nextPatrolFlipTime) {
			rigidbody.velocity = new Vector2((faceRight? -1 : 1), rigidbody.velocity.y);
			Flip();
			animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			nextPatrolFlipTime = Time.time + 1;
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
			// Debug.Log("jump to next stage");
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
			// Debug.Log(x_distance);
			float y_offset = start.y - target.y;
			float initVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(x_distance, 2)) / (x_distance * Mathf.Tan(angle) + y_offset));
			Vector2 finalVelocity = new Vector2(initVelocity * Mathf.Sin(angle) * (start.x > target.x ? -1 : 1), initVelocity * Mathf.Cos(angle));
			rigidbody.velocity = finalVelocity;
			animator.SetBool("jump",true);
		} else {
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
			}
			if (rigidbody.velocity.x <= 0.5f){
				rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
				animator.SetFloat("walk", Mathf.Abs(rigidbody.velocity.x));
			}
		} else {
			if (faceRight) {
				Flip();
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

	public void takeDamage() {
		damageTime = Time.time + 1;
		takingDamage = true;
		health -= 5;
	}

	public void Attack() {
		animator.SetBool("attack", true);
	}

	public bool CurEnemyAlive() {
		return enemyAlive;
	}
	
	public bool barBegin() {
		if (tickCount % 8 == 1) {
			return true;
		} else {
			curBarSetted = false;
			return false;
		}
	}

	public bool barEnd() {
		if (tickCount%8 == 0 && tickCount != 0) {
			return true;
		} else {
			return false;
		}
	}
	public bool ifNextFloor(){
		if (nextFloor == curFloor && grounded) {
			nextFloor = curFloor+1;
			return true;
		} else {
			return false;
		}
	}
	public bool ifGrounded() {
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
	}

	public bool checkCorretness() {
		int curChordPos = curFloor%4;
		string curChord = chordList[curChordPos];
		return analyser.checkChordCorrect(curChord);
	}

	public float getAttackSpeed() {
		return this.attackSpeed;
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			grounded = true;
			int.TryParse(other.gameObject.name.Split('_')[1], out curFloor);
			if (curFloor != 0) Flip();
			if (curFloor != 0 && curFloor%2==0) {
				if (curFloor != preFloor) {
					cameraController.turnOnScroll();
					preFloor = curFloor;
				}
			}
		}
	}
	
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			grounded = false;
		}
	}
}
