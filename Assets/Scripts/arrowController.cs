using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;
using UnityEngine.SceneManagement;
public class arrowController : MonoBehaviour {
	public GameObject center;
	public GameObject meshDrawer;
	public GameObject canvasObject;
	private MeshTest meshTest;
	private string[] circleOfFifths = {"C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"};
	private Vector2[] circlePos = new Vector2[12];
	private Text[] noteNames = new Text[12];
	private string[] midiNames = {"C","Db","D","Eb","E","F","F#","G","Ab","A","Bb","B"};
	private int questionNote;
	private int targetNote;
	private float angle = 0;
	private float radius = 3.56f;
	private float radiusInPixal = 340f;
	private bool isQuestionNote = true;
	private int[] dominantChord = new int[]{0,1,4,10};
	private int[] maj7Chord = new int[]{0,1,4,5};
	private AudioClip[] audioClips;
	private AudioSource audioSource;
	private bool meshGenerated = false;
	private Queue inputBuffer = new Queue();
	private Dictionary<int, bool> chordDic = new Dictionary<int, bool>();
	private int successulRounds = 3;
	private int curSuccessulRounds = 0;
	void Awake()
	{
		Object[] chords_clips_raw = Resources.LoadAll("AudioFiles/circle_of_fifth_chords", typeof(AudioClip));
		audioClips = new AudioClip[chords_clips_raw.Length];
		for (int i = 0; i < chords_clips_raw.Length; i++) {
			audioClips[i] = (AudioClip) chords_clips_raw[i];
		}
		audioSource = GetComponent<AudioSource>();
	}

	void Start () {
		//targetNote = Random.Range(0, circleOfFifths.Length-1);
		// targetNote = circleOfFifths.Length-1;
		targetNote = 7;
		questionNote = (targetNote + 1) % 12;
		for (int i = 0; i < 12; i++) {
			float angle = findAngle(circleOfFifths[i]);
			circlePos[i] = new Vector2(radius * Mathf.Sin(Mathf.Deg2Rad * angle), radius * Mathf.Cos(Mathf.Deg2Rad * angle));
			Vector3 uiPos = new Vector3(radiusInPixal * Mathf.Sin(Mathf.Deg2Rad * angle), radiusInPixal * Mathf.Cos(Mathf.Deg2Rad * angle), 0);
			noteNames[i] = drawNoteNameUI(uiPos, circleOfFifths[i]);
			noteNames[i].enabled = false;
		}
		meshTest = meshDrawer.GetComponent<MeshTest>();
	}
	
	void Update () {

		if (Input.GetKey("k")) {
			GlobalControl.Instance.completed[(int)GlobalControl.Stages.kCirFifth] = true;
			SceneManager.LoadScene(0);
		}

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

		float dial = MidiMaster.GetDial();
		if (dial > 64) {
			angle += rotate(true, (dial-64)/64f * 60f + 20f);
		} else if (MidiMaster.GetDial() < 64) {
			angle -= rotate(false, (64 - dial)/64f * 60f + 20f);
		}

		if (isQuestionNote) {
			if (roundAngle(angle) <= (questionNote * 30)%360 + 2 && roundAngle(angle) >= (questionNote * 30)%360 - 2) {
				if (!audioSource.isPlaying) {
					audioSource.clip = findClipIdx(questionNote, true);
					audioSource.Play();
				}
				
				toggleNoteNameUI(questionNote, dominantChord, true);
				if (!meshGenerated){
					generateMesh(dominantChord, questionNote);
					meshGenerated = true;
				}
				if (isConfirmed(isQuestionNote)) {
					isQuestionNote = false;
					meshTest.clearMesh();
					meshGenerated = false;
					toggleNoteNameUI(questionNote, dominantChord, false);
					inputBuffer.Clear();
					audioSource.Stop();
				}
			} else {
				if (audioSource.isPlaying) audioSource.Stop();
				meshTest.clearMesh();
				meshGenerated = false;
				toggleNoteNameUI(questionNote, dominantChord, false);
			}
		} else {
			if ( roundAngle(angle) <= (targetNote * 30)%360 + 2 && roundAngle(angle) >= (targetNote * 30)%360 - 2) {
				if (!audioSource.isPlaying) {
					audioSource.clip = findClipIdx(targetNote, false);
					audioSource.Play();
				}
				toggleNoteNameUI(targetNote, maj7Chord, true);
				if (!meshGenerated) {
					generateMesh(maj7Chord, targetNote);
					meshGenerated = true;
				}
				if (isConfirmed(isQuestionNote)) {
					isQuestionNote = true;
					meshTest.clearMesh();
					meshGenerated = false;
					toggleNoteNameUI(targetNote, maj7Chord, false);
					targetNote = Random.Range(0, circleOfFifths.Length-1);
					questionNote = (targetNote + 1) % 12;
					Debug.Log(circleOfFifths[questionNote]);
					Debug.Log(questionNote);
					inputBuffer.Clear();
					audioSource.Stop();
					curSuccessulRounds++;
					if (curSuccessulRounds >= successulRounds) {
						GlobalControl.Instance.completed[(int)GlobalControl.Stages.kCirFifth] = true;
						SceneManager.LoadScene(0);
					}
				}
			} else {
				if (audioSource.isPlaying) audioSource.Stop();
				meshTest.clearMesh();
				meshGenerated = false;
				toggleNoteNameUI(targetNote, maj7Chord, false);
			}
		}
	}

	private AudioClip findClipIdx(int note, bool isDominant){
		string chord_name = circleOfFifths[note];
		AudioClip res = null;
		foreach(AudioClip ac in audioClips) {
			string[] nameArray = ac.name.Split('_');
			if (nameArray[0].Equals(chord_name)) {
				if (isDominant) {
					if(nameArray[1].Equals("7")) {
						res = ac;
					}
				} else {
					if (nameArray[1].Equals("Maj7")) {
						res = ac;
					}
				}
			}
		}
		return res;
	}

	private Text drawNoteNameUI(Vector3 position, string text) {
		GameObject ngo = new GameObject(text);
		ngo.transform.SetParent(canvasObject.transform);
		Text myText = ngo.AddComponent<Text>();
		Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
     	myText.font = ArialFont;
		myText.fontSize = 42;
		myText.text = text; 
		myText.alignment = TextAnchor.MiddleCenter;
		myText.rectTransform.localPosition = position;
		myText.rectTransform.localScale = new Vector3(1,1,1);
		return myText;
	}
	private float roundAngle(float angle) {
		float res = angle;
		while(res < 0) {
			res += 360;
		}
		return res;
	}

	private void toggleNoteNameUI(int note, int[] chord, bool enabled){
		for (int i = 0; i < chord.Length; i++) {
			noteNames[(note+chord[i])%12].enabled = enabled;
		}
	}

	private void generateMesh(int[] notes, int note){
		Vector2[] vertices2D = new Vector2[notes.Length];
		for (int i = 0; i < notes.Length; i++) {
			vertices2D[i] = circlePos[(note+notes[i])%12];
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
				chordDic.Add(midiName2noteNum(circleOfFifths[(targetNote + maj7Chord[i])%12]), false);
			}
		}
		
		foreach(int note in inputBuffer) {
			if (chordDic.ContainsKey(note%12)) {
				chordDic[note%12] = true;
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
