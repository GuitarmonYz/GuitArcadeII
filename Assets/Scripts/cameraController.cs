using UnityEngine;
//using System.Collections;

public class cameraController : MonoBehaviour {
	public GameObject player;
	private Vector2 velocity;
	public float smoothTimeh;
	public float smoothTimev;
	// Use this for initialization
	void Start () {
		//player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float PosX = Mathf.SmoothDamp (transform.position.x, player.transform.position.x+4.63f, ref velocity.x, smoothTimeh);
		float PosY = Mathf.SmoothDamp (transform.position.y, player.transform.position.y+2.59f, ref velocity.y, smoothTimev);
		transform.position = new Vector3 (PosX, PosY, transform.position.z);
	}
}
