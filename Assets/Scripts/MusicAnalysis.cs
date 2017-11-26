using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MidiJack;
public class MusicAnalysis : MonoBehaviour {
	Queue<double[]> key_down = new Queue<double[]>();
	Queue<float[]> key_up = new Queue<float[]>();
	Dictionary<string, double[]> instructionMap = new Dictionary<string, double[]>();
	private bool input_lock = false;
	AudioSource songAudioSource;
	AudioSource[] sourceArray;
	// Use this for initialization
	public bool active = false;
	private Manager manager;
	void Awake()
	{
		sourceArray = GetComponents<AudioSource>();
		songAudioSource = sourceArray[0];
		instructionMap.Add("movef", new double[]{4,2,2,2,2,4});
		instructionMap.Add("moveb", new double[]{4,2,4,2,3,1});
		instructionMap.Add("attack", new double[]{3,1,4,2,3,2,1});
		instructionMap.Add("defence", new double[]{2,1,1,1,2,1,2,3,2,1});
		manager = GetComponent<Manager>();
	}
	
	void Update () {
		for (int i=0; i < 128; i++){
			if(MidiMaster.GetKeyDown(i) && !input_lock){
				//Debug.Log(i.ToString()+" fixed "+songAudioSource.time);
				manager.GetPlayerController().prepare();
				key_down.Enqueue(new double[]{i,MidiMaster.GetKey(i), songAudioSource.time});
			}
			if(MidiMaster.GetKeyUp(i) && !input_lock){
				//Debug.Log("key up " + i);
				key_up.Enqueue(new float[]{i,songAudioSource.time});
			}
		}
	}

	public string analysisMusic(int currentTick, List<double> songTickTimes){
		string res ="";
		input_lock = true;
		// string instruction = manager.checkProgress();
		// double[] rhythmTemplate = instructionMap[instruction];
		// double accuracy = rhythmDetection(currentTick, songTickTimes, rhythmTemplate, 0);
		// Debug.Log("rhythm accuracy: " + accuracy);
		// manager.incrementProgress(accuracy >= 0.8);
		// if (accuracy >= 0.8){
		// 	res = instruction;
		// }else{
		// 	res = "failed";
		// }
		res = rhythmDetection(currentTick, songTickTimes, instructionMap);
		key_down.Clear();
		key_up.Clear();
		input_lock = false;
		return res;
	}
	// public string matchInstruction(int currentTick, List<double> songTickTimes){
	// 	string res = "";
	// 	foreach (KeyValuePair<string, double[]> entry in instructionMap){
	// 		if (rhythmDetection(currentTick, songTickTimes, entry.Value, 2) == 1){
	// 			res = entry.Key;
	// 			break;
	// 		} 
	// 	}
	// 	return res = (res != "") ? res : "failed";
	// }

	public string rhythmDetection(int currentTick, List<double> songTickTimes, Dictionary<string, double[]> rhythmTemplates){
		if (key_down.Count == 0) return "failed";
		double interval = (songTickTimes[1] - songTickTimes[0])/4;
		double[][] key_down_copy = key_down.ToArray();
		double[] prev_onset = key_down_copy[0];
		List<double> diff_onset = new List<double>();
		for (int k = 1; k < key_down_copy.Length; k++){
			diff_onset.Add(System.Math.Round((key_down_copy[k][2]-prev_onset[2])/interval));
			prev_onset = key_down_copy[k];
		}
		diff_onset.Add(System.Math.Round((songTickTimes[currentTick-1]-key_down_copy[key_down.Count-1][2])/interval));
		
		string interonsite_str = "";
		foreach(double interonsite in diff_onset){
			interonsite_str += (" "+interonsite);
		}
		Debug.Log(interonsite_str);
		string res = "";
		foreach (KeyValuePair<string, double[]> entry in instructionMap) {
			double[] rhythmTemplate = entry.Value;
			int miss = 0;
			if (diff_onset.Count == rhythmTemplate.Length) {
				for(int i = 0; i < diff_onset.Count; i++){
					if (diff_onset[i] != rhythmTemplate[i]){
						miss++;
					}
				}
			} else {
				miss = rhythmTemplate.Length;
			}
			
			if (miss == 0) {
				res = entry.Key;
				break;
			} else {
				res = "failed";
			}
			Debug.Log(miss);
		}
		return res;
	}

	public double rhythmDetection(int currentTick, List<double> songTickTimes, double[] rhythmTemplate, int searchMethod){
		if (key_down.Count == 0) return 0;
		double interval = (songTickTimes[1] - songTickTimes[0])/4;
		
		double[][] key_down_copy = key_down.ToArray();
		double[] prev_onset = key_down_copy[0];
		int miss, i=0 ,j = 0;
		List<double> diff_onset = new List<double>();

		for (int k = 1; k < key_down_copy.Length; k++){
			diff_onset.Add(System.Math.Round((key_down_copy[k][2]-prev_onset[2])/interval));
			prev_onset = key_down_copy[k];
		}

		diff_onset.Add(System.Math.Round((songTickTimes[currentTick-1]-key_down_copy[key_down.Count-1][2])/interval));
		
		string interonsite_str = "";
		foreach(double interonsite in diff_onset){
			interonsite_str += (" "+interonsite);
		}
		Debug.Log(interonsite_str);

		if (searchMethod == 0){
			if (rhythmTemplate.Length < diff_onset.Count){
				miss = rhythmTemplate.Length;
				for(int k = 0; k < diff_onset.Count-rhythmTemplate.Length; k++){
					int numMiss = 0;
					for(int t = k; t < rhythmTemplate.Length; t++){
						if(rhythmTemplate[t]!=diff_onset[t]) numMiss++;
					}
					if (miss > numMiss) miss = numMiss;
				}
			}else{
				Debug.Log("here");
				miss = diff_onset.Count;
				for(int k = 0; k <= rhythmTemplate.Length-diff_onset.Count; k++){
					int numMiss = 0;
					for(int t = k; t < diff_onset.Count; t++){
						if(rhythmTemplate[t]!=diff_onset[t]) numMiss++;
					}
					if (miss > numMiss) miss = numMiss;
				} 
			}
			Debug.Log(miss);
		}else if (searchMethod == 1){
			miss = 0;
			while (i < rhythmTemplate.Length && j < diff_onset.Count){
				if (rhythmTemplate[i] == diff_onset[j]){
					i++;
					j++;
				}else if (rhythmTemplate[i] > diff_onset[j]){
					double tmp = 0;
					while(rhythmTemplate[i] > tmp){
						if (j > diff_onset.Count-1) break;
						tmp += diff_onset[j];
						j++;
					}
					miss++;
				}else if (rhythmTemplate[i] < diff_onset[j]){
					double tmp = 0;
					while(diff_onset[j] > tmp){
						if (i > rhythmTemplate.Length-1) break;
						tmp += rhythmTemplate[i];
						i++;
					}
					miss++;
				}
			}
		} else {
			miss = rhythmTemplate.Length;
		}
		return 1 - miss/(float)rhythmTemplate.Length;
	}

	public double offBeatDetection(int currentTick, List<double> songTickTimes, int numBar){
		double offBeat = 0;
		double numOnset = key_down.Count;
		double interval = (songTickTimes[1]-songTickTimes[0])/2;
		double tri_interval = (songTickTimes[1]-songTickTimes[0])/3;
		double bar_length = songTickTimes[songTickTimes.Count-1] - songTickTimes[0] + interval;
		double markIdx = songTickTimes[currentTick-(numBar*4-1)];
		double tri_markIdx = songTickTimes[currentTick-(numBar*4-1)];
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
			offBeat += (tmp_offBeat * 1 + tri_tmp_offBeat * 0);
		}
		return (offBeat/numOnset)*1000;
	}
}
