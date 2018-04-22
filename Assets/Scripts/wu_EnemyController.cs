using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_EnemyController : MonoBehaviour {
	public int health = 100;
	public float reduceRate = 0.5f;
	public GameObject bullet;
	private GameObject player;
	private wu_PlayerController playerController;
	private Rigidbody2D rg2d;
	private Vector2 force;
	private Animator animator;
	private float time;
	private float hitTime = 0;
	private bool faceRight = true;
	private bool can_attack = true;
	private bool attacking = false;
	private float fireRate = 1;
	private float nextFire = 0;
	private Vector3 curPlayerPos;
	private Collider2D _collider2D;
	private bool destroyed = false;
	private float explodeTime = 0;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<wu_PlayerController>();
		rg2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		_collider2D = GetComponent<Collider2D>();
		time = Time.time;
		rg2d.velocity = new Vector2(3,0);
	}
	
	void Update () {
		if (!destroyed) {
			if (!attacking) Patrol();
			RaycastHit2D hit2D;
			hit2D = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f * (faceRight ? 1 : -1), transform.position.y), transform.right * (faceRight ? 1 : -1), 10);
			Debug.DrawRay(new Vector3(transform.position.x + 0.5f * (faceRight ? 1 : -1), transform.position.y, transform.position.z), transform.right * (faceRight ? 1 : -1), Color.blue);
			if (hit2D != false) {
				if (hit2D.collider.tag == "Player" || hit2D.collider.tag == "shield") {
					attacking = true;
					animator.SetBool("Attack", false);
					curPlayerPos = hit2D.collider.gameObject.transform.position;
					if (Time.time > nextFire && can_attack) {
						Attack();
						nextFire = Time.time + fireRate;
					}
				}
			}
		} else if (Time.time > explodeTime + 1) {
			Destroy(this.gameObject);
		}
	}
	public void Patrol() {
		animator.SetBool("Attack", false);
		if (Time.time > time + 1) {
			Vector2 preVelocity = rg2d.velocity;
			preVelocity.x *= -1;
			rg2d.velocity = preVelocity;
			time = Time.time;
			Flip();
		}
	}

	public void Attack() {
		animator.SetBool("Attack", true);
		rg2d.velocity = new Vector2(rg2d.velocity.x * 0.1f, rg2d.velocity.y);
		if (Mathf.Abs(transform.position.x + 1 * 1 * ((transform.position.x > curPlayerPos.x) ? -1 : 1) - curPlayerPos.x) > 0.75f) {
			GameObject newBullet = Instantiate(bullet, new Vector3(transform.position.x + 1 * ((transform.position.x > curPlayerPos.x) ? -1 : 1), transform.position.y, transform.position.z), Quaternion.identity);
			NinjaBulletController nbc = newBullet.GetComponent<NinjaBulletController>();
			if (transform.position.x > curPlayerPos.x) {
				nbc.setVelocity(-2);
			} else {
				nbc.setVelocity(2);
			}
		}
	}

	public void selfDestroy() {
		if (this.gameObject.activeSelf) {
			animator.SetBool("Explode", true);
			playerController.takeDamage(5);
			// Debug.Log("take damage");
			_collider2D.enabled = false;
			rg2d.bodyType = RigidbodyType2D.Kinematic;
			destroyed = true;
			explodeTime = Time.time;
		}
	}

	void OnCollisionStay2D(Collision2D other)
	{
		if (other.gameObject.CompareTag ("Player")) {
			if (playerController.getAttackState()){
				if (Time.time > hitTime) {
					health -= 30;
					// Debug.Log("reduce health");
					hitTime = Time.time + playerController.getAttackSpeed();
				}
				rg2d.velocity = new Vector2(0,0);
				animator.SetBool("Hitted", true);
				can_attack = false;
			}
			if (health <= 0) {
				playerController.setAttackState(false);
				playerController.enemyAlive = false;
				animator.SetBool("Hitted", false);
				this.gameObject.SetActive(false);
			}
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
}
