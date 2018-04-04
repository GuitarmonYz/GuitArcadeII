using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_EnemyController : MonoBehaviour {
	public int health = 100;
	GameObject player;
	wu_PlayerController playerController;
	Rigidbody2D rg2d;
	Vector2 force;
	Animator animator;
	float time;
	private bool faceRight = true;
	private bool can_attack = true;
	private bool attacking = false;
	private float fireRate = 1;
	private float nextFire = 0;
	public GameObject bullet;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<wu_PlayerController>();
		rg2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		time = Time.time;
		rg2d.velocity = new Vector2(3,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (!attacking) Patrol();
		RaycastHit2D hit2D;
		hit2D = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f * (faceRight ? 1 : -1), transform.position.y), transform.right * (faceRight ? 1 : -1), 10);
		Debug.DrawRay(new Vector3(transform.position.x + 0.5f * (faceRight ? 1 : -1), transform.position.y, transform.position.z), transform.right * (faceRight ? 1 : -1), Color.blue);
		if (hit2D != false) {
			// Debug.Log(hit2D.collider.tag);
			if (hit2D.collider.tag == "Player") {
				attacking = true;
				animator.SetBool("Attack", false);
				if (Time.time > nextFire) {
					Attack();
					nextFire = Time.time + fireRate;
				}
			}
		}
	}
	public void Patrol() {
		animator.SetBool("Attack", false);
		if (Time.time > time + 1) {
			Vector2 preVelocity = rg2d.velocity;
			// Debug.Log(preVelocity);
			preVelocity.x *= -1;
			// Debug.Log(preVelocity);
			rg2d.velocity = preVelocity;
			// force.x *= -1;
			time = Time.time;
			Flip();
			// Debug.Log(transform.right);
		}
	}

	public void Attack() {
		animator.SetBool("Attack", true);
		Debug.Log("in attack");
		GameObject newBullet = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
		NinjaBulletController nbc = newBullet.GetComponent<NinjaBulletController>();
		nbc.setVelocity(rg2d.velocity.x*2);
		
	}

	void OnCollisionStay2D(Collision2D other)
	{
		// Debug.Log("enter");
		if (other.gameObject.CompareTag ("Player")) {
			// wu_PlayerController playerController = other.gameObject.GetComponent<wu_PlayerController>();
			if (playerController.getAttackState()){
				health -= 10;
				Debug.Log("take damage");
				rg2d.velocity = new Vector2(0,0);
				animator.SetBool("Hitted", true);
			} 
			if (health <= 0) {
				playerController.setAttackState(false);
				playerController.enemyAlive = false;
				animator.SetBool("Hitted", false);
				this.gameObject.SetActive(false);
			}
		}
		if (other.gameObject.CompareTag("floor")) {
			// Debug.Log(other.gameObject.name);
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
