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
	private float dequeueCycle = 1;
	private float nextDequeueTime;
	private float curBarTime = 0;
	private int curBar = 0;
	// Use this for initialization
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
			{"G#", 8}, {"A", 9}, {"A#", 10}, {"B", 11}
		};
		MajorChord = new HashSet<int>(){
			0, 4, 7, 11
		};
		MinChord = new HashSet<int>(){
			0, 3, 7, 10
		};
		nextDequeueTime = Time.time + dequeueCycle;
		// Signiture sg1 = new Signiture();
		// Feature[] features_1 = new Feature[]{
		// 	new Feature(0, 180.38f), new Feature(1.583f, 175.38f),
		// 	new Feature(3.166f, 169.138f), new Feature(6.333f, 192.138f),
		// 	new Feature(7.916f, 197.138f), new Feature(9.5f, 192.138f),
		// 	new Feature(12.667f, 186.138f), new Feature(14.25f, 192.138f),
		// 	new Feature(15.833f, 197.138f), new Feature(17.41f, 192.138f),
		// 	new Feature(19, 186.138f), new Feature(20.58f, 180.138f),
		// 	new Feature(22.166f, 175.138f), new Feature(25.333f, 180.138f),
		// 	new Feature(26.916f, 175.138f), new Feature(28.583f, 169.138f)
		// };
		// sg1.setFeatures(features_1);
		// float[] weights_1 = new float[]{
		// 	0.5f, 0.5f, 1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1, 0.5f, 0.5f, 1
		// };
		// sg1.setWeights(weights_1);
		// Signiture sg2 = new Signiture();
		// Feature[] features_2 = new Feature[]{
		// 	new Feature(0, 180), new Feature(2.25f, 175),
		// 	new Feature(3, 169), new Feature(6, 192),
		// 	new Feature(7.5f, 197), new Feature(9, 192),
		// 	new Feature(12, 186), new Feature(13.5f, 192),
		// 	new Feature(15, 197), new Feature(16.5f, 192),
		// 	new Feature(18, 186), new Feature(19.5f, 180),
		// 	new Feature(22.5f, 175), new Feature(25.5f, 180),
		// 	new Feature(27.75f, 175), new Feature(28.5f, 169)
		// };
		// sg2.setFeatures(features_2);
		// float[] weights_2 = new float[]{
		// 	0.75f, 0.25f, 1, 0.5f, 0.5f, 1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 1, 1, 0.75f, 0.25f, 1
		// };
		// sg2.setWeights(weights_2);
		// Debug.Log(EMDProcessor.distance(sg1, sg2, 0));
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
				if (history[preBarIdx + 4].Count != 0) {
					Debug.Log("start calculating EMD");
					CalculateEMD(history[preBarIdx], history[preBarIdx + 4], barEndTime);
				}
			} else {
				if (history[preBarIdx - 4].Count != 0) {
					Debug.Log("Start calculating EMD");
					CalculateEMD(history[preBarIdx], history[preBarIdx - 4], barEndTime);
				}
			}
		}
		curBarBuffer.Clear();
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
	}
}
