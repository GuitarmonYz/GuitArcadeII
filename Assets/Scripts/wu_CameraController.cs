using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wu_CameraController : MonoBehaviour {
	private Vector2 velocity;
	public float smoothTimeh;
	public float smoothTimev;
	private float startPosY;
	private bool downSwitch = false;
	void Start () {
		startPosY = transform.position.y - 6.8f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (downSwitch) {
			DownMoveScreen();
		}
	}
	void DownMoveScreen () {
		if (transform.position.y > startPosY) {
			float PosY = Mathf.SmoothDamp (transform.position.y, transform.position.y-0.02f, ref velocity.y, smoothTimev);
			transform.position = new Vector3 (transform.position.x, PosY, transform.position.z);
		} else {
			startPosY = transform.position.y - 6.8f;
			downSwitch = false;
		}
	}
	public void turnOnScroll() {
		downSwitch = true;
	}
}
