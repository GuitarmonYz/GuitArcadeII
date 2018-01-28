using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
public class arrowController : MonoBehaviour {
	public GameObject center;
	private string[] circleOfFifths = {"C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"};
	//private Vector2[] circlePos = new Vector2[12];
	private string[] midiNames = {"C","Db","D","Eb","E","F","Gb","G","Ab","A","Bb","B"};
	private int rotateCounter = 0;
	private float speed = 10;
	private float radius = 3.56f;
	private string preNote = "C";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < 128; i++) {
			if (MidiMaster.GetKeyDown(i)) {
				string curNote = noteNum2Name(i);
				rotate(preNote, curNote);
				preNote = curNote;
			}
		}

		// if (rotateCounter < speed) {
		// 	rotateCounter++;
		// 	rotate("A", "B", speed);
		// 	//Debug.Log("rotating");
		// } else {
		// 	rotateCounter = 0;
		// }
		//rotate("A", "B");
	}

	private string noteNum2Name(int noteNum){
		return this.midiNames[noteNum % 12];
	}

	private void rotate(string startNote, string endNote){
		float angleToRotate = findAngle(endNote) - findAngle(startNote);
		transform.RotateAround(center.transform.position, Vector3.back, angleToRotate);
	}

	private float findAngle(string noteName){
		float angle = 0;
		for (int i = 0; i < circleOfFifths.Length; i++) {
			if (circleOfFifths[i].Equals(noteName)) {
				angle = (i + 1) * 30;
			}
		}
		return angle;
	}
}
