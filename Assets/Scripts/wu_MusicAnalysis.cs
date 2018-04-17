using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
using FastEMD;
public class wu_MusicAnalysis : MonoBehaviour {
	private Queue buffer;
	private Dictionary<string, int> midiNumDict;
	private HashSet<int> MajorChord;
	private HashSet<int> MinChord;
	private float dequeueCycle = 1;
	private float nextDequeueTime;
	// Use this for initialization
	void Start () {
		buffer = new Queue();
		midiNumDict = new Dictionary<string, int>(){
			{"C", 0}, {"C#", 1}, {"D", 2}, {"D#",3},
			{"E", 4}, {"F", 5}, {"F#", 6}, {"G", 7},
			{"G#", 8}, {"A", 9}, {"A#", 10}, {"B", 11}
		};
		MajorChord = new HashSet<int>(){
			0, 4, 7, 11
		};
		MinChord = new HashSet<int>(){
			0, 3, 7, 10
		};
		nextDequeueTime = Time.time + dequeueCycle;
		Signiture sg1 = new Signiture();
		Feature[] features_1 = new Feature[]{
			new Feature(0, 180.38f), new Feature(1.583f, 175.38f),
			new Feature(3.166f, 169.138f), new Feature(6.333f, 192.138f),
			new Feature(7.916f, 197.138f), new Feature(9.5f, 192.138f),
			new Feature(12.667f, 186.138f), new Feature(14.25f, 192.138f),
			new Feature(15.833f, 197.138f), new Feature(17.41f, 192.138f),
			new Feature(19, 186.138f), new Feature(20.58f, 180.138f),
			new Feature(22.166f, 175.138f), new Feature(25.333f, 180.138f),
			new Feature(26.916f, 175.138f), new Feature(28.583f, 169.138f)
		};
		// Feature[] features_1 = new Feature[]{
		// 	new Feature(0, 180.38f), new Feature(1.583f, 175.38f),
		// 	new Feature(3.166f, 169.138f), new Feature(6.333f, 192.138f),
		// };
		sg1.setFeatures(features_1);
		float[] weights_1 = new float[]{
			0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1, 0.5f, 0.5f, 1
		};
		sg1.setWeights(weights_1);
		Signiture sg2 = new Signiture();
		Feature[] features_2 = new Feature[]{
			new Feature(0, 180), new Feature(2.25f, 175),
			new Feature(3, 169), new Feature(6, 192),
			new Feature(7.5f, 197), new Feature(9, 192),
			new Feature(12, 186), new Feature(13.5f, 192),
			new Feature(15, 197), new Feature(16.5f, 192),
			new Feature(18, 186), new Feature(19.5f, 180),
			new Feature(22.5f, 175), new Feature(25.5f, 180),
			new Feature(27.75f, 175), new Feature(28.5f, 169)
		};
		sg2.setFeatures(features_2);
		float[] weights_2 = new float[]{
			0.75f, 0.25f, 1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1, 1, 0.75f, 0.25f, 1
		};
		sg2.setWeights(weights_2);
		Debug.Log(EMDProcessor.distance(sg1, sg2, 0));
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextDequeueTime && buffer.Count != 0) {
			buffer.Dequeue();
			nextDequeueTime = Time.time + dequeueCycle;
		}
		for(int i = 0; i < 128; i++) {
			if (MidiMaster.GetKey(i) != 0) {
				enqueueBuffer(i);
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

	public bool checkChordCorrect(string chord){
		if (buffer.Count == 0) return false;
		int chordMidiNum = (chord.Length == 1) ? midiNumDict[chord] : midiNumDict[chord.Substring(0,1)];
		bool res = true;
		if (chord.Length == 1) {
			foreach(int num in buffer) {
				res = res && MajorChord.Contains(num%12 - chordMidiNum);
			}
		} else {
			foreach(int num in buffer) {
				res = res && MinChord.Contains(num%12 - chordMidiNum);
			}
		}
		return res;
	}
}
