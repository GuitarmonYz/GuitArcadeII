using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spearController : MonoBehaviour {
	Rigidbody2D rb2d;
	Vector3 target;
	Animator animator;
	// Use this for initialization
	void Start () {
		rb2d = this.GetComponent<Rigidbody2D>();
		rb2d.AddForce(new Vector2(4.5f,10),ForceMode2D.Impulse);
		target = this.transform.position;
		target.x += 8.23f;
		animator = GetComponent<Animator>();
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("monster")){
			this.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
			animator.SetTrigger("explode");
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		var newRotation = Quaternion.LookRotation(transform.position - target, Vector3.forward);
    	newRotation.x = 0.0f;
    	newRotation.y = 0.0f;
    	transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 4);
	}

	void destroySelf(){
		Destroy(this.gameObject);
	}

}
