using UnityEngine;
using System.Collections;

public class metronome : MonoBehaviour {

	public double bpm = 90.0f;
	// public GameObject boss;
	// bossController boss_controller;
	// SpriteRenderer boss_render;
	// bool flip = true;
	// bool r_or_b = true;
	// bool first_time = false;
	double nextTick = 0.0f;
	//double sampleRate = 0.0f;
	bool ticked = false;
	int tick_times = 0;

// 	public int Base = 4;
// public int Step = 4;
// public float BPM = 162;
// public int CurrentStep = 1;
// public int CurrentMeasure = 1;

// private float interval;
// private float nextTime;
	// AudioSource[] source_array;
	// AudioSource bt_source;
	// AudioSource solo_source;
	// Object[] bt_clips_raw;
	// Object[] solo_clips_raw;

	// AudioClip[] bt_clips;
	// AudioClip[] solo_clips;
	// Use this for initialization
	// bool first = true;
	// void Awake()
	// {
	// 	bt_clips_raw = Resources.LoadAll("AudioFiles/backing_track", typeof(AudioClip));
	// 	solo_clips_raw = Resources.LoadAll("AudioFiles/solo", typeof(AudioClip));
	// 	bt_clips = new AudioClip[bt_clips_raw.Length];
	// 	solo_clips = new AudioClip[solo_clips_raw.Length];
	// 	for (int i = 0; i<bt_clips_raw.Length;i++){
	// 		bt_clips[i] = (AudioClip)bt_clips_raw[i];
	// 	}
	// 	for (int i = 0; i<solo_clips_raw.Length;i++){
	// 		solo_clips[i] = (AudioClip)solo_clips_raw[i];
	// 	}
	// 	source_array = GetComponents<AudioSource>();
	// 	bt_source = source_array[0];
	// 	solo_source = source_array[1];
	// 	bt_source.clip = bt_clips[0];
	// 	solo_source.clip = solo_clips[0];
	// 	boss_controller = boss.GetComponent<bossController>();
	// 	boss_render = boss.GetComponent<SpriteRenderer>();
	// }
	void Start () {
		
		double startTick = AudioSettings.dspTime;
		nextTick = startTick + (60.0 / bpm);
		//sampleRate = AudioSettings.outputSampleRate;
		
		// bt_source.Play();
		// solo_source.Play();
		//StartMetronome();
	}
	
	// Update is called once per frame
	void Update () {
		if ( !ticked && nextTick >= AudioSettings.dspTime ) {
			ticked = true;
			tick_times++;
			//Debug.Log("ticked");
			BroadcastMessage("OnTick", tick_times);
		}

	}
	// void OnTick() {
	// 	if (tick_times+1 <= 128){
	// 		if ((tick_times+1)%32 == 0){
	// 		int random = Random.Range(0,13);
	// 		solo_source.clip = solo_clips[random];
	// 		solo_source.Play();
	// 		}
	// 	}else if (tick_times+1 <= 144 && tick_times + 1 >128){
	// 		if (first){
	// 			bt_source.clip = bt_clips[1];
	// 			bt_source.Play();
	// 			first = false;
	// 		}

	// 	}else if (tick_times+1 <= 272 && tick_times + 1 > 144){
	// 		if (!first){
	// 			bt_source.clip = bt_clips[2];
	// 			bt_source.Play();
	// 			first = !first;
	// 		}
	// 		if ((tick_times+1)%32 == 0){
	// 			int random = Random.Range(14,22);
	// 			solo_source.clip = solo_clips[random];
	// 			solo_source.Play();
	// 		}
	// 	}else if (tick_times + 1 > 272 && tick_times + 1 < 304){
	// 		if (first){
	// 			bt_source.clip = bt_clips[3];
	// 			bt_source.Play();
	// 			first = false;
	// 		}
			
	// 	}else{
	// 		bt_source.Stop();
	// 	}
	// 	if ((tick_times+1) % 32 == 0){
	// 		if(first_time){
	// 			first_time = false;
	// 		}else{
	// 			if(flip){
	// 				if(r_or_b){
	// 					boss_controller.switchColor();
						
	// 				}else{
	// 					boss_controller.switchColor();
						
	// 				}
	// 				flip = !flip;
	// 			}else{
	// 				if(r_or_b){
	// 					boss_controller.attack(0);
	// 					r_or_b = !r_or_b;
	// 				}else{
	// 					boss_controller.attack(1);
	// 					r_or_b = !r_or_b;
	// 				}
	// 				flip = !flip;
	// 			}
	// 		}
	// 	}
		
		
	// }

	void FixedUpdate() {
		double timePerTick = 60.0f / bpm;
		double dspTime = AudioSettings.dspTime;

		while (dspTime >= nextTick) {
			ticked = false;
			nextTick += timePerTick;
		}
	}
	// public void StartMetronome()
    // {
    //     //StopCoroutine("DoTick");
    //     CurrentStep = 1; 
    //     var multiplier = Base / 4f;
    //     var tmpInterval = 60f / BPM;
    //     interval = tmpInterval / multiplier; 
    //     nextTime = Time.time; // set the relative time to now
    //     StartCoroutine("DoTick"); 
    // }
	// IEnumerator DoTick() // yield methods return IEnumerator
    // {
    //     for (; ; )
    //     {
    //         Debug.Log("bop");
    //         // do something with this beat
    //         nextTime += interval; // add interval to our relative time
    //         yield return new WaitForSeconds(nextTime - Time.time); // wait for the difference delta between now and expected next time of hit
    //         CurrentStep++;
    //         if (CurrentStep > Step)
    //         {
    //             CurrentStep = 1;
    //             CurrentMeasure++;
    //         }
    //     }
    // }

}
