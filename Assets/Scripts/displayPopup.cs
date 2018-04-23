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
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kCirFifth] = true;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kRhythm] = true;
		}
		if (this.gameObject.name == "gameboy") {
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kMelody] = true;
		}
	}
	
	void OnMouseExit()
	{
		displayInfo = false;	
		if (this.gameObject.name == "Safe") {
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kCirFifth] = false;
		}
		if (this.gameObject.name == "desktop_computer") {
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kRhythm] = false;
		}
		if (this.gameObject.name == "gameboy") {
			GlobalControl.Instance.seen[(int)GlobalControl.Stages.kMelody] = false;
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
