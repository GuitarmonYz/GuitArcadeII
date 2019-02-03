using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireBallController : MonoBehaviour {

	Vector2 playerPos;
	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (playerPos != null)
			transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), playerPos, 3 * Time.deltaTime);
	}

	public void setPlayerPos(Vector2 pos) {
		playerPos = pos;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// Debug.Log("trigger");
		if (other.gameObject.CompareTag("Player")){
			// Debug.Log("trigger_ed");
			// this.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
			animator.SetTrigger("explode");
		} else if (other.gameObject.CompareTag("shield")) {
			animator.SetTrigger("explode");
		}
	}
	void destroySelf(){
		Destroy(this.gameObject);
	}
}
