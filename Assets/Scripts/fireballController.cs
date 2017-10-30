using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireballController : MonoBehaviour {
	

	public float speed = 1f;
	Vector2 startPos;
	Vector2 endPos;
	float startTime;
	float length;
	Animator animator;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		startPos = transform.position;
		endPos = new Vector2(startPos.x - 4.12f, startPos.y -1.78f);
		length = Vector2.Distance(startPos,endPos);
		animator=GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		float disCover = (Time.time - startTime)*speed;
		float fracTime = disCover / length;
		transform.position = Vector2.Lerp(startPos,endPos,fracTime);
		//Debug.Log(transform.position.x);
		if (transform.position.x == endPos.x){
			animator.SetTrigger("explotion");
			//Debug.Log("trigger");
		}
		
	}
	void destroySelf(){
		Destroy(this.gameObject);
	}
}
