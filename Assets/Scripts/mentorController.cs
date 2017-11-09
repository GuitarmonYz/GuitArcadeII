using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mentorController : MonoBehaviour {
	Rigidbody2D rb2d;
	float moveH = 0;
	void Awake () {
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		rb2d.velocity = new Vector2(moveH,0);
	}
	public void move(float moveH){
		this.moveH = moveH;
	}
	public void stop(){
		this.moveH = 0;
	}
}
