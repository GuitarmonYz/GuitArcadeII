using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
public class arrowController : MonoBehaviour {
	public GameObject center;
	public GameObject meshDrawer;
	private MeshTest meshTest;
	private string[] circleOfFifths = {"C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"};
	private Vector2[] circlePos = new Vector2[12];
	private string[] midiNames = {"C","Db","D","Eb","E","F","Gb","G","Ab","A","Bb","B"};
	// private string preNote = "C";
	private int questionNote;
	private int targetNote;
	private float angle = 0;
	private float radius = 3.56f;
	private bool isQuestionNote = true;
	private int[] dominantChord = new int[]{0,1,4,10};
	private int[] maj7Chord = new int[]{0,1,4,5};
	private bool meshGenerated = false;
	private Queue inputBuffer = new Queue();
	private Dictionary<int, bool> chordDic = new Dictionary<int, bool>();
	// Use this for initialization
	void Start () {
		questionNote = Random.Range(0, circleOfFifths.Length-1);
		targetNote = (questionNote + 1) % 12;
		for (int i = 0; i < 12; i++) {
			float angle = findAngle(circleOfFifths[i]);
			circlePos[i] = new Vector2(radius * Mathf.Sin(Mathf.Deg2Rad * angle), radius * Mathf.Cos(Mathf.Deg2Rad * angle));
		}
		meshTest = meshDrawer.GetComponent<MeshTest>();
	}
	
	// Update is called once per frame
	void Update () {

		// for (int i = 0; i < 128; i++) {
		// 	if (MidiMaster.GetKeyDown(i)) {
		// 		string curNote = noteNum2Name(i);
		// 		rotate(preNote, curNote);
		// 		preNote = curNote;
		// 	}
		// }

		for (int i = 0; i < 128; i++) {
			if (MidiMaster.GetKeyDown(i)) {
				if (inputBuffer.Count >= 10) {
					inputBuffer.Dequeue();
					inputBuffer.Enqueue(i);
				} else {
					inputBuffer.Enqueue(i);
				}
			}
		}
		// if (Input.GetKeyDown("e")) {
		// 	Vector2[] vertices2D = new Vector2[] {
		// 		circlePos[0],
		// 		circlePos[1],
		// 		circlePos[4],
		// 		circlePos[5]
		// 	};
		// 	meshTest.drawMesh(vertices2D);
		// }
		// if (Input.GetKeyDown("r")) {
		// 	Vector2[] vertices2D = new Vector2[] {
		// 		circlePos[0],
		// 		circlePos[2],
		// 		circlePos[5],
		// 		circlePos[8]
		// 	};
		// 	meshTest.drawMesh(vertices2D);
		// }
		float dial = MidiMaster.GetDial();
		if (dial > 64) {
			angle += rotate(true, (dial-64)/64f * 60f + 20f);
		} else if (MidiMaster.GetDial() < 64) {
			angle -= rotate(false, (64 - dial)/64f * 60f + 20f);
		}
		if (isQuestionNote) {
			if (angle <= questionNote * 30 + 2 && angle >= questionNote * 30 - 2) {
				Debug.Log("found question note " + circleOfFifths[questionNote]);
				Debug.Log("play question note " + circleOfFifths[questionNote] + "7");
				if (!meshGenerated){
					generateMesh(dominantChord);
					meshGenerated = true;
				}
				if (isConfirmed(isQuestionNote)) {
					isQuestionNote = false;
					meshTest.clearMesh();
					meshGenerated = false;
				}
			} else {
				meshGenerated = false;
			}
		} else {
			if (angle <= targetNote * 30 + 2 && angle >= targetNote * 30 - 2) {
				Debug.Log("found target note " + circleOfFifths[targetNote]);
				Debug.Log("play question note " + circleOfFifths[targetNote] + "maj7");
				if (!meshGenerated) {
					generateMesh(maj7Chord);
					meshGenerated = true;
				}
				if (isConfirmed(isQuestionNote)) {

				}
			} else {
				meshGenerated = false;
			}
		}
	}

	private void generateMesh(int[] notes){
		Vector2[] vertices2D = new Vector2[notes.Length];
		for (int i = 0; i < notes.Length; i++) {
			vertices2D[i] = circlePos[(questionNote+notes[i])%12];
		}
		meshTest.drawMesh(vertices2D);
	}

	private bool isConfirmed(bool isQuestionNote){
		chordDic.Clear();
		if (isQuestionNote) {
			for (int i = 0; i < dominantChord.Length; i++) {
				chordDic.Add(midiName2noteNum(circleOfFifths[(questionNote + dominantChord[i])%12]), false);
			}
		} else {
			for (int i = 0; i < maj7Chord.Length; i++) {
				chordDic.Add(midiName2noteNum(circleOfFifths[(questionNote + maj7Chord[i])%12]), false);
			}
		}
		foreach(int note in inputBuffer) {
			if (chordDic.ContainsKey(note)) {
				chordDic[note] = true;
			}
		}
		foreach (bool val in chordDic.Values) {
			if (!val) return false;
		}
		return true;
	}

	private int midiName2noteNum(string midiName) {
		for (int i = 0; i < midiNames.Length; i++) {
			if (midiNames[i].Equals(midiName)) {
				return i;
			}
		}
		return -1;
	}

	// private string noteNum2Name(int noteNum){
	// 	return this.midiNames[noteNum % 12];
	// }

	// private void rotate(string startNote, string endNote){
	// 	float angleToRotate = findAngle(endNote) - findAngle(startNote);
	// 	transform.RotateAround(center.transform.position, Vector3.back, angleToRotate);
	// }

	private float rotate(bool clockWise, float speed) {
		if (clockWise) {
			transform.RotateAround(center.transform.position, Vector3.back, speed * Time.deltaTime);
		} else {
			transform.RotateAround(center.transform.position, Vector3.forward, speed * Time.deltaTime);
		}
		return speed * Time.deltaTime;
	}

	private float findAngle(string noteName){
		float angle = 0;
		for (int i = 0; i < circleOfFifths.Length; i++) {
			if (circleOfFifths[i].Equals(noteName)) {
				angle = i * 30;
			}
		}
		return angle;
	}
}
