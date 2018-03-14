using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_EnemyController : MonoBehaviour {
	public int health = 100;
	GameObject player;
	wu_PlayerController playerController;
	Rigidbody2D rg2d;
	Vector2 force;
	float time;
	public bool faceRight;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<wu_PlayerController>();
		rg2d = GetComponent<Rigidbody2D>();
		time = Time.time;
		// rg2d.velocity = new Vector3(4,0,0);
		force = new Vector2(5,0);
		rg2d.AddForce(force);
		Flip();
	}
	
	// Update is called once per frame
	void Update () {
		Patrol();
	}
	public void Patrol() {
		if (Time.time > time + 1) {
			// Vector3 preVelocity = rg2d.velocity;
			// preVelocity.x *= -1;
			// rg2d.velocity = preVelocity;
			force.x *= -1;
			time = Time.time;
			Flip();
		}
		rg2d.AddForce(force);

		// rg2d.velocity = new Vector3(2,0,0);
	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.gameObject.CompareTag ("Player")) {
			// wu_PlayerController playerController = other.gameObject.GetComponent<wu_PlayerController>();
			if (playerController.getAttackState()){
				health -= 10;
			} 
			if (health <= 0) {
				playerController.setAttackState(false);
				this.gameObject.SetActive(false);
			}
		}
	}
	public void Flip() {
		faceRight = !faceRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
