using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;
public class MusicAnalysis : MonoBehaviour {
	Queue<double[]> key_down = new Queue<double[]>();
	Queue<float[]> key_up = new Queue<float[]>();
	public bool input_lock = true;
	AudioSource songAudioSource;
	AudioSource[] sourceArray;
	// Use this for initialization
	public bool active = false;
	void Awake()
	{
		sourceArray = GetComponents<AudioSource>();
		songAudioSource = sourceArray[0];
	}
	void Start () {
		
	}
	
	
	void Update () {
		for (int i=0; i < 128; i++){
			if(MidiMaster.GetKeyDown(i) && !input_lock){
				//Debug.Log(i.ToString()+" fixed "+songAudioSource.time);
				key_down.Enqueue(new double[]{i,MidiMaster.GetKey(i), songAudioSource.time});
			}
			if(MidiMaster.GetKeyUp(i) && !input_lock){
				//Debug.Log("key up " + i);
				key_up.Enqueue(new float[]{i,songAudioSource.time});
			}
		}
	}

	// void OnAudioFilterRead (float[] data, int channels) {
	// 	if (!active)
	// 		return;

	// 	// You can't execute any function of Unity here because this function is working on Unity Audio Thread (this ensure the Metronome Accuracy)
	// 	// To Fix that you need to execute your function on Main Thread again, don't worry i created an easy way to do that :D
	// 	// There are so much other fixes to do this, like Ninja Thread.
	// 	ToMainThread.AssignNewAction ().ExecuteOnMainThread (trackKey());
	// }

	// IEnumerator trackKey () {
	// 	if (!active)
	// 		yield return null;
	// 	for (int i=0; i < 128; i++){
	// 		if(MidiMaster.GetKeyDown(i) && !input_lock){
	// 			Debug.Log(i.ToString()+" nofix "+songAudioSource.time);
	// 			key_down.Enqueue(new double[]{i,MidiMaster.GetKey(i), songAudioSource.time});
	// 		}
	// 		if(MidiMaster.GetKeyUp(i) && !input_lock){
	// 			//Debug.Log("key up " + i);
	// 			key_up.Enqueue(new float[]{i,songAudioSource.time});
	// 		}
	// 	}
	// }

	public void analysisMusic(int currentTick, List<double> songTickTimes){
		input_lock = true;
		Debug.Log(offBeatDetection(currentTick, songTickTimes));
		key_down.Clear();
		key_up.Clear();
		input_lock = false;
	}

	public double onBeatDetection(int currentTick, List<double> songTickTimes){
		double offBeat = 0;
		double numBeat = key_down.Count;
		double interval = (songTickTimes[1] - songTickTimes[0]);
		double bar_length = songTickTimes[songTickTimes.Count-1] - songTickTimes[0];
		for (int i = currentTick - 31; i <= currentTick; i++){
			if (key_down.Count == 0) break;
			while (key_down.Peek()[2] < songTickTimes[i+1]){
				offBeat += System.Math.Abs(interval/2 - System.Math.Abs(songTickTimes[i] + interval/2 - key_down.Peek()[2]))/(interval/2);
				key_down.Dequeue();
				if (key_down.Count == 0) break;
			}
		}
		
		return (offBeat/numBeat)*1000;
	}

	public double offBeatDetection(int currentTick, List<double> songTickTimes){
		double offBeat = 0;
		double numOnset = key_down.Count;
		double interval = (songTickTimes[1]-songTickTimes[0])/2;
		double tri_interval = (songTickTimes[1]-songTickTimes[0])/3;
		double bar_length = songTickTimes[songTickTimes.Count-1] - songTickTimes[0] + interval;
		double markIdx = songTickTimes[currentTick-31];
		Debug.Log(markIdx);
		Debug.Log(interval);
		double tri_markIdx = songTickTimes[currentTick-31];
		foreach (double[] onset in key_down){
			if (onset[2] > markIdx){
				while (markIdx < onset[2])
					markIdx += interval;
			}
			if (onset[2] > tri_markIdx){
				while (tri_markIdx < onset[2])
					tri_markIdx += tri_interval;
			}
			Debug.Log(onset[2]+" "+(markIdx-interval));
			
			double tmp_offBeat = System.Math.Abs(interval/2 - System.Math.Abs(markIdx - interval/2 - onset[2]))/(interval/2);
			double tri_tmp_offBeat = System.Math.Abs(tri_interval/2 - System.Math.Abs(tri_markIdx - tri_interval/2 - onset[2]))/(tri_interval/2);
			//Debug.Log("off"+tmp_offBeat);
			//Debug.Log("tri_off"+tri_tmp_offBeat);
			//offBeat += System.Math.Min(tmp_offBeat, tri_tmp_offBeat);
			offBeat += (tmp_offBeat * 1 + tri_tmp_offBeat * 0);
		}
		
		return (offBeat/numOnset)*1000;
	}
}
