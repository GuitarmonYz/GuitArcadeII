using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
using FastEMD;
public class wu_MusicAnalysis : MonoBehaviour {
	private Queue buffer;
	private List<List<float[,]>> history;
	private List<float[,]> curBarBuffer;
	private Dictionary<string, int> midiNumDict;
	private HashSet<int> MajorChord;
	private HashSet<int> MinChord;
	private HashSet<int> M7Chord;
	private HashSet<int> ScaleTone;
	private float dequeueCycle = 3;
	private float nextDequeueTime;
	private float curBarTime = 0;
	private int curBar = 0;
	private GameObject player;
	private wu_PlayerController playerController;
	private bool emphasis = false;
	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerController = player.GetComponent<wu_PlayerController>();
	}
	void Start () {
		buffer = new Queue();
		curBarBuffer = new List<float[,]>();
		history = new List<List<float[,]>>();
		for (int i = 0; i < 8; i++) {
			history.Add(new List<float[,]>());
		}
			
		midiNumDict = new Dictionary<string, int>(){
			{"C", 0}, {"C#", 1}, {"D", 2}, {"D#",3},
			{"E", 4}, {"F", 5}, {"F#", 6}, {"G", 7},
			{"G#", 8}, {"A", 9}, {"Bb", 10}, {"B", 11}
		};
		MajorChord = new HashSet<int>(){
			0, 4, 7, 11
		};
		M7Chord = new HashSet<int>(){
			2, 5, 7, 11
		};
		MinChord = new HashSet<int>(){
			0, 2, 5, 9
		};
		ScaleTone = new HashSet<int>(){
			0, 2, 4, 5, 7, 9, 11
		};
		nextDequeueTime = Time.time + dequeueCycle;
	}
	void Update () {
		if (Time.time > nextDequeueTime && buffer.Count != 0) {
			if (buffer.Count != 0)
				buffer.Dequeue();
			nextDequeueTime = Time.time + dequeueCycle;
		}
		for(int i = 0; i < 128; i++) {
			if (MidiMaster.GetKeyDown(i)) {
				enqueueBuffer(i);
				curBarBuffer.Add(new float[, ]{{Time.time - curBarTime, i}});
			}
		}
	}
	void enqueueBuffer(int newVal) {
		if (buffer.Count < 4) {
			buffer.Enqueue(newVal);
		} else {
			buffer.Dequeue();
			buffer.Enqueue(newVal);
		}
	}

	public bool checkScaleCorrect() {
		if (buffer.Count == 0) return false;
		bool res = true;
		foreach(int num in buffer) {
			res = res && ScaleTone.Contains(num%12);
		}
		return res;
	}
	public int getCurBar(){
		return curBar;
	}

	public bool checkChordCorrect(string chord){
		if (buffer.Count == 0) return false;
		int chordMidiNum = (chord.Length == 1) ? midiNumDict[chord] : midiNumDict[chord.Substring(0,1)];
		bool res = true;
		if (chord.Length == 1) {
			if (chordMidiNum == 0) {
				foreach(int num in buffer) {
					res = res && MajorChord.Contains(num%12);
				}
			} else {
				foreach(int num in buffer) {
					res = res && M7Chord.Contains(num%12);
				}
			}
		} else {
			foreach(int num in buffer) {
				res = res && MinChord.Contains(num%12);
			}
		}
		return res;
	}
	public void setBarBegin(float time) {
		float barEndTime = time - curBarTime;
		curBarTime = time;
		if (curBar-1 >= 0) {
			int preBarIdx = (curBar-1) % 8;
			history[preBarIdx].Clear();
			foreach (float[,] pair in curBarBuffer) {
				history[preBarIdx].Add(pair);
			}
			if (preBarIdx < 4) {
				if (history[preBarIdx + 4].Count != 0 && history[preBarIdx].Count != 0) {
					Debug.Log("start calculating EMD");
					CalculateEMD(history[preBarIdx], history[preBarIdx + 4], barEndTime);
				}
			} else {
				if (history[preBarIdx - 4].Count != 0 && history[preBarIdx].Count != 0) {
					Debug.Log("Start calculating EMD");
					CalculateEMD(history[preBarIdx], history[preBarIdx - 4], barEndTime);
				}
			}
		}
		// Debug.Log(curBar);
		checkEmphasis();
		curBarBuffer.Clear();
		buffer.Clear();
		curBar++;
	}
	private float[] getWeight(List<float[,]> buffer, float barEndTime){
		float[] weights = new float[buffer.Count];
		for (int i = 0; i < buffer.Count-1; i++) {
			weights[i] = buffer[i+1][0,0] - buffer[i][0,0];
		}
		weights[weights.Length-1] = barEndTime - buffer[buffer.Count-1][0,0];
		return weights;
	}

	private void checkEmphasis(){
		if (curBarBuffer.Count == 0) return;
		float targetWeight = 0;
		float count = 0;
		if ((curBar-1)%4 < 1) {
			Debug.Log("Dm");
			foreach(float[,] pair in curBarBuffer) {
				Debug.Log(pair[0,1]%12);
				if (pair[0,1]%12 == 0) {
					targetWeight += pair[0,0];
					count++;
				}
			}
		}
		if ((curBar-1)%4 == 1) {
			Debug.Log("G");
			foreach(float[,] pair in curBarBuffer) {
				if (pair[0,1]%12 == 4){
					targetWeight += pair[0,0];
					count++;
				}
			}
		}
		if ((curBar-1)%4 > 1) {
			Debug.Log("C");
			foreach(float[,] pair in curBarBuffer) {
				if (pair[0,1]%12 == 11) {
					targetWeight += pair[0,0];
					count++;
				}
			}
		}
		if (targetWeight/count > 7.4/curBarBuffer.Count) {
			emphasis = true;
		} else {
			emphasis = false;
		}
		Debug.Log("emph:" + emphasis);
	}

	public bool getEmph(){
		return emphasis;
	}

	private void CalculateEMD(List<float[,]> bf_1, List<float[,]> bf_2, float barEndTime){
		Signiture sg_1 = new Signiture();
		Signiture sg_2 = new Signiture();

		Feature[] features_1 = new Feature[bf_1.Count];
		Feature[] features_2 = new Feature[bf_2.Count];
		for (int i = 0; i < bf_1.Count; i++) {
			features_1[i] = new Feature(bf_1[i][0,0], bf_1[i][0,1]); 
		}
		for (int i = 0; i < bf_2.Count; i++) {
			features_2[i] = new Feature(bf_2[i][0,0], bf_2[i][0,1]);
		}
		float[] weights_1 = getWeight(bf_1, barEndTime);
		float[] weights_2 = getWeight(bf_2, barEndTime);
		
		sg_1.setFeatures(features_1);
		sg_1.setWeights(weights_1);
		sg_2.setFeatures(features_2);
		sg_2.setWeights(weights_2);
		double dist = EMDProcessor.distance(sg_1, sg_2, 0);
		Debug.Log("Dist: " + dist);
		if (dist < 2) {
			playerController.takeCreateDamage();
		}
	}
}
