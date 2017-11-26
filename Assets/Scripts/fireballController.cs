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
		
		animator=GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		float disCover = (Time.time - startTime)*speed;
		float fracTime = disCover / length;
		if (endPos != null){
			transform.position = Vector2.Lerp(startPos,endPos,fracTime);
			//Debug.Log(transform.position.x);
			// if (transform.position.x == endPos.x){
			// 	animator.SetTrigger("explotion");
			// }
		}
		
	}

	public void setPlayerLocation(Vector2 location){
		endPos = location;
		length = Vector2.Distance(startPos,endPos);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		animator.SetTrigger("explotion");
		if (other.gameObject.CompareTag("shield")){
			
			Destroy(other.gameObject);
		}
	}
	void destroySelf(){
		Destroy(this.gameObject);
	}
}
