using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossController : MonoBehaviour {
	public GameObject fireball;
	public GameObject ice;
	public GameObject fireParticle;
	SpriteRenderer fireRender;
	Rigidbody2D rb2d;
	SpriteRenderer spriteRenderer;
	PolygonCollider2D[] colliders;
	GameObject fireParticleEffect;
	Animator animator;
	float moveH = 0;
	int currentColliderIndex = 0;
	float attack_time = 0;
	Color32 ori_color;
	Color32 dst_color;
	Color32 red;
	Color32 blue;
	// Use this for initialization
	void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		fireParticleEffect = this.transform.GetChild(0).gameObject;
		colliders = fireParticleEffect.GetComponents<PolygonCollider2D>();
		fireRender = fireParticle.GetComponent<SpriteRenderer>();
	}
	void Start () {
		ori_color = Color.white;
		dst_color = Color.white;
		red = new Color32(255,66,66,255);
		blue = new Color32(72,66,255,255);
	}
	
	// Update is called once per frame
	void Update () {
		rb2d.velocity = new Vector2(moveH,0);
		spriteRenderer.color = Color.Lerp(spriteRenderer.color,dst_color, Time.deltaTime);
		if(Time.time>attack_time+0.5f) animator.SetBool("attack",false);
	}
	public void setMoveH(float moveH){
		this.moveH = moveH;
	}
	public void move(float moveH){
		this.moveH = moveH;
		//animator.SetBool("move", true);
	}
	public void back(float moveH){
		this.moveH = moveH;
	}
	public void stop(){
		this.moveH = 0;
		//animator.SetBool("move", false);
	}
	public void switchColor(){
		if (ori_color == Color.white){
			dst_color = red;
			ori_color = blue;
			//fireRender.color = blue;
		}else{
			Color32 tmp = ori_color;
			ori_color = dst_color;
			dst_color = tmp;
		}
	}
	public void attack(int attack_way){
		animator.SetBool("attack",true);
		this.attack_time = Time.time;
		if (attack_way == 0){
			Invoke("Instantiate_fireball",0.5f);
		}else{
			Invoke("Instantiate_ice",0.5f);
		}
	}
	private void Instantiate_fireball(){
		Vector2 curPos = this.transform.position;
		Vector2 tarPos = new Vector2(curPos.x - 2.24f, curPos.y + 0.43f);
		Instantiate(fireball,tarPos,Quaternion.identity);
	}
	private void Instantiate_ice(){
		Vector2 curPos = this.transform.position;
		Vector2 tarPos = new Vector2(curPos.x-7.72f,curPos.y+6.29f);
		Instantiate(ice, tarPos,Quaternion.identity);
	}
	public void SetColliderForSprite(int spriteNum){
		colliders[currentColliderIndex].enabled = false;
		currentColliderIndex = spriteNum;
		colliders[currentColliderIndex].enabled = true;
	}
}
