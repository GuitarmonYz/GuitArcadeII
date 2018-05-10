using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class playerController : MonoBehaviour {
	public GameObject umbrella;
	private GameObject new_umb;
	public GameObject shield;
	private GameObject new_shi;
	public GameObject spear;
	private GameObject new_spear;
	public Image content;
	Rigidbody2D rb2d;
	Animator animator;
	SpriteRenderer spriteRenderer;
	float moveH = 0;
	float health = 100;
	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		if(content != null) content.fillAmount = health/100.0f;
	}
	
	// Update is called once per frame
	void Update () {
		rb2d.velocity = new Vector2(moveH,0);
	}
	public void setMoveH(float moveH){
		this.moveH = moveH;
	}
	public void prepare(){
		animator.SetBool("prepare", true);
	}
	public void deprepare(){
		animator.SetBool("prepare", false);
	}
	public void removeShield(){
		if (new_shi != null) Destroy(new_shi.gameObject);
	}
	public void move(float moveH){
		this.moveH = moveH;
		deprepare();
		animator.SetBool("move", true);
	}
	public void back(float moveH){
		this.moveH = moveH;
		deprepare();
		animator.SetBool("move",true);
	}
	public void stop(){
		this.moveH = 0;
		animator.SetBool("move", false);
	}
	public void use_shield(){
		if(new_umb!=null){
			Destroy(new_umb.gameObject);
		}
		new_shi = Instantiate(shield, this.transform.position, Quaternion.identity);
		new_shi.transform.parent = this.transform;
	}
	public void use_umbrella(){
		if(new_shi != null){
			Destroy(new_shi.gameObject);
		}
		Vector2 curPos = this.transform.position;
		Vector2 tarPos = new Vector2(curPos.x+0.4f,curPos.y+0.45f);
		new_umb = Instantiate(umbrella, tarPos, Quaternion.identity);
		new_umb.transform.parent = this.transform;
	}
	public void throw_spear(){
		Vector2 curPos = this.transform.position;
		
		for(int i = 0; i < 3; i++){
			float offset = UnityEngine.Random.Range(0.1f,2f);
			Vector2 tarPos = new Vector2(curPos.x+offset, curPos.y);
			new_spear = Instantiate(spear, tarPos, Quaternion.identity);
			new_spear.transform.parent = this.transform;
		}
	}

	// void OnCollisionEnter2D(Collision2D other)
	// {
	// 	Debug.Log("trigger");
	// 	if (other.gameObject.CompareTag("fireball")){
	// 		this.health -= 10;
	// 		content.fillAmount = health/100.0f;
	// 	}
	// }

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("trigger");
		if (other.gameObject.CompareTag("fireball")){
			this.health -= 10;
			content.fillAmount = health/100.0f;
		}
	}

}
