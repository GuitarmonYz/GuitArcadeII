using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {
	private bool actioning;
	private Rigidbody2D rigidbody2D;
	public GameObject player;

	public int speed = 20;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		actioning = false;
		moveForward();
	}
	
	// Update is called once per frame
	void Update () {
		//moveForward();
	}

	private void moveForward(){
		
		
	}

	private void moveBackward(){

	}

}
