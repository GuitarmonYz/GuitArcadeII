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
		rg2d.velocity = new Vector2(3,0);
		Flip();
	}
	
	// Update is called once per frame
	void Update () {
		Patrol();
		RaycastHit2D hit2D;
		hit2D = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), transform.right, 10);
		Debug.DrawRay(new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.right, Color.blue);
		if (hit2D != false) {
			Debug.Log(hit2D.collider.tag);
		}
	}
	public void Patrol() {
		// Debug.Log(rg2d.velocity);
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
	void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log("enter");
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
		if (other.gameObject.CompareTag("floor")) {
			Debug.Log(other.gameObject.name);
		}
	}
	public void Flip() {
		faceRight = !faceRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
