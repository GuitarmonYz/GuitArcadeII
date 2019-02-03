using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {
	private bool actioning;
	private Rigidbody2D rb2d;
	private Vector2 curPos;
	private float idelTime;
	public GameObject player;
	public GameObject fireBall;
	private bool prepare;
	private bool isIdel;
	private bool isMoving;
	private Animator animator;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		actioning = false;
		prepare = false;
		isIdel = false;
		isMoving = false;
		curPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (!actioning){
			actioning = true;
			if (prepare) {
				Debug.Log("Prepare Fire");
			} else {
				int random = Random.Range(1,4);
				curPos = transform.position;
				switch (random) {
					case 1 :
						if (curPos.x - player.transform.position.x + 3 >= 18){
							int subrandom = Random.Range(1,3);
							switch(subrandom) {
								case 1 :
									moveLeft();
									break;
								case 2 :
									notMove();
									break;
							}
							Debug.Log("right conner");
						}else{
							moveRight();
						}
						break;
					case 2 :
						if (curPos.x - 3 - player.transform.position.x <=2) {
							int subrandom = Random.Range(1,3);
							switch(subrandom) {
								case 1 :
									moveRight();
									break;
								case 2 :
									notMove();
									break;
							}
						}else{
							moveLeft();
						}
						break;
					case 3 :
						notMove();
						break;
				}
			}
		} else {
			if (isMoving){
				if (Mathf.Abs(transform.position.x - curPos.x) > 3) {
					rb2d.velocity = new Vector2(0,0);
					actioning = false;
					isMoving = false;
					Debug.Log("terminate");
				}
			}
			if (isIdel) {
				if (Time.time > idelTime + 4){
					actioning = false;
					isIdel = false;
				} 
			}
		}
	}
	public void moveRight(){
		rb2d.velocity = new Vector2 (1f, 0);
		isMoving = true;
		animator.SetTrigger("Move");
		Debug.Log("move right");
	}

	public void moveLeft(){
		rb2d.velocity = new Vector2 (-1f, 0);
		isMoving = true;
		animator.SetTrigger("Move");
		Debug.Log("move left");
	}

	public void notMove(){
		Debug.Log("not Move");
		animator.SetTrigger("Idel");
		idelTime = Time.time;
		isIdel = true;
	}

	public void prepareFire(){
		this.prepare = true;
		animator.SetTrigger("PreFire");
	}

	public void shootFire(){
		if (prepare){
			Debug.Log("fire");
			animator.SetTrigger("Fire");
			Vector2 curPos = this.transform.position;
			Vector2 tarPos = new Vector2(curPos.x - 2.24f, curPos.y + 0.43f);
			GameObject new_fireBall = Instantiate(fireBall,tarPos,Quaternion.identity);
			fireBallController fc = new_fireBall.GetComponent<fireBallController>();
			fc.setPlayerPos(player.transform.position);
			this.prepare = false;
			this.actioning = false;
		}
	}

	



}
