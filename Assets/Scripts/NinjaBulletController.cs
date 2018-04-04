using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaBulletController : MonoBehaviour {
	private float rotation_speed = 60;
	private Rigidbody2D rg2D;
	// Use this for initialization
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		rg2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		rg2D.MoveRotation(rg2D.rotation + rotation_speed * Time.fixedDeltaTime);
	}
	public void setVelocity(float x){
		// rg2D = GetComponent<Rigidbody2D>();
		if (rg2D == null) {
			Debug.Log(rg2D);
		}
		rg2D.velocity = new Vector2(x, 0);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.CompareTag("Player")){
			other.gameObject.GetComponent<wu_PlayerController>().takeDamage();
			Destroy(this.gameObject);
		}
		
	}
}
