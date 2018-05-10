using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_rockController : MonoBehaviour {
	[HideInInspector] public int initFloor;
	private int curFloor;
	private GameObject player;
	private wu_MusicAnalysis musicAnalysis;
	private wu_PlayerController playerController;
	private Rigidbody2D rg2d;
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		musicAnalysis = player.GetComponent<wu_MusicAnalysis>();
		playerController = player.GetComponent<wu_PlayerController>();
		rg2d = GetComponent<Rigidbody2D>();
	}
	void Update () {
		rg2d.MoveRotation(rg2d.rotation + 60 * Time.fixedDeltaTime);
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("floor")) {
			int.TryParse(other.gameObject.name.Split('_')[1], out curFloor);
			if (curFloor > initFloor) {
				rg2d.velocity = new Vector2(-(rg2d.velocity.x * 1.5f), rg2d.velocity.y);
			}
			if (curFloor == initFloor) {
				rg2d.velocity = new Vector2(1, rg2d.velocity.y);
			}
		}
		if (other.gameObject.CompareTag("Player")) {
			if (!musicAnalysis.getEmph()) {
				// Debug.Log("take rock damage");
				playerController.takeDamage(5);
			}
			Destroy(this.gameObject);
		}
	}
	public void setInitFloor(int floor){
		initFloor = floor;
	}
	
}
