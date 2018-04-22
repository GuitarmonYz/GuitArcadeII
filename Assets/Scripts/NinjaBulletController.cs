using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaBulletController : MonoBehaviour {
	private float rotation_speed = 60;
	private Rigidbody2D rg2D;
	private GameObject player;
	private wu_PlayerController playerController;
	void Awake()
	{
		rg2D = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Start()
	{
		playerController = player.GetComponent<wu_PlayerController>();
	}
	void Update () {
		rg2D.MoveRotation(rg2D.rotation + rotation_speed * Time.fixedDeltaTime);
		if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 2) {
			if (playerController.checkCorretness()) {
				playerController.openShield();
			}
		}
	}
	public void setVelocity(float x){
		if (rg2D == null) {
			Debug.Log(rg2D);
		}
		rg2D.velocity = new Vector2(x, 0);
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("shield")) {
			Destroy(this.gameObject);
		}
		if(other.gameObject.CompareTag("Player")){
			other.gameObject.GetComponent<wu_PlayerController>().takeDamage(5);
			Destroy(this.gameObject);
		}
		
	}
}
