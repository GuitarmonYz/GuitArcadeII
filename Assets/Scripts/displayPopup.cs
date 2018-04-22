using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayPopup : MonoBehaviour {
	public Text text;
	public float fadeTime = 10;
	private bool displayInfo = false;
	void Start () {
		text.color = Color.clear;
	}
	void Update () {
		fadeText();
	}
	
	void OnMouseOver()
	{
		displayInfo = true;
		if (this.gameObject.name == "Safe") {
			GlobalControl.Instance.seen[0] = true;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[1] = true;
		}
		if (this.gameObject.name == "gameboy") {
			Debug.Log("Mouse over gameboy");
			GlobalControl.Instance.seen[2] = true;
		}
	}
	
	void OnMouseExit()
	{
		displayInfo = false;	
		if (this.gameObject.name == "Safe") {
			GlobalControl.Instance.seen[0] = false;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[1] = false;
		}
		if (this.gameObject.name == "gameboy") {
			GlobalControl.Instance.seen[1] = false;
		}
	}

	private void fadeText() {
		if (displayInfo) {
			text.color = Color.Lerp(text.color, Color.black, fadeTime * Time.deltaTime);
		} else {
			text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
		}
	}
}
