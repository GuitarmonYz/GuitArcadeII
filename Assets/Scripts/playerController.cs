﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class playerController : MonoBehaviour {
	public GameObject umbrella;
	private GameObject new_umb;
	public GameObject shield;
	private GameObject new_shi;
	public GameObject spear;
	private GameObject new_spear;
	Rigidbody2D rb2d;
	Animator animator;
	SpriteRenderer spriteRenderer;
	float moveH = 0;
	// Use this for initialization
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		rb2d.velocity = new Vector2(moveH,0);
	}
	public void setMoveH(float moveH){
		this.moveH = moveH;
	}
	public void move(float moveH){
		this.moveH = moveH;
		animator.SetBool("move", true);
	}
	public void back(float moveH){
		this.moveH = moveH;
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
		Vector2 tarPos = new Vector2(curPos.x+0.1f, curPos.y);
		// Quaternion tar_qua = Quaternion.identity;
		// tar_qua.z = -55;
		new_spear = Instantiate(spear, tarPos, Quaternion.identity);
		new_spear.transform.parent = this.transform;
	}

}
