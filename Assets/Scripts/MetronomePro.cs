// Created by Carlos Arturo Rodriguez Silva (Legend)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MetronomePro : MonoBehaviour {
	
	[Header("Variables")]
	public bool active = false;
	[Space(5)]
	private AudioSource metronomeAudioSource;
	public AudioClip highClip;
	public AudioClip lowClip;

	[Space(5)]

	private AudioSource songAudioSource;

	[Header("Metronome (this values will be overrided by Song Data)")]

	public double Bpm = 140.0f;
	public double OffsetMS = 0;
	

	public int Step = 4;
	public int Base = 4;

	public int CurrentMeasure = 0;
	public int CurrentStep = 0;
	public int CurrentTick;
	public int barOffset = 0;
	public int roundLength = 2;
	public List<Double> songTickTimes;

	double interval;

	public bool neverPlayed = true;
	private MusicAnalysis musicAnalysis;
	private Manager manager;
	void Awake()
	{
		AudioSource[] source_array = GetComponents<AudioSource>();
		songAudioSource = source_array[0];
		metronomeAudioSource = source_array[2];
		musicAnalysis = GetComponent<MusicAnalysis>();
		manager = GetComponent<Manager>();
	}

	public void GetSongData (double _bpm, double _offsetMS, int _base, int _step) {
		Bpm = _bpm;
		OffsetMS = _offsetMS;
		Base = _base;
		Step = _step;
	}

	// Set the new BPM when is playing
	public void UpdateBPM () {
		try {
			SetDelay ();
		} catch {
			
			Debug.Log ("Please enter the new BPM value correctly.");
		}
	}

	// Set the new Offset when is playing
	public void UpdateOffset () {
		try {
			SetDelay ();
		} catch {
			
			Debug.Log ("Please enter the new Offset value correctly.");
		}
	}


	void SetDelay () {
		bool isPlaying = false;

		if (songAudioSource.isPlaying) {
			isPlaying = true;
		}


		songAudioSource.Pause ();

		CalculateIntervals ();
		CalculateActualStep ();

		if (isPlaying) {
			songAudioSource.Play ();
		}
	}

	// Play Metronome
	public void Play () {
		if (neverPlayed) {
			CalculateIntervals ();
		}

		neverPlayed = false;
		active = true;
		musicAnalysis.active = true;
	}

	// Pause Metronome
	public void Pause () {
		active = false;
	}

	// Stop Metronome
	public void Stop () {
		active = false;

		CurrentMeasure = 0;
		CurrentStep = 4;
		CurrentTick = 0;
	}

	// Calculate Time Intervals for the song
	public void CalculateIntervals () {
		try {
			active = false;
			var multiplier = Base / Step;
			var tmpInterval = 60f / Bpm;
			interval = tmpInterval / multiplier;

			int i = 0;

			songTickTimes.Clear ();

			while (interval * i <= songAudioSource.clip.length) {
				songTickTimes.Add ((interval * i) + (OffsetMS / 1000f));
				i++;
			}

			active = true;
		} catch {
			
			Debug.LogWarning ("There isn't an Audio Clip assigned in the Player.");
		}
	}

	// Calculate Actual Step when the user changes song position in the UI
	public void CalculateActualStep () {
		active = false;

		// Get the Actual Step searching the closest Song Tick Time using the Actual Song Time
		for (int i = 0; i < songTickTimes.Count; i++) {
			if (songAudioSource.time < songTickTimes[i]) {
				CurrentMeasure = (i / Base);
				CurrentStep = (int)((((float)i / (float)Base) - (i / Base)) * 4);
				if (CurrentStep == 0) {
					CurrentMeasure = 0;
					CurrentStep = 4;
				} else {
					CurrentMeasure++;
				}

				CurrentTick = i;
				Debug.Log ("Metronome Synchronized at Tick: " + i + " Time: "+ songTickTimes[i]);
				break;
			}
		}
		active = true;
	}

	// Read Audio (this function executes from Unity Audio Thread)
	void OnAudioFilterRead (float[] data, int channels) {
		if (!active)
			return;

		// You can't execute any function of Unity here because this function is working on Unity Audio Thread (this ensure the Metronome Accuracy)
		// To Fix that you need to execute your function on Main Thread again, don't worry i created an easy way to do that :D
		// There are so much other fixes to do this, like Ninja Thread.
		ToMainThread.AssignNewAction ().ExecuteOnMainThread (CalculateTicks());
	}
		
	// Metronome Main function, this calculates the times to make a Tick, Step Count, Metronome Sounds, etc.
	IEnumerator CalculateTicks () {
		if (!active)
			yield return null;

		// Check if the song time is greater than the current tick Time
		if (songAudioSource.time >= songTickTimes [CurrentTick]) {

			CurrentTick++;

			if (CurrentTick >= songTickTimes.Count) {
				active = false;
			}

			// If the Current Step is greater than the Step, reset it and increment the Measure
			if (CurrentStep >= Step) {
				CurrentStep = 1;
				CurrentMeasure++;
				metronomeAudioSource.clip = highClip;
			} else {
				CurrentStep++;
				metronomeAudioSource.clip = lowClip;
			}

			// Call OnTick functions
			StartCoroutine (manager.OnTick(CurrentTick, songTickTimes, barOffset, Step, roundLength));
		}

		yield return null;
	}

	// Tick Time (execute here all what you want)
	// IEnumerator OnTick () {
	// 	// Play Audio Tick
	// 	metronomeAudioSource.Play ();
	// 	// YOUR FUNCTIONS HERE
		
	// 	if (CurrentTick >= barOffset * Step){
	// 		if ((CurrentTick - barOffset * Step - 1) % (roundLength * Step * 2) == 0){
	// 			if (CurrentTick != barOffset * Step + 1) musicAnalysis.analysisMusic(CurrentTick, songTickTimes);
				
	// 		}
	// 	}

	// 	if ((CurrentTick-5)%8 == 0){
	// 		//Debug.Log(CurrentTick);
	// 		if (CurrentTick == 5){
	// 			Debug.Log("input start");
	// 			musicAnalysis.input_lock = false;
	// 		}else{
	// 			Debug.Log("analysis start");
	// 			musicAnalysis.analysisMusic(CurrentTick, songTickTimes);
	// 		}
			
	// 	}

	// 	//Debug.Log ("Current Step: " + CurrentStep + "/" + Step);
	// 	yield return null;
	// }
}