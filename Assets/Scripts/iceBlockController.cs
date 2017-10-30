using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceBlockController : MonoBehaviour {
	public float speed = 1f;
	float length;
	float startTime;
	Vector2 startPos;
	Vector2 endPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
		startTime = Time.time;
		endPos = new Vector2(startPos.x,startPos.y-8.88f);
		length = Vector2.Distance(startPos,endPos);
	}
	
	// Update is called once per frame
	void Update () {
		float disCover = (Time.time - startTime)*speed;
		float fracTime = disCover / length;
		transform.position = Vector2.Lerp(startPos,endPos,fracTime);
	}
	
	
	private void selfDestroy(){
		Destroy(this.gameObject);
	}
	public void destroy(){
		Invoke("selfDestroy",0.01f);
	}
}
